
namespace FoodTourneyBack.Models
{
    public class Place
    {
        public int PlaceId { get; set; }
        public string Name { get; set; }
	public string Vicinity { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
