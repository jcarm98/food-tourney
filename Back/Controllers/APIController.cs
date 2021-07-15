using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using FoodTourneyBack.Models;
using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;
using System.Dynamic;

namespace FoodTourneyBack.Controllers
{
	[Route("/")]
	[ApiController]
	public class APIController : ControllerBase
	{
		private readonly ZipContext _context;
		public APIController(ZipContext context) {
			_context = context;
		}

		[HttpGet("places/")]
		public string getPlaces(
			[FromQuery, BindRequired] double lat,
			[FromQuery, BindRequired] double lng,
			[FromQuery] int? amount
		) {
			string raw = "";
			const int defaultAmount = 25;
			var wc = new WebClient();

			string apiKey = "secret_google_api_key";
			string URI = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location="
			+ lat
			+ ","
			+ lng
			+ "&rankby=distance&type=restaurant&key=" + apiKey;

			Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
			if (amount == null) { amount = defaultAmount; }
			string nextPageToken = "";
			int loopStop = 100;
			int successCount = 0;
			// Need to hit google api up to 3 times
			while (response.Count < amount && loopStop != 0 && successCount < 3) {
				loopStop--;

				// To avoid google caching invalid responses to requests
				// Add parameter to query string that always changes 
				string temp = URI
					+ "&pagetoken="
					+ nextPageToken
					+ "&request_count="
					+ loopStop.ToString();

				string googleMapsResponse = (nextPageToken == "")
					? wc.UploadString(URI, "GET")
					: wc.UploadString(temp, "GET");
				wc.Dispose();

				raw += googleMapsResponse;
				if (JsonSerializer.Deserialize<dynamic>(googleMapsResponse)
					.GetProperty("status").ToString() == "INVALID_REQUEST") {
					System.Threading.Thread.Sleep(100);
					continue;
				}
				++successCount;

				// Need multiple web clients to send multiple requests
				wc = new WebClient();

				dynamic fragment = JsonSerializer.Deserialize<dynamic>(googleMapsResponse);
				var it = fragment.GetProperty("results").EnumerateArray();
				bool hasNext = it.MoveNext();

				while (hasNext && response.Count < amount){
					// Only add one place with the same name, no two Starbucks, etc.
					if (!response.ContainsKey(it.Current.GetProperty("name").ToString())){
						dynamic newObject = new ExpandoObject();
						newObject.PlaceId = -1;
						newObject.Lat = it.Current.GetProperty("geometry").GetProperty("location").GetProperty("lat").GetDouble();
						newObject.Lng = it.Current.GetProperty("geometry").GetProperty("location").GetProperty("lng").GetDouble();
						newObject.Name = it.Current.GetProperty("name").ToString();
						newObject.Vicinity = it.Current.GetProperty("vicinity").ToString();
						try{
							newObject.Rating = it.Current.GetProperty("rating").ToString();
							newObject.TotalRatings = it.Current.GetProperty("user_ratings_total").ToString();
						}
						catch(KeyNotFoundException e){
							newObject.Rating = "none";
							newObject.TotalRatings = "0";
						}
						
						response.Add(newObject.Name, newObject);
					}
					// Iterate through using enumerate array
					hasNext = it.MoveNext();
				}

				// Unable to get next page token using getproperty
				// Using regex as a workaround
				var it2 = fragment.EnumerateObject();
				string tokenString = "";
				bool hasNext2 = it2.MoveNext();
				int counter = 0;
				while (hasNext2) {
					// 0: html_attributions: []
					// 1: next_page_token: string
					// 2: results: json[]
					if (counter == 1)
						tokenString += (counter) + "\t" + it2.Current.ToString() + "\n";
					counter++;
					// Iterate through using enumerate object
					hasNext2 = it2.MoveNext();
				}
				Match m = Regex.Match(tokenString, @"\b[A-Za-z0-9-_]*\b");
				m = m.NextMatch()
					.NextMatch()
					.NextMatch()
					.NextMatch();
				nextPageToken = m.Value;
			}

			wc.Dispose();
			try { return JsonSerializer.Serialize(response); }
			catch (KeyNotFoundException e) { return e.ToString(); }
		}

		[HttpPost("tally/{zip}")]
		public int Tally(int zip)
		{
			using (var reader = new StreamReader(Request.Body))
			{
				var body = reader.ReadToEndAsync();
				string res = body.Result;
				Place obj = JsonSerializer.Deserialize<Place>(res, new JsonSerializerOptions());
				var pidList = _context.Places
					.Where(p => p.Lat == obj.Lat && p.Lng == obj.Lng)
					.ToList<Place>();

				int pid = (pidList.Count == 0) ? -1 : pidList[0].PlaceId;

				if (pid == -1) {
					// need to add place with new placeid
					// and update pid variable

					// get highest placeid and add 1
					pid = _context.Places
						.Where(p => p.PlaceId != -1)
						.Select(p => p.PlaceId)
						.DefaultIfEmpty()
						.Max() + 1;

					_context.Places.Add(new Place()
					{
						PlaceId = pid,
						Name = obj.Name,
						Lat = obj.Lat,
						Lng = obj.Lng,
						Vicinity = obj.Vicinity
					});
					_context.SaveChanges();
				}

				List<ZipTuple> rows = _context.Zips
					.Where(z => z.Zip == zip && z.PlaceId == pid)
					.ToList<ZipTuple>();

				ZipTuple row = (rows.Count > 0)
					? rows[0]
					: new ZipTuple() { Id = -1 };

				if (row.Id == -1) {
					int nextZipId = _context.Zips
						.Select(z => z.Id)
						.DefaultIfEmpty()
						.Max() + 1;

					row.Id = nextZipId;
					row.Zip = zip;
					row.PlaceId = pid;
					row.Count = 1;

					_context.Zips.Add(row);
					_context.SaveChanges();
				} else {
					row.Count += 1;
					_context.SaveChanges();
				}
				return 1;
			}
		}

		// ex. GET popular/90210
		// Returns object with information about popular picks for a given zip code
		// If object has placeid -1 then there is no popular pick for a zip code
		[HttpGet("popular/")]
		public Place GetPopular([FromQuery, BindRequired] int zip)
		{
			List<int> tuples = _context.Zips
				.OrderByDescending(z => z.Count)
				.Where(z => z.Zip == zip)
				.Select(z => z.PlaceId)
				.ToList<int>();

			int placeId =
				(tuples.Count != 0)
					? tuples[0]
					: -1;

			if (placeId == -1) {
				return new Place() { PlaceId = -1, };
			} else {
				// return place object of the popular place for a zip code
				return _context.Places
					.Where(p => p.PlaceId == placeId)
					.ToList<Place>()[0];
			}
		}
	}
}
