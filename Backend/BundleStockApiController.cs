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
    public class BundleStockApiController : Controller
    {

        private IConfiguration Configuration;
        private readonly ILogger<BundleStockApiController> logger;
        private readonly IWebHostEnvironment Environment;

        public BundleStockApiController(ILogger<BundleStockApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }


        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            BundleDispatch lst = new BundleDispatch();
            string ConnString = this.Configuration.GetConnectionString("MyConn");
            using (SQLiteConnection db = new SQLiteConnection(ConnString))
            {
                int Duplicate = db.ExecuteScalar<int>("select Sum(Quantity) from BundleStockbook where BundleNo ='" + id + "'");
                if (Duplicate == 0)
                {
                    System.IO.File.WriteAllText("MyFile/" + id + ".txt", "Already Scanned");
                    return Conflict(new { Message = "Already Scanned" });
                }
                int Exist = db.ExecuteScalar<int>("select Count(*) from Bundle where FormattedNo ='" + id + "'");
                if (Exist == 0)
                {
                    System.IO.File.WriteAllText("MyFile/" + id + ".txt", "Invalid QR Code");
                    return NotFound(new { Message = "Invalid QR Code" });
                }

            }
            //System.IO.File.WriteAllText("MyFile/" + id + ".txt", "Success");
            return Ok(new { Message = "Success" });
        }
        [HttpPut("{Id}")]

        public IActionResult put(int id, BundleDispatch lst)
        {
            try
            {
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            //System.IO.File.WriteAllText("TestingDate.txt", lst.Date.ToString());
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string client = Request.Headers["ClientUSID"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (user.IsEditAllowed == 1)
                            {
                                //lst.VehicleNo = lst.VehicleNo.ToUpper();

                                string json = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
                                //System.IO.File.WriteAllText("MyFile/put" + lst.VehicleNo + ".txt", json);

                                conn.Execute("Delete from BundleStockbook where VoucherId=" + id + " and VoucherType='BundleDispatch'", transaction);

                                conn.Execute("Delete from BundleStock where BundleDispatchId=" + id, transaction);
                                conn.Execute("Update BundleDispatch set Remark=@Remark where Id=" + id, lst, transaction);

                                BundleStockBook BundleStockbook;
                                Bundle Bundle;
                                foreach (BundleStock BundleStock in lst.BundleStock)
                                {

                                    int Duplicate = conn.ExecuteScalar<int>("select Sum(Quantity) from BundleStockbook where BundleNo ='" + BundleStock.BundleNo + "'", transaction);

                                    if (Duplicate == 0)
                                    {
                                        transaction.Rollback();
                                        return Conflict(new { Message = BundleStock.BundleNo + "Already Scanned" });
                                    }
                                    int Exist = conn.ExecuteScalar<int>("select Count(*) from Bundle where FormattedNo ='" + BundleStock.BundleNo + "'", transaction);
                                    if (Exist == 0)
                                    {
                                        transaction.Rollback();
                                        return NotFound(new { Message = BundleStock.BundleNo + "Invalid QR Code" });
                                    }

                                    //BundleStock.Godown = conn.ExecuteScalar<int>("select GodownId from Bundle where FormattedNo ='" + BundleStock.BundleNo + "'", transaction);
                                    BundleStock.BundleDispatchId = id;
                                    BundleStock.Id = conn.ExecuteScalar<int>("select max(Id) from BundleStock") + 1;
                                    //BundleStock.Serial = conn.ExecuteScalar<int>("select max(Serial) from BundleStock") + 1;
                                    BundleStock.Serial = conn.ExecuteScalar<int>("select max(Serial) from BundleStock where BundleDispatchId=" + BundleStock.BundleDispatchId, transaction) + 1;
                                    BundleStock.Status = 1;
                                    BundleStock.BundleId = conn.ExecuteScalar<int>("select Id from Bundle where FormattedNo ='" + BundleStock.BundleNo + "' ", transaction);
                                    //BundleStock.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

                                    //conn.Execute("insert into BundleStock(Id,Date,Status,BundleId,VehicleNo,Number,BundleDispatchId,Serial,Godown)" +
                                    //    " values(@Id,@Date,@Status,@BundleId,@VehicleNo,@Number,@BundleDispatchId,@Serial,@Godown)", BundleStock, transaction);



                                    conn.Execute("INSERT INTO BundleStock(Id, Date, Status, BundleId, VehicleNo, Number, BundleDispatchId, Serial, Godown) " +
             "VALUES (@Id, @Date, @Status, @BundleId, @VehicleNo, @BundleNo, @BundleDispatchId, @Serial, @Godown)",
             new
             {
                 Id = BundleStock.Id,
                 Date = lst.Date,
                 Status = BundleStock.Status,
                 BundleId = BundleStock.BundleId,
                 VehicleNo = BundleStock.VehicleNo,
                 BundleNo = BundleStock.BundleNo,
                 BundleDispatchId = BundleStock.BundleDispatchId,
                 Serial = BundleStock.Serial,
                 Godown = BundleStock.Godown
             }, transaction);



                                    Bundle = conn.Query<Bundle>("select * from Bundle where id= " + BundleStock.BundleId, transaction).FirstOrDefault();
                                    BundleStockbook = new BundleStockBook();
                                    BundleStockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from BundleStockbook", transaction);
                                    //BundleStockbook.Date = BundleStock.Date;

                                    BundleStockbook.VoucherId = lst.Id;
                                    BundleStockbook.BundleId = Bundle.Id;
                                    BundleStockbook.BundleNumber = Bundle.FormattedNo;

                                    BundleStockbook.Quality = Bundle.Quality;

                                    BundleStockbook.GSM = Bundle.GSMId;
                                    BundleStockbook.BundleSize = Bundle.BundleSizeId;
                                    BundleStockbook.Unit = conn.ExecuteScalar<string>("select Unit from  BundleSize left join Bundle on Bundle.BundleSizeId=BundleSize.Id  where Bundle.Id= " + BundleStock.BundleId);
                                    BundleStockbook.BundleWeight = (-1) * Bundle.BundleWeight;
                                    BundleStockbook.Quantity = -1;
                                    BundleStockbook.VoucherType = "BundleDispatch";
                                    //conn.Execute("insert into BundleStockBook(Id,VoucherId,Date,BundleNo,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,BundleId,Shift,ReelDia" +
                                    //     ") values(@Id,@VoucherId,@Date,@BundleNo,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@BundleId,@Shift,@ReelDia)", BundleStockbook, transaction);


                                    conn.Execute("INSERT INTO BundleStockBook(Id, VoucherId, Date, BundleNo, Quality, BF, GSM, BundleSize, Unit, BundleWeight, Quantity, VoucherType, BundleId) " +
                 "VALUES (@Id, @VoucherId, @Date, @BundleNo, @Quality, @BF, @GSM, @BundleSize, @Unit, @BundleWeight, @Quantity, @VoucherType, @BundleId)",
                 new
                 {
                     Id = BundleStockbook.Id,
                     VoucherId = BundleStockbook.VoucherId,
                     Date = lst.Date,
                     BundleNo = BundleStockbook.BundleNumber,
                     Quality = BundleStockbook.Quality,
                     BF = BundleStockbook.BF,
                     GSM = BundleStockbook.GSM,
                     BundleSize = BundleStockbook.BundleSize,
                     Unit = BundleStockbook.Unit,
                     BundleWeight = BundleStockbook.BundleWeight,
                     Quantity = BundleStockbook.Quantity,
                     VoucherType = BundleStockbook.VoucherType,
                     BundleId = BundleStockbook.BundleId                     
                 }, transaction);



                                }
                                transaction.Commit();
                                return Ok("Updated");
                                //return Ok(new { Message = "Updated" });
                            }


                            else
                            {
                                return BadRequest("you are not allowed");
                                // return BadRequest(new { Message = "you are not allowed" });
                            }

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return BadRequest(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

        
    }
}
