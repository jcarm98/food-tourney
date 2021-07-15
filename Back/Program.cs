using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FoodTourneyBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var CertPem = File.ReadAllText("/etc/letsencrypt/live/foodtourney.app/cert.pem");
            //var EccPem = File.ReadAllText("/etc/letsencrypt/live/foodtourney.app/privKey.pem");
            //var cert = X509Certificate2.CreateFromPem(CertPem, EccPem);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseUrls("http://localhost:5001")
                        .UseStartup<Startup>();
                });
    }
}
