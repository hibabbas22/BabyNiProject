using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.Data.Services;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendHourlyController : ControllerBase
    {
        public static SendHourlyService sendservice;
        public SendHourlyController(SendHourlyService sendService)
        {
            sendservice = sendService;
        }

        [HttpPost("getdata")]
        public IActionResult sendData()
        {
            var startdate = HttpContext.Request.Form["startDate"];
            var enddate = HttpContext.Request.Form["endDate"];
            var data = sendservice.SendDataTime(startdate, enddate);
            return Ok(data);
         } 
    }
}
