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
    public class SendDailyController : ControllerBase
    {
        public static SendDailyService sendservice;
        public SendDailyController(SendDailyService sendService)
        {
            sendservice = sendService;
        }

        [HttpPost("getdata")]
        public IActionResult postdailydata()
        {
            var startdate = HttpContext.Request.Form["startDate"];
            var enddate = HttpContext.Request.Form["endDate"];
            var data =sendservice.SendData(startdate, enddate);
            return Ok(data);
        }
    }
}
