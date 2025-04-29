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

    public class BundleApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<BundleApiController> logger;
        private IWebHostEnvironment Environment;
        public BundleApiController(ILogger<BundleApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }


        [HttpGet]
        public ActionResult Getch(int Fdate, int Tdate, string SetNo, int GodownId, int FormattedNo, int BF, int GSM, int Size, int Quality, int ReelDia, string Status)
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
                    where = " and( Bundle.SetNo ='" + SetNo + "') ";
                }
                if (GodownId != 0)
                {
                    where = " and( Bundle.GodownId =" + GodownId + ") ";
                }
                if (FormattedNo != 0)
                {
                    where += " and (Bundle.BundleNo=" + FormattedNo + " ) ";
                }
                if (BF != 0)
                {
                    where += " and (Bundle.BF=" + BF + " ) ";
                }
                if (GSM != 0)
                {
                    where += " and (Bundle.GSMId=" + GSM + " ) ";
                }
                if (Size != 0)
                {
                    where += " and (Bundle.SizeId=" + Size + " ) ";
                }
                if (Quality != 0)
                {
                    where += " and (Bundle.Quality=" + Quality + " ) ";
                }
                if (ReelDia != 0)
                {
                    where += " and (Bundle.ReelDia=" + ReelDia + " ) ";
                }
                if (Status != null)
                {
                    if (Status == "0")
                    {
                        where += " and (Bundle.BundleStatus IS NULL OR Bundle.BundleStatus = '' or Bundle.BundleStatus=0)  ";
                    }
                    else if (Status == "2")
                    {
                        where += " and (Bundle.BundleStatus IS NULL OR Bundle.BundleStatus = '' or Bundle.BundleStatus=0 or Bundle.BundleStatus=1)  ";

                    }
                    else
                    {
                        where += " and (Bundle.BundleStatus=" + Status + " ) ";

                    }


                }


                List<Bundle> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<Bundle>("select Bundle.Date,Bundle.Id, Bundle.NoOfRim,Bundle.RimWeight,Bundle.BundleWeight,Bundle.FormattedNo," +
                        "Items.Name as QualityName,GSM.Name as GSM," +
                        "BundleSize.Name as Size from Bundle " +
                        "left join Items on Items.id=Bundle.Quality " +
                        "left join GSM on GSM.Id=Bundle.GSMId " +
                        "left join BundleSize on BundleSize.Id=Bundle.BundleSizeId " +
                        " where (Bundle.Date>=" + fdt + " and Bundle.Date <=" + tdt + ") " + where + "" +
                        "group by Bundle.Id, Bundle.NoOfRim,Bundle.RimWeight,Bundle.BundleWeight,Items.Name,GSM.Name," +
                        "BundleSize.Name   order by Bundle.Id Desc").ToList();

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

                Bundle lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<Bundle>("select Bundle.*  from Bundle  where Id = " + id).FirstOrDefault();
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
        public IActionResult Post(Bundle lst)
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
                            lst.Id = conn.ExecuteScalar<int>("select max(Id) from Bundle") + 1;

                            lst.BundleNo = conn.ExecuteScalar<int>("select max(BundleNo) from Bundle where Date=" + lst.Date) + 1;

                            DateTime now = lst.Date.ToDate();

                            string yearAsAlphabet = Utility.NumberToAlphabet((now.Year / 10) % 10); // Converts the tens digit to alphabet
                            yearAsAlphabet += Utility.NumberToAlphabet(now.Year % 10); // Converts the ones digit to alphabet
                            string monthAsAlphabet = Utility.NumberToAlphabet(now.Month);

                            if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                            {
                                lst.FormattedNo = monthAsAlphabet + lst.Date.ToDate().ToString("dd") + yearAsAlphabet + lst.BundleNo.ToString().PadLeft(4, '0');
                                DateTime currentTime = DateTime.Now;
                                TimeSpan sixAM = new TimeSpan(7, 0, 0);      // 06:00 AM
                                TimeSpan twoPM = new TimeSpan(14, 0, 0);     // 02:00 PM
                                TimeSpan tenPM = new TimeSpan(22, 0, 0);     // 10:00 PM

                             

                            }
                            

                            conn.Execute("insert into Bundle(Id,Date,GSMId,RimWeight,Quality,BundleWeight," +
                                "BundleSizeId,NoOfRim,FormattedNo,BundleNo)values " +
                                "(@Id,@Date,@GSMId,@RimWeight,@Quality,@BundleWeight,@BundleSizeId,@NoOfRim,@FormattedNo,@BundleNo)", lst, transaction);

                            BundleStockBook bundlestock = new BundleStockBook();
                            bundlestock.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from BundleStockBook");
                            bundlestock.VoucherId = lst.Id;
                            bundlestock.BundleId = lst.Id;
                            bundlestock.Date = lst.Date;
                            bundlestock.BundleNo = lst.FormattedNo;
                            bundlestock.Quality = lst.Quality;
                           
                            bundlestock.GSM = lst.GSMId;
                            bundlestock.BundleSize = lst.BundleSizeId;
                           
                            bundlestock.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + lst.BundleSizeId);                          
                            bundlestock.BundleWeight = lst.BundleWeight;
                            
                            bundlestock.Quantity = 1;
                            bundlestock.VoucherType = "Bundle";
                            conn.Execute("insert into BundleStockBook(Id,VoucherId,Date,Quality,BF,GSM,BundleSize,Unit,BundleWeight,Quantity,VoucherType,BundleId,BundleNo" +
                                        ") values(@Id,@VoucherId,@Date,@Quality,@BF,@GSM,@BundleSize,@Unit,@BundleWeight,@Quantity,@VoucherType,@BundleId,@BundleNo)", bundlestock, transaction);
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

        public IActionResult Put(int id, Bundle lst)

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
                            int edit = conn.ExecuteScalar<int>("select BundleDispatch.Status from  BundleDispatch left join BundleStock on BundleStock.BundleDispatchId=BundleDispatch.Id where BundleStock.BundleId=" + lst.Id, transaction);
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

                                    

                                }

                                //if (edit == 0)
                                //{
                                    conn.Execute("Delete from BundleStockBook where VoucherId=" + id + " and VoucherType='Bundle'", transaction);
                                    conn.Execute("UPDATE Bundle SET Date = @Date, GSMId = @GSMId," +
                                        " Quality = @Quality,BundleWeight = @BundleWeight ,BundleSizeId=@BundleSizeId, RimWeight=@RimWeight,NoOfRim=@NoOfRim,FormattedNo=@FormattedNo" +
                                        "  where Id=" + id, lst, transaction);
                                BundleStockBook bundlestock = new BundleStockBook();
                                bundlestock.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from BundleStockBook");
                                bundlestock.VoucherId = lst.Id;
                                bundlestock.BundleId = lst.Id;
                                bundlestock.Date = lst.Date;
                                bundlestock.BundleNo = lst.FormattedNo;
                                bundlestock.Quality = lst.Quality;

                                bundlestock.GSM = lst.GSMId;
                                bundlestock.BundleSize = lst.BundleSizeId;

                                bundlestock.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + lst.BundleSizeId);
                                bundlestock.BundleWeight = lst.BundleWeight;

                                bundlestock.Quantity = 1;
                                bundlestock.VoucherType = "Bundle";
                                conn.Execute("insert into BundleStockBook(Id,VoucherId,Date,Quality,BF,GSM,BundleSize,Unit,BundleWeight,Quantity,VoucherType,BundleId,BundleNo" +
                                            ") values(@Id,@VoucherId,@Date,@Quality,@BF,@GSM,@BundleSize,@Unit,@BundleWeight,@Quantity,@VoucherType,@BundleId,@BundleNo)", bundlestock, transaction);
                                transaction.Commit();
                                    return Ok("Updated");
                                    //return Ok(new { Message = "Updated" });
                                }
                           //     else
                           //     {
                           //         return BadRequest("Bundle is not edit after dispatch.");
                           //         //return BadRequest(new { Message = "Bundle is not edit after dispatch." });
                           //     }
                           //// }

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
                        try
                        {
                            int edit = conn.ExecuteScalar<int>("select BundleDispatch.Status from  BundleDispatch left join BundleStock on BundleStock.BundleDispatchId=BundleDispatch.Id where BundleStock.BundleId=" + id, transaction);
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            int sqrt = conn.ExecuteScalar<int>("select count(*) from BundleStock where BundleStock.BundleId=" + id);
                            //int sdrtm = conn.ExecuteScalar<int>("select count(*) from DispatchReturnMeta where DispatchReturnMeta.BundleId=" + id);
                            //int sgtm = conn.ExecuteScalar<int>("select count(*) from GodownTransferMeta where GodownTransferMeta.BundleId=" + id);
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (sqrt > 0)
                            {
                                return Ok("This reel can't be deleted after dispatch");
                                //return Ok(new { Message = "This reel can't be deleted after dispatch" });
                            }
                            if (user.IsDeleteAllowed == 1)
                            {

                                if (edit == 0)
                                {
                                    conn.Execute("Delete from BundleStockBook where VoucherId=" + id + " and VoucherType='Bundle'", transaction);
                                    conn.Execute("Delete from Bundle where Id=" + id, transaction);
                                    transaction.Commit();
                                    return Ok("Deleted");
                                    // return Ok(new { Message = "Deleted" });
                                }
                                else
                                {
                                    return BadRequest("Bundle is not delete after dispatch.");
                                    //return BadRequest(new { Message = "Bundle is not delete after dispatch." });
                                }
                            }
                            else
                            {
                                return BadRequest("You are not Allowed");
                                //return BadRequest(new {Message= "You are not Allowed" });

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

