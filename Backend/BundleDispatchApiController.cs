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
    public class BundleDispatchApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<BundleDispatchApiController> logger;
        private IWebHostEnvironment Environment;
        public BundleDispatchApiController(ILogger<BundleDispatchApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }
        public IActionResult Get()
        {
            try
            {
                List<BundleStockBook> lst = new List<BundleStockBook>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<BundleStockBook>(
                          "select iif(BundleStockBook.BundleNo is null ,'',BundleStockBook.BundleNo) as BundleNumber," +
                          "Cast(Sum(Quantity) as double ) as Qty," +
                          "Cast(sum(BundleStockBook.BundleWeight) as double ) as BundleWeight,BundleStockBook.BundleId," +
                          "iif(Items.Name is null , '',Items.Name) as ItemName," +
                          "GSM.Name as GSMName, (BundleSize.Name || ' ' || BundleSize.Unit) as SizeName ," +
                          "Cast(sum(Bundle.NoOfRim)as double)as NoOfRim," +
                          "Cast(sum(Bundle.RimWeight)as double) as RimWeight from BundleStockBook " +
                          " left join Items on BundleStockBook.Quality = Items.Id" +
                          " left join GSM on BundleStockBook.GSM = GSM.Id " +
                          "  left join BundleSize on BundleStockBook.BundleSize = BundleSize.Id " +
                          "  left join Bundle on BundleStockBook.BundleId=Bundle.Id" +
                          "   group by BundleStockBook.BundleNo,BundleStockBook.BundleId," +
                          " BundleStockBook.Quality, BundleStockBook.BF, BundleStockBook.GSM," +
                          " BundleStockBook.BundleSize having Sum(Quantity) > 0").ToList();








                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

    }
}
