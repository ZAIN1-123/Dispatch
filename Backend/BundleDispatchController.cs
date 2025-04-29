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

    public class BundleDispatchController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<BundleDispatchController> logger;
        private IWebHostEnvironment Environment;
        public BundleDispatchController(ILogger<BundleDispatchController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }


        [HttpGet]
        public ActionResult Getch(int Fdate, int Tdate, string vehicle, int BundleDispatchno)
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
                if (vehicle != null)
                {
                    where += "  and (BundleDispatch.VehicleNo ='" + vehicle + "' ) ";
                }
                if (BundleDispatchno != 0)
                {
                    where += "  and (BundleDispatch.VNumber =" + BundleDispatchno + " ) ";
                }
                List<BundleDispatch> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {


                    lst = conn.Query<BundleDispatch>("select BundleDispatch.* , Count(BundleStock.Serial) as Serial , Cast(sum(Bundle.BundleWeight) as double ) as BundleWeight " +
                        "from BundleDispatch left join BundleStock on BundleStock.BundleDispatchId= BundleDispatch.Id" +
                         " left join Bundle on  BundleStock.BundleId=Bundle.Id " +
                        " where BundleDispatch.Date>=" + fdt + " and BundleDispatch.Date<=" + tdt + where + " group by BundleDispatch.Id").ToList();

                    return Ok(lst);
                }
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });

            }
        }


        [HttpGet("{Id}")]

        public IActionResult Get(int id)
        {
            try
            {
                BundleDispatch lst = new BundleDispatch();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<BundleDispatch>("select BundleDispatch.* from BundleDispatch where Id=" + id).FirstOrDefault();
                    lst.BundleStock = conn.Query<BundleStock>("select BundleStock.*," +
                        " Items.Name as ItemName, GSM.Name as GSMName, (BundleSize.Name|| ' ' || BundleSize.Unit) as BundleSizeName,Bundle.Id as BundleId, Bundle.BundleWeight as VBundleWeight " +
                        "from BundleStock" +
                        " left join Bundle on BundleStock.BundleId=Bundle.Id" +
                        " left join Items on Bundle.Quality = Items.Id " +                                             
                        "left join GSM on Bundle.GSMId = GSM.Id " +
                        "left join BundleSize on Bundle.BundleSizeId=BundleSize.Id" +
                        "  where BundleDispatchId=" + id).ToList();
                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(BundleDispatch lst)
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
                            string client = Request.Headers["ClientUSID"].FirstOrDefault();
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();

                            lst.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            int FYID = conn.ExecuteScalar<int>("select Id from FinancialYear where StartDate<=" + lst.Date + " and EndDate>=" + lst.Date);
                            lst.FYID = FYID;
                            lst.CreateDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            lst.CreateTime = int.Parse(DateTime.Now.ToString("HHmmss"));
                            lst.Enteredby = user.Id;
                            lst.Id = conn.ExecuteScalar<int>("select max(Id) from BundleDispatch") + 1;
                            lst.VNumber = conn.ExecuteScalar<int>("select max(VNumber) from BundleDispatch where FYID=@FYID", new { FYID = FYID }, transaction) + 1;
                            lst.Status = 0;
                            conn.Execute("insert into BundleDispatch(Id,Date,VNumber,VehicleNo,CreateDate,CreateTime,Enteredby,Remark,Status,FYID)" +
                                "values(@Id,@Date,@VNumber,@VehicleNo,@CreateDate,@CreateTime,@Enteredby,@Remark,@Status,@FYID)", lst, transaction);
                            BundleStockBook BundleStockbook;
                            Bundle Bundle;

                            lst.VehicleNo = lst.VehicleNo.ToUpper();                           

                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
                            System.IO.File.WriteAllText("MyFile/post" + lst.VehicleNo + ".txt", json);

                            foreach (BundleStock BundleStock in lst.BundleStock)
                            {
                                int Duplicate = conn.ExecuteScalar<int>("select Sum(Quantity) from BundleStockbook where BundleNo ='" + BundleStock.BundleNo + "'", transaction);

                                if (Duplicate == 0)
                                {
                                    transaction.Rollback();
                                    return Conflict(BundleStock.Number + " Already Scanned");
                                    //return Conflict(new { Message = BundleStock.Number + " Already Scanned" });
                                }
                                int Exist = conn.ExecuteScalar<int>("select Count(*) from Bundle where FormattedNo ='" + BundleStock.BundleNo + "'", transaction);

                                if (Exist == 0)
                                {
                                    transaction.Rollback();
                                    return NotFound(BundleStock.Number + "Invalid QR Code");
                                    //return NotFound(new { Message = BundleStock.Number + "Invalid QR Code" });
                                }
                                
                                BundleStock.BundleDispatchId = lst.Id;
                                BundleStock.Id = conn.ExecuteScalar<int>("select max(Id) from BundleStock") + 1;
                                BundleStock.Serial = conn.ExecuteScalar<int>("select max(Serial) from BundleStock where BundleDispatchId=" + BundleStock.BundleDispatchId, transaction) + 1;
                                BundleStock.Status = 1;
                                BundleStock.BundleId = conn.ExecuteScalar<int>("select Id from Bundle where FormattedNo ='" + BundleStock.BundleNo + "' ", transaction);
                                BundleStock.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                BundleStock.VehicleNo = lst.VehicleNo;
                                conn.Execute("insert into BundleStock(Id,Date,Status,BundleId,VehicleNo,Number,BundleDispatchId,Serial)" +
                                    " values(@Id,@Date,@Status,@BundleId,@VehicleNo,@BundleNo,@BundleDispatchId,@Serial)", BundleStock, transaction);

                                BundleStockbook = new BundleStockBook();
                                Bundle = conn.Query<Bundle>("select * from Bundle where id= " + BundleStock.BundleId, transaction).FirstOrDefault();
                                BundleStockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from BundleStockbook");
                                BundleStockbook.Date = BundleStock.Date;
                                BundleStockbook.VoucherId = lst.Id;
                                BundleStockbook.BundleId = Bundle.Id;
                                BundleStockbook.BundleNo = Bundle.FormattedNo;
                                BundleStockbook.Quality = Bundle.Quality;                          

                                BundleStockbook.GSM = Bundle.GSMId;
                                BundleStockbook.BundleSize = Bundle.BundleSizeId;
                                BundleStockbook.Unit = conn.ExecuteScalar<string>("select Unit from  BundleSize where Id= " + Bundle.BundleSizeId, transaction);
                                BundleStockbook.BundleWeight = (-1) * Bundle.BundleWeight;                               
                                BundleStockbook.Quantity = -1;
                                BundleStockbook.VoucherType = "BundleDispatch";                                
                                conn.Execute("insert into BundleStockBook(Id,VoucherId,Date,BundleNo,Quality,GSM,BundleSize,Unit,BundleWeight,Quantity,VoucherType,BundleId " +
                                ") values(@Id,@VoucherId,@Date,@BundleNo,@Quality,@GSM,@BundleSize,@Unit,@BundleWeight,@Quantity,@VoucherType,@BundleId)", BundleStockbook, transaction);

                            }
                            transaction.Commit();
                            return Ok("Created");
                            //  return Ok(new { Message = "Created" });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            //return BadRequest(ex.Message);
                            return BadRequest(new { Message = "Failed" + ex.Message });
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

                // return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }


        }

        [HttpDelete("{Id}")]
        public IActionResult delete(int id)

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
                            if (user.IsDeleteAllowed == 1)
                            {
                                conn.Execute("Delete from BundleStockbook where VoucherId=" + id + " and VoucherType='BundleDispatch'", transaction);
                                conn.Execute("Delete from BundleDispatch where Id=" + id, transaction);
                                conn.Execute("Delete from BundleStock where BundleDispatchId=" + id, transaction);
                                transaction.Commit();
                                return Ok("Deleted");
                                //return Ok(new { Message = "Deleted" });
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
                            return Ok("failed");
                            // return BadRequest(new { Message = "Failed" + ex.Message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string a = "constraint failed\r\nFOREIGN KEY constraint failed";
                if (ex.Message == a)
                {
                    return BadRequest("this is not deleted");
                    //return BadRequest(new { Message = "this is not deleted" });
                }
                else
                {
                    return BadRequest(ex.Message);
                    //return BadRequest(new { Message = "Failed" + ex.Message });
                }
            }
        }



    }
}






