using Dapper;
using DISPATCHAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class VehicleDispatchreturnController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<VehicleDispatchreturnController> logger;
        private IWebHostEnvironment Environment;
        public VehicleDispatchreturnController(ILogger<VehicleDispatchreturnController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }
        [HttpGet]
        public ActionResult Getch()
        {
            try
            {

                List<DispatchReturn> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<DispatchReturn>("select DispatchReturn.VehicleNo as VehicleNo from DispatchReturn where Status=0 ").ToList();

                    return Ok(lst);
                }
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });

            }
        }

        [HttpGet("{id}")]

        public IActionResult Get(string id)
        {
            try
            {
                List<DispatchReturn> lst = new List<DispatchReturn>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<DispatchReturn>("select DispatchReturnMeta.Number as ReelNumber,Slip.NetWeight as VNetWeight , DispatchReturnMeta.DispatchId as Id,DispatchReturnMeta.VehicleNo as VehicleNo from DispatchReturnMeta " +
                        "left join DispatchReturn on  DispatchReturn.Id = DispatchReturnMeta.DispatchId  left join Slip on Slip.FormattedNo=DispatchReturnMeta.Number where DispatchReturn.Status=0 and  DispatchReturn.VehicleNo='" + id + "'").ToList();
                }
                return Ok(lst);

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }
    }
}
