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
    public class ReelDispatchApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<ReelDispatchApiController> logger;
        private IWebHostEnvironment Environment;
        public ReelDispatchApiController(ILogger<ReelDispatchApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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
                        where = "  where  StockBook.Godown =" + id ;
                    }
                  
                   
                    lst = conn.Query<StockBook>(
                          "select iif(Stockbook.Reelnumber is null ,'',stockbook.ReelNumber) as ReelNumber,Cast(Sum(Quantity) as double ) as Qty,Cast(sum(NetWeight) as double ) as NetWeight ,Stockbook.Godown, StockBook.SlipId," +
                          "iif(Items.Name is null , '',Items.Name) as ItemName," +
                          " iif(BF.Name is null , '',BF.Name) as BFName,ReelDia.Name as ReelDiaName, " +
                          "GSM.Name as GSMName, (Size.Name || ' ' || Size.Unit) as SizeName from StockBook left join Items on StockBook.Quality = Items.Id " +
                         "left join BF on StockBook.BF = BF.Id left join ReelDia on StockBook.BF = ReelDia.Id  left join GSM on StockBook.GSM = GSM.Id " +
                          " left join Size on StockBook.Size = Size.Id " + where +
                          " and StockBook.VoucherType<>'Cutter' group by Stockbook.reelnumber, Stockbook.Godown,StockBook.SlipId, StockBook.Quality, StockBook.BF, StockBook.GSM, StockBook.Size " +
                          "having Sum(Quantity) > 0").ToList();








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
