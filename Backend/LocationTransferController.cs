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
    public class LocationTransferController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<LocationTransferController> logger;
        public LocationTransferController(IConfiguration _configuration, ILogger<LocationTransferController> _logger)
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
                List<LocationTransfer> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<LocationTransfer>("select LocationTransfer.*, GodownLocation.Name as FromLocationName, a.Name as ToLocationName" +
                        "  from LocationTransfer " +
                        "left join GodownLocation on LocationTransfer.FromLocation=GodownLocation.Id " +
                        "left join GodownLocation as a on LocationTransfer.ToLocation=a.Id " +
                        " where LocationTransfer.Date>=" + fdt + " and LocationTransfer.Date<=" + tdt).ToList();
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
                LocationTransfer lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<LocationTransfer>("select * from LocationTransfer where Id=" + Id).FirstOrDefault();
                    //lst.godownmeta = conn.Query<GodownTransferMeta>("select * from GodownTransferMeta left join GodownTransfer on GodownTransferMeta.GodownTransferId=GodownTransfer.Id where GodownTransferId=" + Id).ToList();
                    lst.locationmeta = conn.Query<LocationTransferMeta>("select LocationTransferMeta.* " +
                        "from LocationTransferMeta" +
                        "  where LocationTransferId=" + Id).ToList();
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
        public IActionResult Post(LocationTransfer lst)
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
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (user.LocationTransfer == 1)
                            {
                                lst.Id = conn.ExecuteScalar<int>("select max(id) from LocationTransfer") + 1;
                                lst.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                conn.Execute("insert into LocationTransfer(Id,Date,FromLocation,ToLocation,Remark)Values(@Id,@Date,@FromLocation,@ToLocation,@Remark)", lst, transaction);
                                
                                StockBook stockbook;
                                Slip slip;
                                foreach (LocationTransferMeta stock in lst.locationmeta)
                                {

                                    stock.LocationTransferId = lst.Id;
                                    stock.Id = conn.ExecuteScalar<int>("select max(Id) from LocationTransferMeta") + 1;

                                    conn.Execute("insert into LocationTransferMeta(Id,LocationTransferId,Number,SlipId,VNetWeight,GSMName,SizeName,ReelDiaName,ItemName)" +
                                        " values(@Id,@LocationTransferId,@Number,@SlipId,@VNetWeight,@GSMName,@SizeName,@ReelDiaName,@ItemName)", stock);


                                    slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId).FirstOrDefault();
                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    stockbook.ReelDia = slip.ReelDia;
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId);
                                    stockbook.Godown = lst.FromLocation;
                                    stockbook.NetWeight = -1 * slip.NetWeight;
                                    stockbook.Quantity = -1;
                                    stockbook.VoucherType = "LocationTransfer";
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,ReelDia,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@ReelDia,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);


                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    stockbook.ReelDia = slip.ReelDia;
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId);
                                    stockbook.Godown = lst.ToLocation;
                                    stockbook.NetWeight = slip.NetWeight;
                                    stockbook.Quantity = 1;
                                    stockbook.VoucherType = "LocationTransfer";
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,ReelDia,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@ReelDia,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);
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
                        catch (Exception ex)
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

        public IActionResult Put(int id, LocationTransfer lst)
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
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (user.IsEditAllowed == 1)
                            {
                                conn.Execute("Delete from LocationTransferMeta where LocationTransferId=" + id, transaction);
                                conn.Execute("Update LocationTransfer set Date=@Date,FromLocation=@FromLocation,ToLocation=@ToLocation,Date=@Date,Remark=@Remark where Id=" + id, lst, transaction);

                                
                                StockBook stockbook;
                                Slip slip;
                                foreach (LocationTransferMeta stock in lst.locationmeta)
                                {
                                    stock.LocationTransferId = lst.Id;
                                    stock.Id = conn.ExecuteScalar<int>("select max(Id) from LocationTransferMeta") + 1;

                                    conn.Execute("insert into LocationTransferMeta(Id,LocationTransferId,Number,SlipId,VNetWeight,GSMName,SizeName,ReelDiaName,ItemName)" +
                                        " values(@Id,@LocationTransferId,@Number,@SlipId,@VNetWeight,@GSMName,@SizeName,@ReelDiaName,@ItemName)", stock);

                                    slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId).FirstOrDefault();
                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    stockbook.ReelDia = slip.ReelDia;
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId);
                                    stockbook.Godown = lst.FromLocation;
                                    stockbook.NetWeight = -1 * slip.NetWeight;
                                    stockbook.Quantity = -1;
                                    stockbook.VoucherType = "LocationTransfer";
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,ReelDia,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@ReelDia,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);


                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    stockbook.ReelDia = slip.ReelDia;
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId);
                                    stockbook.Godown = lst.ToLocation;
                                    stockbook.NetWeight = slip.NetWeight;
                                    stockbook.Quantity = 1;
                                    stockbook.VoucherType = "LocationTransfer";
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,ReelDia,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@ReelDia,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);
                                }
                                transaction.Commit();
                                return Ok("Updated");
                                //return Ok(new { Message = "Updated" });
                            }
                            else
                            {
                                return BadRequest("you are not allowed");
                                //return BadRequest(new { Message = "you are not allowed" });
                            }

                        }
                        catch (Exception ex)
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
                        try
                        {
                            conn.Execute("Delete from LocationTransfer where Id=" + id, transaction);
                            conn.Execute("Delete from LocationTransferMeta where LocationTransferId=" + id, transaction);
                            transaction.Commit();
                            return Ok("Deleted");
                            //return Ok(new { Message = "Deleted" });
                        }
                        catch (Exception ex)
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


