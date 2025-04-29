using Dapper;
using DISPATCHAPI.Models;
using iTextSharp.text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class CutterVMasterApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<CutterVMasterApiController> logger;
        public CutterVMasterApiController(ILogger<CutterVMasterApiController> _logger, IConfiguration _Configuration)
        {
            _logger = logger;
            Configuration = _Configuration;
        }


        [HttpGet]
        public ActionResult Getch(int Fdate, int Tdate, int FormattedNo, int GSM, int Size, int Quality)
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

                if (FormattedNo != 0)
                {
                    where += " and (CutterVMaster.SlipId=" + FormattedNo + " ) ";
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

                List<CutterVMaster> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<CutterVMaster>("select CutterVMaster.*,  " +
                        "Cutters.Name as Cutter,Items.Name as Quality,GSM.Name as GSM," +
                        "Size.Name as SIze,Slip.NetWeight as NetWeight" +
                        " from CutterVMaster  left join Cutters on Cutters.Id=CutterVMaster.CutterId" +
                        " left join SLip on Slip.id=CutterVMaster.SlipId" +
                        " left join Items on Items.Id=Slip.Quality" +
                        " left join GSM on GSM.Id=Slip.GSMId" +
                        " left join Size on Size.Id=Slip.SizeId" +
                        " where (CutterVMaster.Date>=" + fdt + " and CutterVMaster.Date <=" + tdt + ") " + where + "").ToList();

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
                CutterVMaster lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<CutterVMaster>("select * from CutterVMaster where Id=" + id).FirstOrDefault();


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

        public IActionResult Post(CutterVMaster lst)
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
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'", transaction).FirstOrDefault();
                            //int Exist = conn.ExecuteScalar<int>("select Count(*) from StockBook where ReelNumber ='" + lst.ReelNo + "'",transaction);
                            //lst.SlipId = conn.QuerySingleOrDefault<int>("SELECT MIN(SlipId) FROM StockBook WHERE ReelNumber = @ReelNumber",new { ReelNumber = lst.ReelNo },transaction);
                            var result = conn.QuerySingleOrDefault<int?>("SELECT MIN(SlipId) FROM StockBook WHERE ReelNumber = @ReelNumber", new { ReelNumber = lst.ReelNo }, transaction);
                            if (result == null)
                            {
                                lst.SlipId = 0;
                            }
                            else
                            {
                                lst.SlipId = result.Value;  // Use the actual value if it's not null
                            }


                            if (lst.SlipId == 0)
                            {
                                transaction.Rollback();
                                return NotFound(" Invalid QR Code");
                                //return NotFound(new { Message = stock.Number + "Invalid QR Code" });
                            }
                            int Cutter = conn.ExecuteScalar<int>("SELECT MIN(CutterId) FROM CutterVMaster WHERE ReelNo = @ReelNo", new { ReelNo = lst.ReelNo }, transaction);
                            //if (Cutter != -1)
                            if (Cutter != 0)
                            {
                                transaction.Rollback();
                                return Ok("Reel Already Issued to Cutter");
                            }
                            lst.Id = conn.ExecuteScalar<int>("select max(Id) from CutterVMaster") + 1;
                            lst.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            lst.Serial = conn.ExecuteScalar<int>("select max(Serial) from CutterVMaster where CutterId=" + lst.CutterId, transaction) + 1;

                            conn.Execute("insert into CutterVMaster(Id,ReelNo,CutterId,SlipId,Date,Serial)values (@Id,@ReelNo,@CutterId,@SlipId,@Date,@Serial)", lst, transaction);
                            conn.Execute("update StockBook set Cutter=-1  where SlipId=" + lst.SlipId, lst, transaction);
                            conn.Execute("update StockBook set  VoucherType='Cutter'  where SlipId=" + lst.SlipId, lst, transaction);
                            conn.Execute("update StockBook set  Quantity=-1  where SlipId=" + lst.SlipId, lst, transaction);
                            conn.Execute("update StockBook set  NetWeight=-(1)*" + lst.NetWeight + "  where SlipId=" + lst.SlipId, lst, transaction);
                            
                            List<StockBook> stockbooklst = conn.Query<StockBook>("select * from StockBook where SlipId=" + lst.SlipId, transaction).ToList();
                            //StockBook stockbook = new StockBook();
                            //stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                            //stockbook.VoucherId = stockbooklst.FirstOrDefault().VoucherId + 1;
                            //stockbook.SlipId = stockbooklst.FirstOrDefault().SlipId;
                            //stockbook.Date = lst.Date;
                            //stockbook.ReelNumber = stockbooklst.FirstOrDefault().ReelNumber;
                            //stockbook.Quality = stockbooklst.FirstOrDefault().Quality;
                            //stockbook.BF = stockbooklst.FirstOrDefault().BF;
                            //stockbook.ReelDia = stockbooklst.FirstOrDefault().ReelDia;
                            //stockbook.GSM = stockbooklst.FirstOrDefault().GSM;
                            //stockbook.Size = stockbooklst.FirstOrDefault().Size;
                            //stockbook.Shift = stockbooklst.FirstOrDefault().Shift;
                            //stockbook.Unit = stockbooklst.FirstOrDefault().Unit;
                            //stockbook.Godown = stockbooklst.FirstOrDefault().Godown;
                            //stockbook.NetWeight = (-1)*(stockbooklst.FirstOrDefault().NetWeight);
                            //stockbook.Quantity = -1;
                            //stockbook.VoucherType = "Cutter";
                            //stockbook.Cutter = -1;
                            //conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia,Cutter" +
                            //            ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia,@Cutter)", stockbook, transaction);
                            transaction.Commit();
                            return Ok("Created");//return Ok(new { Message = "Created" });                   

                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return BadRequest(new { Message = "Failed" + e.Message });
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

        public IActionResult put(int id, CutterVMaster lst)

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
                                
                                var result = conn.QuerySingleOrDefault<int?>("SELECT MIN(SlipId) FROM StockBook WHERE ReelNumber = @ReelNumber", new { ReelNumber = lst.ReelNo }, transaction);
                                if (result == null)
                                {
                                    lst.SlipId = 0;
                                }
                                else
                                {
                                    lst.SlipId = result.Value;  // Use the actual value if it's not null
                                }


                                if (lst.SlipId == 0)
                                {
                                    transaction.Rollback();
                                    return NotFound(" Invalid QR Code");
                                    //return NotFound(new { Message = stock.Number + "Invalid QR Code" });
                                }
                                int CutterId = lst.CutterId;
                                lst.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                lst.Serial = conn.ExecuteScalar<int>("select max(Serial) from CutterVMaster where CutterId=" + lst.CutterId, transaction) + 1;
                                List<StockBook> stockbooklst = conn.Query<StockBook>("select * from StockBook where SlipId=" + lst.SlipId, transaction).ToList();
                                conn.Execute("Delete from Stockbook where SlipId=" + stockbooklst.FirstOrDefault().SlipId + " and VoucherType='Cutter'", transaction);
                                conn.Execute("update CutterVMaster set ReelNo=@ReelNo,CutterId=" + CutterId + ",SlipId=@SlipId,Date=@Date,Serial=@Serial where Id=" + id, lst, transaction);
                                conn.Execute("update StockBook set Cutter=-1  where SlipId=" + lst.SlipId, lst, transaction);
                                conn.Execute("update StockBook set  VoucherType='Cutter'  where SlipId=" + lst.SlipId, lst, transaction);
                                conn.Execute("update StockBook set  Quantity=-1  where SlipId=" + lst.SlipId, lst, transaction);
                                conn.Execute("update StockBook set  NetWeight=-(1)*"+lst.NetWeight+"  where SlipId=" + lst.SlipId, lst, transaction);
                                //StockBook stockbook = new StockBook();
                                //stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                                //stockbook.VoucherId = stockbooklst.FirstOrDefault().VoucherId + 1;
                                //stockbook.SlipId = stockbooklst.FirstOrDefault().SlipId;
                                //stockbook.Date = lst.Date;
                                //stockbook.ReelNumber = stockbooklst.FirstOrDefault().ReelNumber;
                                //stockbook.Quality = stockbooklst.FirstOrDefault().Quality;
                                //stockbook.BF = stockbooklst.FirstOrDefault().BF;
                                //stockbook.ReelDia = stockbooklst.FirstOrDefault().ReelDia;
                                //stockbook.GSM = stockbooklst.FirstOrDefault().GSM;
                                //stockbook.Size = stockbooklst.FirstOrDefault().Size;
                                //stockbook.Shift = stockbooklst.FirstOrDefault().Shift;
                                //stockbook.Unit = stockbooklst.FirstOrDefault().Unit;
                                //stockbook.Godown = stockbooklst.FirstOrDefault().Godown;
                                //stockbook.NetWeight = (-1)*(stockbooklst.FirstOrDefault().NetWeight);
                                //stockbook.Quantity = -1;
                                //stockbook.VoucherType = "Cutter";
                                //stockbook.Cutter = -1;
                                //conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia,Cutter" +
                                //            ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia,@Cutter)", stockbook, transaction);

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
                        catch (Exception ex)
                        {
                            transaction.Rollback();
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
                    string auth = Request.Headers["auth"].FirstOrDefault();
                    string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                    string[] credentials = decodedStrings.Split(':');
                    string username = credentials[0];
                    string password = credentials[1];
                    User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();

                    if (user.IsDeleteAllowed == 1)
                    {
                        List <CutterVMaster> cut= conn.Query<CutterVMaster>("select * from CutterVMaster where id="+id).ToList();
                        List<StockBook> stockbooklst = conn.Query<StockBook>("select * from StockBook where SlipId=" + cut.FirstOrDefault().SlipId).ToList();
                        conn.Execute("Delete from Stockbook where SlipId=" + stockbooklst.FirstOrDefault().SlipId + " and VoucherType='Cutter'");
                        conn.Execute("Delete from CutterVMaster where Id=" + id);

                        return Ok("Deleted");
                        // return Ok(new { Message = "Deleted" });
                    }
                    else
                    {
                        return BadRequest("you are not allowed");
                        // return BadRequest(new { Message = "you are not allowed" });
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






