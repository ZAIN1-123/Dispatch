using Dapper;
using DISPATCHAPI;
using DISPATCHAPI.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class SlipApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<SlipApiController> logger;
        private IWebHostEnvironment Environment;
        public SlipApiController(ILogger<SlipApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }
         

        [HttpGet]
        public ActionResult Getch(int Fdate, int Tdate,string SetNo, int GodownId, int FormattedNo, int BF, int GSM, int Size, int Quality, int ReelDia, string Status)
        {
            string where = "";
            try
            {
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
                if (SetNo != null)
                { 
                    where = " and( Slip.SetNo ='" + SetNo + "') ";
                }
                if (GodownId != 0)
                {
                    where = " and( Slip.GodownId =" + GodownId + ") ";
                }
                if (FormattedNo != 0)
                {
                    where += " and (Slip.Id=" + FormattedNo + " ) ";
                }
                if (BF != 0)
                {
                    where += " and (Slip.BF=" + BF + " ) ";
                }
                if (GSM != 0)
                {
                    where += " and (Slip.GSMId=" + GSM + " ) ";
                }
                if (Size != 0)
                {
                    where += " and (Slip.SizeId=" + Size + " ) ";
                }
                if (Quality != 0)
                {
                    where += " and (Slip.Quality=" + Quality + " ) ";
                }
                if (ReelDia != 0)
                {
                    where += " and (Slip.ReelDia=" + ReelDia + " ) ";
                }
                if (Status != null)
                {
                    if(Status=="0")
                    {
                        where += " and (Slip.SlipStatus IS NULL OR Slip.SlipStatus = '' or slip.SlipStatus=0)  ";
                    }
                    else if(Status=="2")
                    {
                        where += " and (Slip.SlipStatus IS NULL OR Slip.SlipStatus = '' or slip.SlipStatus=0 or Slip.SlipStatus=1)  ";

                    }
                    else
                    {
                        where += " and (Slip.SlipStatus=" + Status + " ) ";

                    }


                }


                List<Slip> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<Slip>("select Slip.*,(CASE WHEN Slip.SlipStatus = 1 THEN 'Repacking' ELSE 'Slip' END) as SStatus,Items.Name as ItemName, BF.Name as BFName, GSM.Name as GSM ,ReelDia.Name as ReelDiaName,Users.UserName as EnteredName , (Size.Name|| ' ' || Size.Unit) as Size ,Size.Unit as Unitname " +
                        "from Slip left join Items on Slip.Quality=Items.Id left join BF on Slip.BF=BF.Id left join ReelDia on Slip.ReelDia=ReelDia.Id left join Users on Slip.EntredBy=Users.Id left join GSM on Slip.GSMId=GSM.Id " +
                        "left join Size on Slip.SizeId=Size.Id" +
                        " where (Slip.Date>=" + fdt + " and Slip.Date <=" + tdt + ") " + where + "  order by Slip.Id Desc").ToList();

                    return Ok(lst);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
               // return BadRequest(new { Message = "Failed" + ex.Message });

            }
        }


        [HttpGet("{Id}")]

        public IActionResult Getch(int id)
        {
            try
            {

                Slip lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<Slip>("select Slip.*  from Slip  where Id = " + id).FirstOrDefault();
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
        public IActionResult Post(Slip lst)
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
                            string a = conn.ExecuteScalar<string>("select Godown.Name from Godown left join Slip on Slip.GodownId=Godown.Id where Godown.Id=" + lst.GodownId);

                            lst.EntryDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            lst.EntryTime = int.Parse(DateTime.Now.ToString("HHmmss"));

                            string client = Request.Headers["ClientUSID"].FirstOrDefault();
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();

                            //manvii
                            if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                            {
                                if (user.BackDateAllowed == 0 && user.manual == 0)
                                {
                                    DateTime updatedDateTime = DateTime.Now;

                                    if (updatedDateTime.Hour < 7)
                                    {

                                        updatedDateTime = DateTime.Now.AddDays(-1);
                                    }
                                    else
                                    {

                                        updatedDateTime = DateTime.Now;

                                    }
                                    lst.Date = int.Parse(updatedDateTime.ToString("yyyyMMdd"));
                                }
                            }
                            else
                            {


                                if (user.BackDateAllowed == 0 && user.manual == 0)
                                {
                                    DateTime updatedDateTime = DateTime.Now;

                                    if (updatedDateTime.Hour < 7)
                                    {

                                        updatedDateTime = DateTime.Now.AddDays(-1);
                                    }
                                    else
                                    {

                                        updatedDateTime = DateTime.Now;

                                    }
                                    lst.Date = int.Parse(updatedDateTime.ToString("yyyyMMdd"));
                                }
                            }




                            lst.EntredBy = user.Id;
                            lst.Id = conn.ExecuteScalar<int>("select max(Id) from Slip") + 1;

                            lst.ReelNo = conn.ExecuteScalar<int>("select max(ReelNo) from Slip where Date=" + lst.Date + " and GodownId=" + lst.GodownId) + 1;

                            lst.ProductionType = "FRESH";

                            DateTime now = lst.Date.ToDate();

                            string yearAsAlphabet = Utility.NumberToAlphabet((now.Year / 10) % 10); // Converts the tens digit to alphabet
                            yearAsAlphabet += Utility.NumberToAlphabet(now.Year % 10); // Converts the ones digit to alphabet
                            string monthAsAlphabet = Utility.NumberToAlphabet(now.Month);

                            if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                            {
                                lst.FormattedNo = monthAsAlphabet + lst.Date.ToDate().ToString("dd") + yearAsAlphabet + lst.ReelNo.ToString().PadLeft(4, '0');
                                DateTime currentTime = DateTime.Now;
                                TimeSpan sixAM = new TimeSpan(7, 0, 0);      // 06:00 AM
                                TimeSpan twoPM = new TimeSpan(14, 0, 0);     // 02:00 PM
                                TimeSpan tenPM = new TimeSpan(22, 0, 0);     // 10:00 PM

                                if (currentTime.TimeOfDay >= sixAM && currentTime.TimeOfDay < twoPM)
                                {
                                    lst.Shift = "A";
                                }
                                else if (currentTime.TimeOfDay >= twoPM && currentTime.TimeOfDay < tenPM)
                                {
                                    lst.Shift = "B";
                                }
                                else if (currentTime.TimeOfDay >= tenPM || currentTime.TimeOfDay < sixAM)
                                {
                                    lst.Shift = "C";
                                }
                                else
                                {
                                    // Handle other cases or provide a default value
                                    lst.Shift = "DefaultShift";
                                }

                            }
                            else
                            {

                                lst.FormattedNo = a + "-" + lst.Date.ToDate().ToString("dd") + monthAsAlphabet + yearAsAlphabet + lst.ReelNo.ToString().PadLeft(4, '0');
                                lst.Shift = "";
                            }

                            conn.Execute("insert into Slip(Id,Date,ReelNo,GSMId,BF,Weight,GodownId,Quality,Wastage,NetWeight,SetNo," +
                                "SizeId,EntryTime,EntryDate,EntredBy,ProductionType,FormattedNo,WastageType,Ismanual,Shift,SlipStatus,ReelDia)values " +
                                "(@Id,@Date,@ReelNo,@GSMId,@BF,@Weight,@GodownId,@Quality,@Wastage,@NetWeight,@SetNo," +
                                "@SizeId,@EntryTime,@EntryDate,@EntredBy,@ProductionType,@FormattedNo,@WastageType,@Ismanual,@Shift,@SlipStatus,@ReelDia)", lst,transaction);

                            StockBook stockbook = new StockBook();
                            stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                            stockbook.VoucherId = lst.Id;
                            stockbook.SlipId = lst.Id;
                            stockbook.Date = lst.Date;
                            stockbook.ReelNumber = lst.FormattedNo;
                            stockbook.Quality = lst.Quality;
                            if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                            {

                                stockbook.BF = lst.ReelDia;
                            }
                            else
                            {
                                stockbook.BF = lst.BF;

                            }
                            stockbook.GSM = lst.GSMId;
                            stockbook.Size = lst.SizeId;
                            stockbook.Shift = lst.Shift;
                            stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + lst.SizeId);
                            stockbook.Godown = lst.GodownId;
                            stockbook.NetWeight = lst.NetWeight;
                            stockbook.ReelDia = lst.ReelDia;
                            stockbook.Quantity = 1;
                            stockbook.VoucherType = "Slip";
                            stockbook.Cutter = 0;
                            conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia,Cutter" +
                                        ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia,@Cutter)", stockbook, transaction);
                            transaction.Commit();
                            return Ok(lst);
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
               // return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }


        [HttpPut("{Id}")]

        public IActionResult Put(int id, Slip lst)

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
                            int edit = conn.ExecuteScalar<int>("select Dispatch.Status from  Dispatch left join Stock on Stock.DispatchId=Dispatch.Id where Stock.SlipId=" + lst.Id, transaction);
                            string client = Request.Headers["ClientUSID"].FirstOrDefault();
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (user.IsEditAllowed == 1)
                            {


                                if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                                {
                                    DateTime currentTime = DateTime.Now;
                                    TimeSpan sixAM = new TimeSpan(7, 0, 0);      // 06:00 AM
                                    TimeSpan twoPM = new TimeSpan(14, 0, 0);     // 02:00 PM
                                    TimeSpan tenPM = new TimeSpan(22, 0, 0);     // 10:00 PM

                                    if (currentTime.TimeOfDay >= sixAM && currentTime.TimeOfDay < twoPM)
                                    {
                                        lst.Shift = "A";
                                    }
                                    else if (currentTime.TimeOfDay >= twoPM && currentTime.TimeOfDay < tenPM)
                                    {
                                        lst.Shift = "B";
                                    }
                                    else if (currentTime.TimeOfDay >= tenPM || currentTime.TimeOfDay < sixAM)
                                    {
                                        lst.Shift = "C";
                                    }
                                    else
                                    {
                                        // Handle other cases or provide a default value
                                        lst.Shift = "DefaultShift";
                                    }

                                }

                                if (edit == 0)
                                {
                                    conn.Execute("Delete from Stockbook where VoucherId=" + id + " and VoucherType='Slip'", transaction);
                                    conn.Execute("UPDATE Slip SET Date = @Date, GSMId = @GSMId,ReelDia=@ReelDia, BF = @BF, Weight = @Weight, GodownId = @GodownId," +
                                        " Quality = @Quality, Wastage = @Wastage,WastageType=@WastageType, NetWeight = @NetWeight, SetNo = @SetNo, SizeId = @SizeId," +
                                        " ProductionType = @ProductionType, Ismanual=@Ismanual,NetWeight = @NetWeight,Shift=@Shift,SlipStatus=@SlipStatus  where Id=" + id, lst, transaction);
                                    StockBook stockbook = new StockBook();
                                    stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                    stockbook.VoucherId = lst.Id;
                                    stockbook.SlipId = lst.Id;   
                                    stockbook.Date = lst.Date;
                                    stockbook.ReelNumber = lst.FormattedNo;
                                    stockbook.Quality = lst.Quality;
                                    if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                                    {

                                        stockbook.BF = lst.ReelDia;
                                    }
                                    else
                                    {
                                        stockbook.BF = lst.BF;

                                    }
                                    stockbook.GSM = lst.GSMId;
                                    stockbook.Size = lst.SizeId;
                                    stockbook.Shift = lst.Shift;
                                    stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + lst.SizeId);
                                    stockbook.Godown = lst.GodownId;
                                    stockbook.NetWeight = lst.NetWeight;
                                    stockbook.ReelDia = lst.ReelDia;
                                    stockbook.Quantity = 1;
                                    stockbook.VoucherType = "Slip";
                                    stockbook.Cutter = 0;
                                    conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia,Cutter" +
                                           ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia,@Cutter)", stockbook, transaction);
                                    transaction.Commit();
                                    return Ok("Updated");
                                    //return Ok(new { Message = "Updated" });
                                }
                                else
                                {
                                    return BadRequest("Slip is not edit after dispatch.");
                                    //return BadRequest(new { Message = "Slip is not edit after dispatch." });
                                }
                            }
                          
                            else
                            {
                                return BadRequest("You are not allowed.");
                                //return BadRequest(new { Message = "You are not allowed." });
                            }
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
                            int edit = conn.ExecuteScalar<int>("select Dispatch.Status from  Dispatch left join Stock on Stock.DispatchId=Dispatch.Id where Stock.SlipId=" + id, transaction);
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            int sqrt = conn.ExecuteScalar<int>("select count(*) from Stock where Stock.SlipId=" + id);
                            int sdrtm = conn.ExecuteScalar<int>("select count(*) from DispatchReturnMeta where DispatchReturnMeta.SlipId=" + id);
                            int sgtm = conn.ExecuteScalar<int>("select count(*) from GodownTransferMeta where GodownTransferMeta.SlipId=" + id);
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (sqrt > 0 || sdrtm > 0 || sgtm > 0)
                            {
                                return Ok("This reel can't be deleted after dispatch");
                                //return Ok(new { Message = "This reel can't be deleted after dispatch" });
                            }
                            if (user.IsDeleteAllowed == 1)
                            {

                                if (edit == 0)
                                {
                                    conn.Execute("Delete from Stockbook where VoucherId=" + id + " and VoucherType='Slip'", transaction);
                                    conn.Execute("Delete from Slip where Id=" + id, transaction);
                                    transaction.Commit();
                                   return Ok("Deleted");
                                   // return Ok(new { Message = "Deleted" });
                                }
                                else
                                {
                                    return BadRequest("Slip is not delete after dispatch.");
                                    //return BadRequest(new { Message = "Slip is not delete after dispatch." });
                                }
                            }
                            else
                            {
                                return BadRequest("You are not Allowed");
                                //return BadRequest(new {Message= "You are not Allowed" });

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

