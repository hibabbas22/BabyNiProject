using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using Serilog;
using System;
using System.IO;

namespace webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json")
                      .Build();
            CreateHostBuilder(args).Build().Run();

        }
        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
        //.UseSerilog() //Uses Serilog instead of default .NET Logger    
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }
}