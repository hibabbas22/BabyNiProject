using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webapi.Controllers;
using webapi.Data.Services;

namespace webapi
{
    public class Startup
    {


        ParseController _parsercontrollers;
        ParserService _parserService;

        AggregatorController _aggregatorController;
        AggregatorService _aggregatorService;

        LoaderController loaderController;
        LoaderService loaderService;
    
        public string ConnectionString { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _parserService = new ParserService(configuration);
            _parsercontrollers = new ParseController(_parserService);

            loaderService = new LoaderService(configuration);
            loaderController = new LoaderController(loaderService);

            _aggregatorService = new AggregatorService(configuration);
            _aggregatorController = new AggregatorController(_aggregatorService);


            ConnectionString = Configuration.GetConnectionString("DefaultConnectionString");
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddTransient<LoaderService>();
            services.AddTransient<ParserService>();
            services.AddTransient<AggregatorService>();
            services.AddTransient<SendHourlyService>();
            services.AddTransient<SendDailyService>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "webapi", Version = "v1" });
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "webapi v1"));
            }
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //watcher
            var filesystemwatcher = new FileSystemWatcher(@"C:\Users\User\Desktop\Mini-Project\Backend\Parse")
            {
                Filter = "*.csv",
                NotifyFilter = NotifyFilters.Attributes
                            | NotifyFilters.CreationTime
                            | NotifyFilters.DirectoryName
                            | NotifyFilters.FileName
                            | NotifyFilters.LastAccess
                            | NotifyFilters.LastWrite
                            | NotifyFilters.Security
                            | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
         
            filesystemwatcher.Created += ParseController.OnCreated;
            filesystemwatcher.Created += LoaderController.OnCreated;


            var filesystemwatcher2 = new FileSystemWatcher(@"C:\Users\User\Desktop\Mini-Project\Backend\Load")
            {
                Filter = "*.txt",
                NotifyFilter  = NotifyFilters.Attributes
                            | NotifyFilters.CreationTime
                            | NotifyFilters.DirectoryName
                            | NotifyFilters.FileName
                            | NotifyFilters.LastAccess
                            | NotifyFilters.LastWrite
                            | NotifyFilters.Security
                            | NotifyFilters.Size,
            EnableRaisingEvents = true
            };

            filesystemwatcher2.Created += AggregatorController.OnCreated;
        }
    }
}
