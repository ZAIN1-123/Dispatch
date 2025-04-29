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
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{

    public class UpdateController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<UpdateController> logger;
        private IWebHostEnvironment Environment;
        public UpdateController(ILogger<UpdateController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        public IActionResult Update()
        {
            string ConnString = this.Configuration.GetConnectionString("MyConn");
            try
            {
                using (SQLiteConnection db = new SQLiteConnection(ConnString))
                {
                    List<int> py1 = db.Query<int>(" select Distinct(Date) from Slip").ToList();
                    List<Godown> godown = db.Query<Godown>("select * from Godown").ToList();
                    int i = 1;

                    foreach (int date in py1)
                    {
                        foreach (Godown g in godown)
                        {

                            List<Slip> py2 = db.Query<Slip>("select * from Slip where GodownId=" + g.Id + " and Date=" + date).ToList();

                            i = 1;
                            foreach (Slip slip1 in py2.OrderBy(o => o.Id))
                            {
                                slip1.ReelNo = i;

                                string a = db.ExecuteScalar<string>("select Godown.Name from Godown left join Slip on Slip.GodownId=Godown.Id where Godown.Id=" + slip1.GodownId);



                                DateTime now = DateTime.Parse(slip1.Date.ToDate().ToString("dd-MM-yyyy"));
                                //string yearAsAlphabet = Utility.NumberToAlphabet(now.Year % 100); // % 100 to get only the last two digits
                                string yearAsAlphabet = Utility.NumberToAlphabet((now.Year / 10) % 10); // Converts the tens digit to alphabet
                                yearAsAlphabet += Utility.NumberToAlphabet(now.Year % 10); // Converts the ones digit to alphabet
                                string monthAsAlphabet = Utility.NumberToAlphabet(now.Month);
                                if (slip1.Date == int.Parse(DateTime.Now.ToString("yyyyMMdd")))
                                {
                                    slip1.FormattedNo = a + "-" + DateTime.Now.ToString("dd") + monthAsAlphabet + yearAsAlphabet + slip1.ReelNo.ToString().PadLeft(4, '0');
                                }
                                else
                                {
                                    slip1.FormattedNo = a + "-" + slip1.Date.ToDate().ToString("dd") + monthAsAlphabet + yearAsAlphabet + slip1.ReelNo.ToString().PadLeft(4, '0');

                                }
                                db.Execute("UPDATE Slip SET FormattedNo=@FormattedNo,ReelNo=@ReelNo  where Id=" + slip1.Id, slip1);
                                i++;
                            }
                        }

                    }


                }
            }
            catch (Exception ex)
            {

            }
            //return Content("Done");
            return Ok(new { Message = "Done" });
        }
        public IActionResult UpdateStock()
        {
            List<Slip> py = new List<Slip>();
            List<Stock> py1 = new List<Stock>();
            List<DispatchReturnMeta> py2 = new List<DispatchReturnMeta>();
            string ConnString = this.Configuration.GetConnectionString("MyConn");

            try
            {

                using (SQLiteConnection db = new SQLiteConnection(ConnString))
                {


                    db.Execute("delete from StockBook ");

                    py = db.Query<Slip>("select * from Slip").ToList();
                    py1 = db.Query<Stock>("select * from Stock").ToList();
                    py2 = db.Query<DispatchReturnMeta>("select * from DispatchReturnMeta").ToList();
                    foreach (Slip slip in py)
                    {
                            StockBook stockbook = new StockBook();
                            stockbook.Id = db.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                            stockbook.VoucherId = slip.Id;
                            stockbook.SlipId = slip.Id;
                            stockbook.Date = slip.Date;
                            stockbook.ReelNumber = slip.FormattedNo;
                            stockbook.Quality = slip.Quality;
                            stockbook.BF =slip.BF;
                            stockbook.GSM = slip.GSMId; 
                            stockbook.Size =slip.SizeId; 
                            stockbook.Unit = db.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId); 
                            stockbook.Godown = slip.GodownId; 
                            stockbook.NetWeight = slip.NetWeight;
                            stockbook.Shift = slip.Shift;
                            stockbook.ReelDia = slip.ReelDia;
                            stockbook.Quantity = 1;
                            stockbook.VoucherType = "Slip";
                            db.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia" +
                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia)", stockbook);
                    }
                    foreach (Stock stock in py1)
                    {
                        StockBook stockbook = new StockBook();
                        stockbook.Id = db.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                        stockbook.Date = stock.Date;
                        stockbook.VoucherId = stock.DispatchId;
                        stockbook.SlipId = stock.SlipId;
                        stockbook.ReelNumber = stock.Number;
                        stockbook.Quality = db.ExecuteScalar<int>("select Items.Id from  Items left join Slip on Slip.Quality=Items.Id  where Slip.Id= " + stock.SlipId);
                        stockbook.BF = db.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);
                        stockbook.GSM = db.ExecuteScalar<int>("select GSM.Id from  GSM left join Slip on Slip.GSMId=GSM.Id  where Slip.Id=  " + stock.SlipId); 
                        stockbook.Size = db.ExecuteScalar<int>("select Size.Id from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id=  " + stock.SlipId); 
                        stockbook.Unit = db.ExecuteScalar<string>("select Unit from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id= " + stock.SlipId); 
                        stockbook.NetWeight = (-1)*( db.ExecuteScalar<decimal>("select NetWeight from  Slip where Id= " + stock.SlipId)); //(-)
                        stockbook.Godown = stock.Godown;
                        stockbook.Quantity = -1;
                        stockbook.VoucherType = "Dispatch";

                        db.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                            ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);

                    }
                    foreach (DispatchReturnMeta stock in py2)
                    {
                        StockBook stockbook = new StockBook();
                        stockbook.Id = db.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook");
                        stockbook.Date = stock.Date;
                        stockbook.SlipId = stock.SlipId;
                        stockbook.VoucherId = stock.DispatchId;
                        stockbook.ReelNumber = stock.Number;
                        stockbook.Quality = db.ExecuteScalar<int>("select Items.Id from  Items left join Slip on Slip.Quality=Items.Id  where Slip.Id= " + stock.SlipId);
                        stockbook.BF = db.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);
                        stockbook.GSM = db.ExecuteScalar<int>("select GSM.Id from  GSM left join Slip on Slip.GSMId=GSM.Id  where Slip.Id=  " + stock.SlipId);
                        stockbook.Size = db.ExecuteScalar<int>("select Size.Id from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id=  " + stock.SlipId);
                        stockbook.Unit = db.ExecuteScalar<string>("select Unit from  Size left join Slip on Slip.SizeId=Size.Id  where Slip.Id= " + stock.SlipId);
                        stockbook.NetWeight = (db.ExecuteScalar<decimal>("select NetWeight from  Slip where Id= " + stock.SlipId)); 
                        stockbook.Godown = stock.Godown;
                        stockbook.Quantity = 1;
                        stockbook.VoucherType = "DispatchReturn";

                        db.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                            ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook);

                    }

                }
            }
            catch (Exception ex)
            {
                //return Content(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }
          //  return Content("done");
            return Ok(new { Message = "done" });

        }

    }
}
