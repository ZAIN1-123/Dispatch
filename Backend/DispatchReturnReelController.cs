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
    public class DispatchReturnReelController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<DispatchReturnReelController> logger;
        private IWebHostEnvironment Environment;
        public DispatchReturnReelController(ILogger<DispatchReturnReelController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }
        [HttpGet("{id}")]

        public IActionResult Get(int id)
        {
            try
            {

                string where = "";
                List<StockBook> lst = new List<StockBook>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    if (id != 0)
                    {
                        where = " where Stockbook.Godown =" + id ;
                    }

                    //lst = conn.Query<Stock>("select slip.Date as Date, slip.Id as slipId,Slip.GodownId as Godown, slip.FormattedNo as ReelNumber,slip.Date as SlipDate,slip.NetWeight as VNetWeight, Items.Name as ItemName, " +
                    //                          "BF.Name as BFName, GSM.Name as GSM, Users.UserName as EnteredName, " +
                    //                          "(Size.Name || ' ' || Size.Unit) as Size, -1 as Quantity" +
                    //                          " from stock left join slip on Stock.SlipId = slip.Id " +
                    //                          "left join Items on Slip.Quality = Items.Id " +
                    //                          "left join BF on Slip.BF = BF.Id " +
                    //                          "left join Users on Slip.EntredBy = Users.Id " +
                    //                          "left join GSM on Slip.GSMId = GSM.Id " +
                    //                          "left join Size on Slip.SizeId = Size.Id "+where
                    //                          ).ToList();
                    //lst = conn.Query<StockBook>(
                    //     "select Stockbook.reelnumber,  ABS(Sum(Quantity)) as Qty,ABS(StockBook.NetWeight) as NetWeight, Stockbook.Godown,StockBook.SlipId,Items.Name as ItemName,ReelDia.Name as ReelDiaName,BF.Name as BFName, " +
                    //     "GSM.Name as GSMName, (Size.Name || ' ' || Size.Unit) as SizeName from StockBook  left join Items on StockBook.Quality = Items.Id " +
                    //     "left join BF on StockBook.BF = BF.Id  left join ReelDia on StockBook.BF = ReelDia.Id left join GSM on StockBook.GSM = GSM.Id " +
                    //     " left join Size on StockBook.Size = Size.Id " + where +
                    //     "  group by Stockbook.reelnumber,Stockbook.Godown, StockBook.SlipId, StockBook.Quality, StockBook.BF, StockBook.GSM, StockBook.Size " +
                    //     "having Sum(Quantity) <= 0").ToList();

                    lst = conn.Query<StockBook>(
    "SELECT Stockbook.reelnumber, ABS(Sum(Quantity)) as Qty, ABS(CAST(StockBook.NetWeight AS double)) as NetWeight, Stockbook.Godown, StockBook.SlipId, Items.Name as ItemName, ReelDia.Name as ReelDiaName, BF.Name as BFName, " +
    "GSM.Name as GSMName, (Size.Name || ' ' || Size.Unit) as SizeName FROM StockBook " +
    "LEFT JOIN Items on StockBook.Quality = Items.Id " +
    "LEFT JOIN BF on StockBook.BF = BF.Id " +
    "LEFT JOIN ReelDia on StockBook.ReelDia = ReelDia.Id " +
    "LEFT JOIN GSM on StockBook.GSM = GSM.Id " +
    "LEFT JOIN Size on StockBook.Size = Size.Id " + where +
    " GROUP BY Stockbook.reelnumber, Stockbook.Godown, StockBook.SlipId, StockBook.Quality, StockBook.BF, StockBook.GSM, StockBook.Size " +
    "HAVING Sum(Quantity) <= 0"
).ToList();


                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
               // return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

    }
}
