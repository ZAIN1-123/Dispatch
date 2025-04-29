using Dapper;
using DISPATCHAPI.Models;
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

    public class ReelDispatchreturnController : Controller
    {

        private IConfiguration Configuration;
        private readonly ILogger<ReelDispatchreturnController> logger;
        private readonly IWebHostEnvironment Environment;

        public ReelDispatchreturnController(ILogger<ReelDispatchreturnController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            DispatchReturn lst = new DispatchReturn();
            string ConnString = this.Configuration.GetConnectionString("MyConn");
            using (SQLiteConnection db = new SQLiteConnection(ConnString))
            {
                int Duplicate = db.ExecuteScalar<int>("select Count(*) from DispatchReturnMeta where Number ='" + id + "'");
                if (Duplicate > 0)
                {
                    return Conflict(new { Message = "Already Scanned" });
                }
                int Exist = db.ExecuteScalar<int>("select Count(*) from Stock where Number ='" + id + "'");
                if (Exist == 0)
                {
                    return NotFound(new { Message = "Invalid QR Code" });
                }

            }
            return Ok(new { Message = "Success" });
        }
        [HttpPut("{Id}")]

        public IActionResult put(int id, DispatchReturn lst)

        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string client = Request.Headers["ClientUSID"].FirstOrDefault();
                        string auth = Request.Headers["auth"].FirstOrDefault();
                        string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                        string[] credentials = decodedStrings.Split(':');
                        string username = credentials[0];
                        string password = credentials[1];
                        User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                        if (user.IsEditAllowed == 1)
                        {
                            conn.Execute("Delete from Stockbook where VoucherId=" + id + " and VoucherType='DispatchReturn'");

                            conn.Execute("Delete from DispatchReturnMeta where DispatchId=" + id);
                            conn.Execute("Update DispatchReturn set VehicleNo=@VehicleNo,Remark=@Remark where Id=" + id, lst);

                            foreach (DispatchReturnMeta stock in lst.stock)
                            {
                                int Duplicate = conn.ExecuteScalar<int>("select Count(*) from DispatchReturnMeta where Number ='" + stock.Number + "'");
                                if (Duplicate > 0)
                                {
                                    return Conflict(new { Message = "Already Scanned" });
                                }
                                int Exist = conn.ExecuteScalar<int>("select Count(*) from Stock where Number ='" + stock.Number + "'");
                                if (Exist == 0)
                                {
                                    return NotFound(new { Message = "Invalid QR Code" });
                                }
                                stock.Godown = conn.ExecuteScalar<int>("select GodownId from Slip where FormattedNo ='" + stock.Number + "'");
                                stock.DispatchId = id;
                                stock.Id = conn.ExecuteScalar<int>("select max(Id) from DispatchReturnMeta") + 1;
                                stock.Serial = conn.ExecuteScalar<int>("select max(Serial) from DispatchReturnMeta") + 1;
                                stock.Status = 1;
                                stock.SlipId = conn.ExecuteScalar<int>("select Id from Slip where FormattedNo ='" + stock.Number + "' ");
                                stock.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                conn.Execute("insert into DispatchReturnMeta(Id,Date,Status,SlipId,VehicleNo,Number,DispatchId,Serial,Godown)" +
                                    " values(@Id,@Date,@Status,@SlipId,@VehicleNo,@Number,@DispatchId,@Serial,@Godown)", stock);
                                StockBook stockbook = new StockBook();
                                stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                stockbook.Date = stock.Date;
                                stockbook.VoucherId = stock.DispatchId;
                                stockbook.SlipId = stock.SlipId;
                                stockbook.ReelNumber = stock.Number;
                                stockbook.Quality = conn.ExecuteScalar<int>("select Items.Id from  Items left join Slip on Slip.Quality=Items.Id  where Slip.Id= " + stock.SlipId);
                                if (client.Contains("BHAGESHWARIDISPATCH"))
                                {
                                    stockbook.Shift = conn.ExecuteScalar<string>("select Shift from Slip where Slip.Id=  " + stock.SlipId);

                                    stockbook.ReelDia = conn.ExecuteScalar<int>("select ReelDia.Id from  ReelDia left join Slip on Slip.ReelDia=ReelDia.Id  where Slip.Id=  " + stock.SlipId);

                                }
                                else
                                {
                                    stockbook.BF = conn.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);

                                }
                                stockbook.GSM = conn.ExecuteScalar<int>("select GSM.Id from  GSM left join Slip on Slip.GSMId=GSM.Id  where Slip.Id=  " + stock.SlipId);
                                stockbook.Size = conn.ExecuteScalar<int>("select Size.Id from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id=  " + stock.SlipId);
                                stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id= " + stock.SlipId);
                                stockbook.NetWeight = (conn.ExecuteScalar<decimal>("select NetWeight from  Slip where Id= " + stock.SlipId)); //(-)
                                stockbook.Godown = stock.Godown;
                                stockbook.Quantity = 1;
                                stockbook.VoucherType = "DispatchReturn";

                                conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia" +
                                      ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia)", stockbook);
                            }
                            transaction.Commit();
                            return Ok("Updated");
                           // return Ok(new { Message = "Updated" });
                        }
                        else
                        {
                            return BadRequest("you are not allowed");
                            //return BadRequest(new { Message = "you are not allowed" });
                        }
                    }

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