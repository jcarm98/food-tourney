using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using FoodTourneyBack.Controllers;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using MySqlConnector.Authentication.Ed25519;

namespace FoodTourneyBack
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var pass = Environment.GetEnvironmentVariable("ASP_DATABASE_PASSWORD");
            // ASP_DATABASE_SERVER_IP
	    Ed25519AuthenticationPlugin.Install();
            var connectionString = "Server="
                + Environment.GetEnvironmentVariable("ASP_DATABASE_SERVER_IP")
                + "; Port="
                + Environment.GetEnvironmentVariable("ASP_DATABASE_PORT")
                + "; Database="
                + Environment.GetEnvironmentVariable("ASP_DATABASE_NAME")
                + "; Uid="
                + Environment.GetEnvironmentVariable("ASP_DATABASE_USER")
                + "; Pwd="
                + pass
                + ";";
            services.AddControllers();
            var serverVersion = new MariaDbServerVersion(new Version(10, 3, 29));
            services.AddDbContext<ZipContext>(
                options => options
                    .UseMySql(connectionString, serverVersion)
                    //.EnableSensitiveDataLogging()   // Comment these 2
                    //.EnableDetailedErrors()         // Lines out for production
            );
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.KnownProxies.Add(IPAddress.Parse(Environment.GetEnvironmentVariable("ASP_HOST_IP")));
                options.KnownProxies.Add(IPAddress.Parse("127.0.1.1"));
            });
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("https://" + Environment.GetEnvironmentVariable("ASP_HOST"));
                        builder.WithOrigins("http://localhost:8080");
                        //.AllowAnyHeader().AllowAnyMethod();
                    });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Necessary for reverse proxy
            app.UseForwardedHeaders();
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
