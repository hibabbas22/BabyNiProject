using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webapi.Data.Services;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParseController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ParseController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
 
        public static ParserService _parserService;
        public ParseController(ParserService parserService)
        {
            _parserService = parserService;

        }
        [HttpGet]
        public IActionResult readCsv()
        {
            _parserService.ReadCsv();
            return Ok();
        }

        //watcher
        public static void OnCreated(object sender, FileSystemEventArgs e)
        {
            _parserService.Parse(e.FullPath);
            _parserService.ReadCsv(e.FullPath);
        }
    }
}
