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
    public class ReelLocationhApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<ReelLocationhApiController> logger;
        private IWebHostEnvironment Environment;
        public ReelLocationhApiController(ILogger<ReelLocationhApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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
                List<StockBook> lst = new List<StockBook>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {


                    string where = "";
                    if (id != 0)
                    {
                        where = " where GodownVMaster.LocationId =" + id;
                    }


                    //lst = conn.Query<StockBook>(
                    //      "select iif(Stockbook.Reelnumber is null ,'',stockbook.ReelNumber) as ReelNumber," +
                    //      "Cast(Sum(Quantity) as double ) as Qty,Cast(sum(NetWeight) as double ) as NetWeight ," +
                    //      "Stockbook.Godown, StockBook.SlipId," +
                    //      "iif(Items.Name is null , '',Items.Name) as ItemName," +
                    //      " ReelDia.Name as ReelDiaName, " +
                    //      "GSM.Name as GSMName, (Size.Name || ' ' || Size.Unit) as SizeName from StockBook left join Items on StockBook.Quality = Items.Id " +
                    //     "left join BF on StockBook.BF = BF.Id " +
                    //     " LEFT JOIN GodownVMaster ON StockBook.SlipId = GodownVMaster.SlipId " +
                    //     "left join ReelDia on StockBook.ReelDia = ReelDia.Id  left join GSM on StockBook.GSM = GSM.Id " +
                    //      " left join Size on StockBook.Size = Size.Id " + where +
                    //      "  group by Stockbook.reelnumber, Stockbook.Godown,StockBook.SlipId, StockBook.Quality, StockBook.BF, StockBook.GSM, StockBook.Size " +
                    //      "having Sum(Quantity) > 0").ToList();



                    lst = conn.Query<StockBook>("SELECT  IIF(Stockbook.Reelnumber IS NULL, '', Stockbook.Reelnumber) AS ReelNumber," +
                        "   CAST(SUM(Quantity) AS DOUBLE) AS Qty," +
                        " CAST(SUM(NetWeight) AS DOUBLE) AS NetWeight," +
                        " Stockbook.Godown,  StockBook.SlipId," +
                        "  IIF(Items.Name IS NULL, '', Items.Name) AS ItemName," +
                        " ReelDia.Name AS ReelDiaName, " +
                        "GSM.Name AS GSMName, (Size.Name || ' ' || Size.Unit) AS SizeName FROM   StockBook  " +
                        "LEFT JOIN Items ON StockBook.Quality = Items.Id " +
                        "LEFT JOIN BF ON StockBook.BF = BF.Id " +
                        " LEFT JOIN GodownVMaster ON StockBook.SlipId = GodownVMaster.SlipId " +
                        "LEFT JOIN ReelDia ON StockBook.ReelDia = ReelDia.Id  " +
                        "LEFT JOIN GSM ON StockBook.GSM = GSM.Id  " +
                        "LEFT JOIN Size ON StockBook.Size = Size.Id " + where + 
                        "   GROUP BY  Stockbook.Reelnumber,    Stockbook.Godown," +
                        " StockBook.SlipId,Items.Name,   ReelDia.Name,   GSM.Name,  Size.Name,  Size.Unit HAVING  SUM(Quantity) > 0;").ToList();








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
