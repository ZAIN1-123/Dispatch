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
    public class BundleVehicleApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<BundleVehicleApiController> logger;
        private IWebHostEnvironment Environment;
        public BundleVehicleApiController(ILogger<BundleVehicleApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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

                List<BundleDispatch> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<BundleDispatch>("select BundleDispatch.Id,BundleDispatch.VehicleNo as VehicleNo from BundleDispatch where Status=0").ToList();

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
                BundleDispatch dispatch;

                //  List<Dispatch> lst = new List<Dispatch>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    dispatch = conn.Query<BundleDispatch>("select * from Dispatch where id = " + id).FirstOrDefault();
                    dispatch.BundleStock = conn.Query<BundleStock>("select BundleStock.Number as ReelNumber,Bundle.BundleWeight as VBundleWeight, BundleStock.BundleDispatchId ,BundleDispatch.VehicleNo as VehicleNo from BundleStock " +
                        "left join BundleDispatch on  BundleStock.BundleDispatchId= BundleDispatch.Id  left join Bundle on BundleStock.BundleId = Bundle.Id where BundleDispatch.Id=" + id).ToList();

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
