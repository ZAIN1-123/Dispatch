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

    public class DispatchApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<DispatchApiController> logger;
        private IWebHostEnvironment Environment;
        public DispatchApiController(ILogger<DispatchApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }


        [HttpGet]
        public ActionResult Getch(int Fdate, int Tdate, string vehicle, int Dispatchno)
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
                    where += "  and (Dispatch.VehicleNo ='" + vehicle + "' ) ";
                }
                if (Dispatchno != 0)
                {
                    where += "  and (Dispatch.VNumber =" + Dispatchno + " ) ";
                }
                List<Dispatch> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {


                    lst = conn.Query<Dispatch>("select Dispatch.* , Count(Stock.Serial) as Serial , Cast(sum(Slip.NetWeight) as double ) as NetWeight " +
                        "from Dispatch left join Stock on Stock.DispatchId= Dispatch.Id" +
                         " left join Slip on  Stock.SlipId=Slip.Id " +
                        " where Dispatch.Date>=" + fdt + " and Dispatch.Date<=" + tdt + where + " group by Dispatch.Id").ToList();

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
                Dispatch lst = new Dispatch();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<Dispatch>("select Dispatch.* from Dispatch where Id=" + id).FirstOrDefault();
                    lst.stock = conn.Query<Stock>("select Stock.*, Godown.Name as GodownName , ReelDia.Name as ReelDiaName," +
                        " Items.Name as ItemName, BF.Name as BFName, GSM.Name as GSMName ,Users.UserName as EnteredName , (Size.Name|| ' ' || Size.Unit) as SizeName,Slip.Id as SlipId, Slip.NetWeight as VNetWeight " +
                        "from Stock" +
                        " left join slip on Stock.SlipId=Slip.Id" +
                        " left join Items on Slip.Quality = Items.Id " +
                        " left join ReelDia on Slip.ReelDia = ReelDia.Id " +
                        "left join BF on Slip.BF = BF.Id " +
                        "left join Users on Slip.EntredBy = Users.Id " +
                        "left join GSM on Slip.GSMId = GSM.Id " +
                        "left join Size on Slip.SizeId=Size.Id" +
                        " left join Godown on Stock.Godown=Godown.Id where DispatchId=" + id).ToList();
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
        public IActionResult Post(Dispatch lst)
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
                            lst.Id = conn.ExecuteScalar<int>("select max(Id) from Dispatch") + 1;
                            lst.VNumber = conn.ExecuteScalar<int>("select max(VNumber) from Dispatch where FYID=@FYID", new { FYID = FYID }, transaction) + 1;
                            lst.Status = 0;
                            conn.Execute("insert into Dispatch(Id,Date,VNumber,VehicleNo,CreateDate,CreateTime,Enteredby,Remark,Status,FYID)" +
                                "values(@Id,@Date,@VNumber,@VehicleNo,@CreateDate,@CreateTime,@Enteredby,@Remark,@Status,@FYID)", lst, transaction);
                            StockBook stockbook;
                            Slip slip;

                            lst.VehicleNo = lst.VehicleNo.ToUpper();

                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
                           //System.IO.File.WriteAllText("MyFile/post" + lst.VehicleNo + ".txt", json);

                            foreach (Stock stock in lst.stock)
                            {
                                int Duplicate = conn.ExecuteScalar<int>("select Sum(Quantity) from Stockbook where ReelNumber ='" + stock.Number + "'",transaction);

                                if (Duplicate == 0)
                                {
                                    transaction.Rollback();
                                    return Conflict(stock.Number + " Already Scanned");
                                    //return Conflict(new { Message = stock.Number + " Already Scanned" });
                                }
                                int Exist = conn.ExecuteScalar<int>("select Count(*) from Slip where FormattedNo ='" + stock.Number + "'", transaction);

                                if (Exist == 0)
                                {
                                    transaction.Rollback();
                                    return NotFound(stock.Number + "Invalid QR Code");
                                    //return NotFound(new { Message = stock.Number + "Invalid QR Code" });
                                }

                                stock.Godown = conn.ExecuteScalar<int>("select GodownId from Slip where FormattedNo ='" + stock.Number + "'", transaction);
                                stock.DispatchId = lst.Id;
                                stock.Id = conn.ExecuteScalar<int>("select max(Id) from Stock") + 1;
                                stock.Serial = conn.ExecuteScalar<int>("select max(Serial) from Stock where DispatchId=" + stock.DispatchId,transaction) + 1;
                                stock.Status = 1;
                                stock.SlipId = conn.ExecuteScalar<int>("select Id from Slip where FormattedNo ='" + stock.Number + "' ", transaction);
                                stock.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                conn.Execute("insert into Stock(Id,Date,Status,SlipId,VehicleNo,Number,DispatchId,Serial,Godown)" +
                                    " values(@Id,@Date,@Status,@SlipId,@VehicleNo,@Number,@DispatchId,@Serial,@Godown)", stock, transaction);

                                stockbook = new StockBook();
                                slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId, transaction).FirstOrDefault();
                                stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                stockbook.Date = stock.Date;
                                stockbook.VoucherId = lst.Id;
                                stockbook.SlipId = slip.Id;
                                stockbook.ReelNumber = slip.FormattedNo;
                                stockbook.Quality = slip.Quality;
                                if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                                {
                                    stockbook.Shift = conn.ExecuteScalar<string>("select Shift from Slip where Slip.Id=  " + stock.SlipId);

                                    stockbook.ReelDia = conn.ExecuteScalar<int>("select ReelDia.Id from  ReelDia left join Slip on Slip.ReelDia=ReelDia.Id  where Slip.Id=  " + stock.SlipId);

                                }
                                else
                                {
                                    stockbook.BF = slip.BF;


                                }


                                stockbook.GSM = slip.GSMId;
                                stockbook.Size = slip.SizeId;
                                stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId, transaction);
                                stockbook.NetWeight = (-1) * slip.NetWeight;
                                stockbook.Godown = stock.Godown;
                                stockbook.Quantity = -1;
                                stockbook.VoucherType = "Dispatch";
                                stockbook.Cutter = -1;
                                conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia,Cutter" +
                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia,@Cutter)", stockbook, transaction);

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

        public IActionResult put(int id, Dispatch lst)
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

                            System.IO.File.WriteAllText("TestingDate.txt",lst.Date.ToString());

                            string client = Request.Headers["ClientUSID"].FirstOrDefault();
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];

                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (user.IsEditAllowed == 1)
                            {
                                lst.VehicleNo = lst.VehicleNo.ToUpper();

                                string json = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
                                System.IO.File.WriteAllText("MyFile/put" + lst.VehicleNo + ".txt", json);

                                conn.Execute("Delete from Stockbook where VoucherId=" + id + " and VoucherType='Dispatch'", transaction);

                                conn.Execute("Delete from Stock where DispatchId=" + id, transaction);
                                conn.Execute("Update Dispatch set Date=@Date,VehicleNo=@VehicleNo,Remark=@Remark where Id=" + id, lst, transaction);

                                StockBook stockbook;
                                Slip slip;
                                foreach (Stock stock in lst.stock)
                                {

                                    int Duplicate = conn.ExecuteScalar<int>("select Sum(Quantity) from Stockbook where ReelNumber ='" + stock.Number + "'", transaction);

                                    if (Duplicate == 0)
                                    {
                                        transaction.Rollback();
                                        return Conflict(new { Message = stock.Number + "Already Scanned" });
                                    }
                                    int Exist = conn.ExecuteScalar<int>("select Count(*) from Slip where FormattedNo ='" + stock.Number + "'", transaction);
                                    if (Exist == 0)
                                    {
                                        transaction.Rollback();
                                        return NotFound(new { Message = stock.Number + "Invalid QR Code" });
                                    }

                                    stock.Godown = conn.ExecuteScalar<int>("select GodownId from Slip where FormattedNo ='" + stock.Number + "'", transaction);
                                    stock.DispatchId = id;
                                    stock.Id = conn.ExecuteScalar<int>("select max(Id) from Stock") + 1;
                                    //stock.Serial = conn.ExecuteScalar<int>("select max(Serial) from Stock") + 1;
                                    stock.Serial = conn.ExecuteScalar<int>("select max(Serial) from Stock where DispatchId=" + stock.DispatchId, transaction) + 1;
                                    stock.Status = 1;
                                    stock.SlipId = conn.ExecuteScalar<int>("select Id from Slip where FormattedNo ='" + stock.Number + "' and NetWeight=" + stock.VNetWeight,transaction);
                                    //stock.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

                                    //conn.Execute("insert into Stock(Id,Date,Status,SlipId,VehicleNo,Number,DispatchId,Serial,Godown)" +
                                    //    " values(@Id,@Date,@Status,@SlipId,@VehicleNo,@Number,@DispatchId,@Serial,@Godown)", stock, transaction);



                                    conn.Execute("INSERT INTO Stock(Id, Date, Status, SlipId, VehicleNo, Number, DispatchId, Serial, Godown) " +
             "VALUES (@Id, @Date, @Status, @SlipId, @VehicleNo, @Number, @DispatchId, @Serial, @Godown)",
             new
             {
                 Id = stock.Id,
                 Date = lst.Date,
                 Status = stock.Status,
                 SlipId = stock.SlipId,
                 VehicleNo = stock.VehicleNo,
                 Number = stock.Number,
                 DispatchId = stock.DispatchId,
                 Serial = stock.Serial,
                 Godown = stock.Godown
             }, transaction);



                                    slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId, transaction).FirstOrDefault();
                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook",transaction);
                                    //stockbook.Date = stock.Date;

                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    stockbook.ReelNumber = slip.FormattedNo;

                                    stockbook.Quality = slip.Quality;
                                    if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                                    {
                                        stockbook.Shift = conn.ExecuteScalar<string>("select Shift from Slip where Slip.Id=  " + stock.SlipId);

                                        stockbook.ReelDia = conn.ExecuteScalar<int>("select ReelDia.Id from  ReelDia left join Slip on Slip.ReelDia=ReelDia.Id  where Slip.Id=  " + stock.SlipId);

                                    }
                                    else
                                    {

                                        stockbook.BF = slip.BF;
                                    }
                                    stockbook.GSM = slip.GSMId;
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id= " + stock.SlipId);
                                    stockbook.NetWeight = (-1) * slip.NetWeight;
                                    stockbook.Godown = stock.Godown;
                                    stockbook.Quantity = -1;
                                    stockbook.VoucherType = "Dispatch";
                                    stockbook.Cutter = -1;
                                    //conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia" +
                                    //     ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia)", stockbook, transaction);


                                    conn.Execute("INSERT INTO StockBook(Id, VoucherId, Date, ReelNumber, Quality, BF, GSM, Size, Unit, NetWeight, Quantity, Godown, VoucherType, SlipId, Shift, ReelDia,Cutter) " +
                 "VALUES (@Id, @VoucherId, @Date, @ReelNumber, @Quality, @BF, @GSM, @Size, @Unit, @NetWeight, @Quantity, @Godown, @VoucherType, @SlipId, @Shift, @ReelDia,@Cutter)",
                 new
                 {
                     Id = stockbook.Id,
                     VoucherId = stockbook.VoucherId,
                     Date = lst.Date,
                     ReelNumber = stockbook.ReelNumber,
                     Quality = stockbook.Quality,
                     BF = stockbook.BF,
                     GSM = stockbook.GSM,
                     Size = stockbook.Size,
                     Unit = stockbook.Unit,
                     NetWeight = stockbook.NetWeight,
                     Quantity = stockbook.Quantity,
                     Godown = stockbook.Godown,
                     VoucherType = stockbook.VoucherType,
                     SlipId = stockbook.SlipId,
                     Shift = stockbook.Shift,
                     ReelDia = stockbook.ReelDia,
                     Cutter=stockbook.Cutter
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
                            //sreturn BadRequest(new { Message = "Failed" + ex.Message });
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
                                conn.Execute("Delete from Stockbook where VoucherId=" + id + " and VoucherType='Dispatch'", transaction);
                                conn.Execute("Delete from Dispatch where Id=" + id, transaction);
                                conn.Execute("Delete from Stock where DispatchId=" + id, transaction);
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






