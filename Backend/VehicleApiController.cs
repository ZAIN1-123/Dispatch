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
    public class VehicleApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<VehicleApiController> logger;
        private IWebHostEnvironment Environment;
        public VehicleApiController(ILogger<VehicleApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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

                List<Dispatch> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<Dispatch>("select Dispatch.Id,Dispatch.VehicleNo as VehicleNo from Dispatch where Status=0").ToList();

                    return Ok(lst);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });

            }
        }

        [HttpGet("{id}")]

        public IActionResult Get(string id)
        {
            try
            {
                Dispatch dispatch;

              //  List<Dispatch> lst = new List<Dispatch>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    dispatch = conn.Query<Dispatch>("select * from Dispatch where id = " + id).FirstOrDefault();
                    dispatch.stock = conn.Query<Stock>("select Stock.Number as ReelNumber,Slip.NetWeight as VNetWeight, Stock.DispatchId ,Dispatch.VehicleNo as VehicleNo from Stock " +
                        "left join Dispatch on  Stock.DispatchId= Dispatch.Id  left join Slip on Stock.SlipId = Slip.Id where Dispatch.Id=" + id).ToList();

                    //lst = conn.Query<Dispatch>("select Stock.Number as ReelNumber,Slip.NetWeight as VNetWeight, Stock.DispatchId as Id,Dispatch.VehicleNo as VehicleNo from Stock " +
                    //    "left join Dispatch on  Stock.DispatchId= Dispatch.Id  left join Slip on Slip.FormattedNo=Stock.Number where Dispatch.Status=0 and  Dispatch.Id=" + id).ToList();
                }
                return Ok(dispatch);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }
    }
}
