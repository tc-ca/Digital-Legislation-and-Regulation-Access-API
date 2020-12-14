using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LegsandRegsCS.Models;
using LegsandRegsCS.Data;
using Microsoft.Extensions.Primitives;
using Microsoft.ApplicationInsights;

namespace LegsandRegsCS
{
    public class Program
    {
        public static string secretTokenHeader;
        public static StringValues secretToken;
        public static bool downForMaintenance = false;
        public static string databaseUpdatePassword;
        public static Language[] languages;
        public static string primaryDateSouceURL;

        public static IServiceProvider services;
        public static TelemetryClient telemetry;

        public static void Main(string[] args)
        {
            
            var host = CreateHostBuilder(args).Build();

            var scope = host.Services.CreateScope();

            //Adding services to class variable so that it can be accessed by the SeedData methods, which will need it to make DB changes
            Program.services = scope.ServiceProvider;
            
            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
