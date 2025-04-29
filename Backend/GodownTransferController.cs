using Dapper;
using DISPATCHAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class GodownTransferController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<GodownTransferController> logger;
        public GodownTransferController(IConfiguration _configuration, ILogger<GodownTransferController> _logger)
        {
            Configuration = _configuration;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Getch(int Fdate, int Tdate)
        {
            try
            {
                string where = "";
                int fdt = 0, tdt = 0;
                if (Fdate != 0 && Tdate != 0)
                {
                    fdt = Fdate;
                    tdt = Tdate;
                }
                else
                {
                    fdt = (int.Parse(DateTime.Now.ToString("yyyyMMdd")));
                    tdt = (int.Parse(DateTime.Now.ToString("yyyyMMdd")));
                }
                List<GodownTransfer> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<GodownTransfer>("select GodownTransfer.*, Godown.Name as FromGodownName, a.Name as ToGodownName" +
                        "  from GodownTransfer " +
                        "left join Godown on GodownTransfer.FromGodown=Godown.Id " +
                        "left join Godown as a on GodownTransfer.ToGodown=a.Id " +
                        " where GodownTransfer.Date>=" + fdt + " and GodownTransfer.Date<=" + tdt ).ToList();
                    return Ok(lst);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }
        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            try
            {
                GodownTransfer lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<GodownTransfer>("select * from GodownTransfer where Id=" + Id).FirstOrDefault();
                    //lst.godownmeta = conn.Query<GodownTransferMeta>("select * from GodownTransferMeta left join GodownTransfer on GodownTransferMeta.GodownTransferId=GodownTransfer.Id where GodownTransferId=" + Id).ToList();
                    lst.godownmeta = conn.Query<GodownTransferMeta>("select GodownTransferMeta.* " +
                        "from GodownTransferMeta" +
                        "  where GodownTransferId=" + Id).ToList();
                    return Ok(lst);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
               // return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }
        [HttpPost]
        public IActionResult Post(GodownTransfer lst)
        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try {
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'",transaction).FirstOrDefault();
                            if (user.GodownTransfer == 1)
                            {
                                lst.Id = conn.ExecuteScalar<int>("select max(id) from GodownTransfer", transaction) + 1;
                                lst.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                conn.Execute("insert into GodownTransfer(Id,Date,FromGodown,ToGodown,Remark)Values(@Id,@Date,@FromGodown,@ToGodown,@Remark)", lst,transaction);
                                //conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType" +
                                //   ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType)", lst);



                                //foreach (GodownTransferMeta stock in lst.godownmeta)
                                //{

                                //    stock.GodownTransferId = lst.Id;
                                //    stock.Id = conn.ExecuteScalar<int>("select max(Id) from GodownTransferMeta") + 1;
                                //    //stock.SlipId = conn.ExecuteScalar<int>("select Id from Slip where Slip.FormattedNo ='" + stock.Number +" ' ");

                                //    conn.Execute("insert into GodownTransferMeta(Id,GodownTransferId,Number,SlipId,VNetWeight,GSMName,SizeName,BFName,ItemName,ReelDia)" +
                                //        " values(@Id,@GodownTransferId,@Number,@SlipId,@VNetWeight,@GSMName,@SizeName,@BFName,@ItemName,@ReelDia)", stock, transaction);
                                //    conn.Execute("update StockBook set Godown=" + lst.ToGodown + " where SlipId=" + stock.SlipId, transaction);

                                //}
                                StockBook stockbook;
                                Slip slip;
                                foreach (GodownTransferMeta stock in lst.godownmeta)
                                {

                                    stock.GodownTransferId = lst.Id;
                                    stock.Id = conn.ExecuteScalar<int>("select max(Id) from GodownTransferMeta") + 1;

                                    conn.Execute("insert into GodownTransferMeta(Id,GodownTransferId,Number,SlipId,VNetWeight,GSMName,SizeName,BFName,ItemName)" +
                                        " values(@Id,@GodownTransferId,@Number,@SlipId,@VNetWeight,@GSMName,@SizeName,@BFName,@ItemName)", stock,transaction);


                                    slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId, transaction).FirstOrDefault();
                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook", transaction);
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    stockbook.BF = slip.BF;
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId, transaction);
                                    stockbook.Godown = lst.FromGodown;
                                    stockbook.NetWeight = -1 * slip.NetWeight;
                                    stockbook.Quantity = -1;
                                    stockbook.VoucherType = "GodownTransfer";
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook,transaction);


                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook", transaction);
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    stockbook.BF = slip.BF;
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId);
                                    stockbook.Godown = lst.ToGodown;
                                    stockbook.NetWeight = slip.NetWeight;
                                    stockbook.Quantity = 1;
                                    stockbook.VoucherType = "GodownTransfer";
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);
                                }
                                transaction.Commit();
                              return Ok("Created");
                                //return Ok(new { Message = "Created" });
                            }
                            else
                            {
                                return BadRequest("you are not allowed");
                                //return BadRequest(new { Message = "you are not allowed" });
                            }

                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                            return Ok("failed");
                            //return BadRequest(new { Message = "Failed" + ex.Message });
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

        [HttpPut("{Id}")]

        public IActionResult Put(int id, GodownTransfer lst)
        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try {
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (user.IsEditAllowed == 1)
                            {
                                conn.Execute("Delete from GodownTransferMeta where GodownTransferId=" + id,transaction);
                                conn.Execute("Update GodownTransfer set Date=@Date,FromGodown=@FromGodown,ToGodown=@ToGodown,Date=@Date,Remark=@Remark where Id=" + id, lst, transaction);

                                //foreach (GodownTransferMeta stock in lst.godownmeta)
                                //{
                                //    stock.GodownTransferId = lst.Id;
                                //    if (stock.Id == 0)
                                //    {
                                //        stock.Id = conn.ExecuteScalar<int>("select max(Id) from GodownTransferMeta") + 1;
                                //    }
                                //    //stock.SlipId = conn.ExecuteScalar<int>("select Id from slip where Slip.FormattedNo='" + stock.Number + " ' ");
                                //    //conn.Execute("insert into GodownTransferMeta(Id,GodownTransferId,Number,SlipId)" +
                                //    //    " values(@Id,@GodownTransferId,@Number,@SlipId)", stock);
                                //    conn.Execute("insert into GodownTransferMeta(Id,GodownTransferId,Number,SlipId,VNetWeight,GSMName,SizeName,BFName,ItemName,ReelDia)" +
                                //        " values(@Id,@GodownTransferId,@Number,@SlipId,@VNetWeight,@GSMName,@SizeName,@BFName,@ItemName,@ReelDia)", stock, transaction);
                                //    conn.Execute("update StockBook set Godown=" + lst.ToGodown + " where SlipId=" + stock.SlipId, transaction);

                                //}
                                StockBook stockbook;
                                Slip slip;
                                foreach (GodownTransferMeta stock in lst.godownmeta)
                                {
                                    stock.GodownTransferId = lst.Id;
                                    stock.Id = conn.ExecuteScalar<int>("select max(Id) from GodownTransferMeta") + 1;

                                    conn.Execute("insert into GodownTransferMeta(Id,GodownTransferId,Number,SlipId,VNetWeight,GSMName,SizeName,BFName,ItemName)" +
                                        " values(@Id,@GodownTransferId,@Number,@SlipId,@VNetWeight,@GSMName,@SizeName,@BFName,@ItemName)", stock);

                                    slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId).FirstOrDefault();
                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    stockbook.BF = slip.BF;
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId);
                                    stockbook.Godown = lst.FromGodown;
                                    stockbook.NetWeight = -1 * slip.NetWeight;
                                    stockbook.Quantity = -1;
                                    stockbook.VoucherType = "GodownTransfer";
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);


                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    stockbook.BF = slip.BF;
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId);
                                    stockbook.Godown = lst.ToGodown;
                                    stockbook.NetWeight = slip.NetWeight;
                                    stockbook.Quantity = 1;
                                    stockbook.VoucherType = "GodownTransfer";
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);
                                }
                               return Ok("Updated");
                                //return Ok(new { Message = "Updated" });
                            }
                            else
                            {
                                return BadRequest("you are not allowed");
                                //return BadRequest(new { Message = "you are not allowed" });
                            }

                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                            return Ok("failed");
                            //return BadRequest(new { Message = "Failed" + ex.Message });
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


        [HttpDelete("{Id}")]

        public IActionResult Delete(int id)
        {
            try
            {
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try {
                            conn.Execute("Delete from GodownTransfer where Id=" + id, transaction);
                            conn.Execute("Delete from GodownTransferMeta where GodownTransferId=" + id, transaction);
                            transaction.Commit();
                            return Ok("Deleted");
                            //return Ok(new { Message = "Deleted" });
                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                            return Ok("failed");
                            //return BadRequest(new { Message = "Failed" + ex.Message });
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

 
