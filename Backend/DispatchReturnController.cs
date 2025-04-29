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

    public class DispatchReturnController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<DispatchReturnController> logger;
        private IWebHostEnvironment Environment;
        public DispatchReturnController(ILogger<DispatchReturnController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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
                    where += "  and (DispatchReturn.VehicleNo ='" + vehicle + "' ) ";
                }
                if (Dispatchno != 0)
                {
                    where += "  and (DispatchReturn.VNumber =" + Dispatchno + " ) ";
                }
                List<DispatchReturn> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<DispatchReturn>("select DispatchReturn.*,Count(DispatchReturnMeta.Serial) as Serial ," +
                        " Sum(cast(Slip.NetWeight as double)) as NetWeight from DispatchReturn" +
                         " left join DispatchReturnMeta on  DispatchReturnMeta.DispatchId=DispatchReturn.Id " +
                         " left join Slip on  DispatchReturnMeta.SlipId=Slip.Id " +
                        " where DispatchReturn.Date>=" + fdt + " and DispatchReturn.Date<=" + tdt + where + " group by DispatchReturn.Id").ToList();

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

        public IActionResult Get(int id)
        {
            try
            {
                DispatchReturn lst = new DispatchReturn();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<DispatchReturn>("select * from DispatchReturn where Id=" + id).FirstOrDefault();
                    lst.stock = conn.Query<DispatchReturnMeta>("select DispatchReturnMeta.*, Godown.Name as GodownName , " +
                        " Items.Name as ItemName, BF.Name as BFName,ReelDia.Name as ReelDiaName, GSM.Name as GSMName ,Users.UserName as EnteredName , (Size.Name|| ' ' || Size.Unit) as SizeName,Slip.Id as SlipId, Slip.NetWeight as VNetWeight " +
                        "from DispatchReturnMeta" +
                        " left join slip on DispatchReturnMeta.SlipId=Slip.Id" +
                        " left join Items on Slip.Quality = Items.Id " +
                        "left join BF on Slip.BF = BF.Id " +
                        "left join ReelDia on Slip.ReelDia = ReelDia.Id " +
                        "left join Users on Slip.EntredBy = Users.Id " +
                        "left join GSM on Slip.GSMId = GSM.Id " +
                        "left join Size on Slip.SizeId=Size.Id" +
                        " left join Godown on DispatchReturnMeta.Godown=Godown.Id where DispatchId=" + id).ToList();
                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post(DispatchReturn lst)
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
                            string client = Request.Headers["ClientUSID"].FirstOrDefault();
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
                            lst.Id = conn.ExecuteScalar<int>("select max(Id) from DispatchReturn") + 1;
                            lst.VNumber = conn.ExecuteScalar<int>("select max(VNumber) from Dispatch where FYID=@FYID", new { FYID = FYID }, transaction) + 1;
                            
                            lst.Status = 0;
                            conn.Execute("insert into DispatchReturn(Id,Date,VNumber,VehicleNo,CreateDate,CreateTime,Enteredby,Remark,Status,FYID)" +
                                "values(@Id,@Date,@VNumber,@VehicleNo,@CreateDate,@CreateTime,@Enteredby,@Remark,@Status,@FYID)", lst,transaction);
                            StockBook stockbook;
                            Slip slip;
                            foreach (DispatchReturnMeta stock in lst.stock)
                            {

                                int Duplicate = conn.ExecuteScalar<int>("select Sum(Quantity) from Stockbook where ReelNumber ='" + stock.Number + "'");
                                //int Duplicate = conn.ExecuteScalar<int>("select Count(*) from DispatchReturnMeta where Number ='" + stock.Number + "'");
                                if (Duplicate == 1)
                                {
                                    return Conflict(new { Message = stock.Number+  "Either not Dispatched or Already Returned." });
                                    //return Conflict(new { Message = "Already Scanned" });
                                }
                                int Exist = conn.ExecuteScalar<int>("select Count(*) from Slip where FormattedNo ='" + stock.Number + "'");
                                //int Exist = conn.ExecuteScalar<int>("select Count(*) from Stock where Number ='" + stock.Number + "'");
                                if (Exist == 0)
                                {
                                    return NotFound(new { Message = stock.Number + "Invalid QR Code" });
                                }

                                stock.Godown = conn.ExecuteScalar<int>("select GodownId from Slip where FormattedNo ='" + stock.Number + "'");
                                stock.DispatchId = lst.Id;
                                stock.Id = conn.ExecuteScalar<int>("select max(Id) from DispatchReturnMeta") + 1;
                                stock.Serial = conn.ExecuteScalar<int>("select max(Serial) from DispatchReturnMeta where DispatchId=" + stock.DispatchId) + 1;
                                stock.Status = 1;
                                stock.SlipId = conn.ExecuteScalar<int>("select Id from Slip where FormattedNo ='" + stock.Number + "' ");
                                stock.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                conn.Execute("insert into DispatchReturnMeta(Id,Date,Status,SlipId,VehicleNo,Number,DispatchId,Serial,Godown)" +
                                    " values(@Id,@Date,@Status,@SlipId,@VehicleNo,@Number,@DispatchId,@Serial,@Godown)", stock, transaction);

                                slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId).FirstOrDefault();
                                stockbook = new StockBook();
                                stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                stockbook.Date = stock.Date;
                                //stockbook.VoucherId = stock.DispatchId;
                                //stockbook.SlipId = stock.SlipId;
                                //stockbook.ReelNumber = stock.Number;
                                //stockbook.Quality = conn.ExecuteScalar<int>("select Items.Id from  Items left join Slip on Slip.Quality=Items.Id  where Slip.Id= " + stock.SlipId);
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
                                    //stockbook.BF = conn.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);
                                    stockbook.BF = slip.BF;// conn.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);
                                }
                                stockbook.GSM = slip.GSMId;// conn.ExecuteScalar<int>("select GSM.Id from  GSM left join Slip on Slip.GSMId=GSM.Id  where Slip.Id=  " + stock.SlipId);
                                stockbook.Size = slip.SizeId;
                                stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id= " + stock.SlipId);
                                stockbook.NetWeight = slip.NetWeight;//(conn.ExecuteScalar<decimal>("select NetWeight from  Slip where Id= " + stock.SlipId)); 
                                stockbook.Godown = stock.Godown;
                                stockbook.Quantity = 1;
                                stockbook.VoucherType = "DispatchReturn";
                                stockbook.Cutter =0;
                                conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia,Cutter" +
                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia,@Cutter)", stockbook, transaction);
                            }
                            transaction.Commit();
                            return Ok("Created");
                            //return Ok(new { Message = "Created" });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                           // throw;
                            return BadRequest(new { Message = "Failed" + ex.Message });
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

        [HttpPut("{id}")]

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
                        try {
                            string client = Request.Headers["ClientUSID"].FirstOrDefault();
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            DispatchReturn lst1 = conn.Query<DispatchReturn>("select * from DispatchReturn where Id=" + id).FirstOrDefault();
                            lst1.stock = conn.Query<DispatchReturnMeta>("select DispatchReturnMeta.*, Godown.Name as GodownName , " +
                                " Items.Name as ItemName, BF.Name as BFName, GSM.Name as GSMName ,Users.UserName as EnteredName , (Size.Name|| ' ' || Size.Unit) as SizeName,Slip.Id as SlipId, Slip.NetWeight as VNetWeight " +
                                "from DispatchReturnMeta" +
                                " left join slip on DispatchReturnMeta.SlipId=Slip.Id" +
                                " left join Items on Slip.Quality = Items.Id " +
                                "left join BF on Slip.BF = BF.Id " +
                                "left join Users on Slip.EntredBy = Users.Id " +
                                "left join GSM on Slip.GSMId = GSM.Id " +
                                "left join Size on Slip.SizeId=Size.Id" +
                                " left join Godown on DispatchReturnMeta.Godown=Godown.Id where DispatchId=" + id).ToList();
                            if (user.IsEditAllowed == 1)
                            {
                                conn.Execute("Delete from Stockbook where VoucherId=" + id + " and VoucherType='DispatchReturn'", transaction);
                                conn.Execute("Delete from DispatchReturnMeta where DispatchId=" + id, transaction);
                                conn.Execute("Update DispatchReturn set Remark=@Remark where Id=" + id, lst, transaction);

                                StockBook stockbook;
                                Slip slip;

                                foreach (DispatchReturnMeta stock in lst.stock)
                                {
                                    //int Duplicate = conn.ExecuteScalar<int>("select Count(*) from DispatchReturnMeta where Number ='" + stock.Number + "'");
                                    //if (Duplicate > 0)
                                    //{
                                    //    return Conflict(new { Message = "Already Scanned" });
                                    //}
                                    //int Exist = conn.ExecuteScalar<int>("select Count(*) from Stock where Number ='" + stock.Number + "'");
                                    //if (Exist == 0)
                                    //{
                                    //    return NotFound(new { Message = "Invalid QR Code" });
                                    //}
                                    int Duplicate = conn.ExecuteScalar<int>("select Sum(Quantity) from Stockbook where ReelNumber ='" + stock.Number + "'");
                                    //int Duplicate = conn.ExecuteScalar<int>("select Count(*) from DispatchReturnMeta where Number ='" + stock.Number + "'");
                                    if (Duplicate == 1)
                                    {
                                        return Conflict(new { Message = stock.Number + "Either not Dispatched or Already Returned." });
                                        //return Conflict(new { Message = "Already Scanned" });
                                    }
                                    int Exist = conn.ExecuteScalar<int>("select Count(*) from Slip where FormattedNo ='" + stock.Number + "'");
                                    //int Exist = conn.ExecuteScalar<int>("select Count(*) from Stock where Number ='" + stock.Number + "'");
                                    if (Exist == 0)
                                    {
                                        return NotFound(new { Message = stock.Number+"Invalid QR Code" });
                                    } 
                                    stock.Godown = conn.ExecuteScalar<int>("select GodownId from Slip where FormattedNo ='" + stock.Number + "'");
                                    stock.DispatchId = id;
                                    stock.Id = conn.ExecuteScalar<int>("select max(Id) from DispatchReturnMeta") + 1; 
                                    stock.Serial = conn.ExecuteScalar<int>("select max(Serial) from DispatchReturnMeta") + 1;
                                    stock.Status = 1;
                                    stock.SlipId = conn.ExecuteScalar<int>("select Id from Slip where FormattedNo ='" + stock.Number + "' and NetWeight=" + stock.VNetWeight);

                                    stock.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                    conn.Execute("insert into DispatchReturnMeta(Id,Date,Status,SlipId,VehicleNo,Number,DispatchId,Serial,Godown)" +
                                        " values(@Id,@Date,@Status,@SlipId,@VehicleNo,@Number,@DispatchId,@Serial,@Godown)", stock, transaction);
                                    slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId).FirstOrDefault();

                                    stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                    stockbook.Date = stock.Date;
                                    //stockbook.VoucherId = stock.DispatchId;
                                    //stockbook.SlipId = stock.SlipId;
                                    //stockbook.ReelNumber = stock.Number;
                                    //stockbook.Quality = conn.ExecuteScalar<int>("select Items.Id from  Items left join Slip on Slip.Quality=Items.Id  where Slip.Id= " + stock.SlipId);
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = slip.Id;
                                    //stock.SlipId = conn.ExecuteScalar<int>("select Id from Slip where FormattedNo ='" + stock.Number + "' and NetWeight=" + stock.VNetWeight, transaction);
                                    stockbook.ReelNumber = slip.FormattedNo;
                                    stockbook.Quality = slip.Quality;
                                    if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                                    {
                                        stockbook.Shift = conn.ExecuteScalar<string>("select Shift from Slip where Slip.Id=  " + stock.SlipId);

                                        stockbook.ReelDia = conn.ExecuteScalar<int>("select ReelDia.Id from  ReelDia left join Slip on Slip.ReelDia=ReelDia.Id  where Slip.Id=  " + stock.SlipId);

                                    }
                                    else
                                    {
                                        //stockbook.BF = conn.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);
                                        stockbook.BF = slip.BF;// conn.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);
                                    }
                                    //stockbook.GSM = conn.ExecuteScalar<int>("select GSM.Id from  GSM left join Slip on Slip.GSMId=GSM.Id  where Slip.Id=  " + stock.SlipId);
                                    //stockbook.Size = conn.ExecuteScalar<int>("select Size.Id from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id=  " + stock.SlipId);
                                    stockbook.GSM = slip.GSMId;// conn.ExecuteScalar<int>("select GSM.Id from  GSM left join Slip on Slip.GSMId=GSM.Id  where Slip.Id=  " + stock.SlipId);
                                    stockbook.Size = slip.SizeId;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id= " + stock.SlipId);
                                    stockbook.NetWeight = slip.NetWeight; //(conn.ExecuteScalar<decimal>("select NetWeight from  Slip where Id= " + stock.SlipId)); //(-)
                                    stockbook.Godown = stock.Godown;
                                    stockbook.Quantity = 1;
                                    stockbook.VoucherType = "DispatchReturn";
                                    stockbook.Cutter = 0;
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia,Cutter" +
                                          ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia,@Cutter)", stockbook, transaction);
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
               // return BadRequest(new { Message = "Failed" + ex.Message });
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
                        try {
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (user.IsDeleteAllowed == 1)
                            {
                                conn.Execute("Delete from Stockbook where VoucherId=" + id + " and VoucherType='DispatchReturn'",transaction);
                                conn.Execute("Delete from DispatchReturn where Id=" + id, transaction);
                                conn.Execute("Delete from DispatchReturnMeta where DispatchId=" + id, transaction);
                                transaction.Commit();
                                return Ok("Deleted");
                                //return Ok(new { Message = "Deleted" });
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




