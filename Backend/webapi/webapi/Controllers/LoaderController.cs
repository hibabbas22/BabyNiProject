using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webapi.Data.Services;
using static System.Net.WebRequestMethods;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaderController : ControllerBase
    {
        public static LoaderService loadservice;
        public ParserService parserservice;

        private readonly ILogger<LoaderController> _logger;
   
        public LoaderController(LoaderService loaderService)
        {
            loadservice = loaderService;
        }
        public LoaderController(LoaderService loadService, ParserService parserService, ILogger<LoaderController> logger
)
        {
            //controller can now see the service--.inject
            _logger = logger;
            loadservice = loadService;
            parserservice = parserService;
        }
        [HttpGet]
        public IActionResult LoadData()
        {
            loadservice.ConnectVertica();
            //_logger.Information("Data is Loaded Successfully");
            return Ok();
        }
        public static void OnCreated(object sender, FileSystemEventArgs e)
        {
            loadservice.ConnectVertica(e.FullPath);
            loadservice.Load(e.FullPath);
        }
    }
}
