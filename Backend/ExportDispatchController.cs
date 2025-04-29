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
using System.Text;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class ExportDispatchController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<ExportDispatchController> logger;
        private IWebHostEnvironment Environment;
        public ExportDispatchController(ILogger<ExportDispatchController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }


        public IActionResult Get(string vehicleNo)
        {
            try
            {
                Dispatch dispatch = new Dispatch();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    dispatch = conn.Query<Dispatch>("select Dispatch.* from Dispatch where VehicleNo='" + vehicleNo + "' order by Dispatch.Id desc").FirstOrDefault();
                    //dispatch = conn.Query<Dispatch>("select Dispatch.* from Dispatch where Date=" + DateTime.Now.ToString("yyyyMMdd") + " and VehicleNo='" + vehicleNo + "'").FirstOrDefault();

                    //if (dispatch == null)
                    //{
                    //    //dispatch = conn.Query<Dispatch>("select Dispatch.* from Dispatch where Date=" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + " and VehicleNo='" + vehicleNo + "'").FirstOrDefault();
                    //    dispatch = conn.Query<Dispatch>("select Dispatch.* from Dispatch where  VehicleNo='" + vehicleNo + "'").FirstOrDefault();
                    //}

                    List<ExportPackingSlip> lst = new List<ExportPackingSlip>();

                    lst = conn.Query<ExportPackingSlip>("select Items.Name as ItemName, GSM.Name as GSMName , " +
                        "Size.Name as SizeName,Size.Unit as SizeUnit,Slip.FormattedNo as ReelNo,Slip.NetWeight from Stock" +
                        " left join slip on Stock.SlipId=Slip.Id" +
                        " left join Items on Slip.Quality = Items.Id " +
                        " left join GSM on Slip.GSMId = GSM.Id " +
                        " left join Size on Slip.SizeId=Size.Id" +
                        " where DispatchId=" + dispatch.Id).ToList();
                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }
    }

    public class ExportPackingSlip
    {
        public string ItemName { get; set; }        
        public string ReelNo { get; set; }
        public string SizeName { get; set; }
        public string GSMName { get; set; }
        public string SizeUnit { get; set; }
        public decimal NetWeight { get; set; }
    }
}






