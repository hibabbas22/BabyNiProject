using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webapi.Data.Services;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregatorController : ControllerBase
    {
        public static AggregatorService _aggregatorService;

        public AggregatorController(AggregatorService aggregatorService)
        {
            _aggregatorService = aggregatorService;
        }
        [HttpGet]
        public IActionResult aggregate()
        {
            _aggregatorService.aggregateDataHourly();
            _aggregatorService.aggregateDataDaily();
            return Ok();
        }
        public static void OnCreated(object sender, FileSystemEventArgs e)
        {
            _aggregatorService.aggregateDataHourly(e.FullPath);
            _aggregatorService.aggregateDataDaily(e.FullPath);
            _aggregatorService.AggregateNew();
        }
    }
}

