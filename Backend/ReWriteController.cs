using Dapper;
using DISPATCHAPI.Models;
using iTextSharp.text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    public class ReWriteController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<ReWriteController> logger;


        public ReWriteController(ILogger<ReWriteController> _logger, IConfiguration _Configuration)
        {
            logger = _logger;
            Configuration = _Configuration;
        }


        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {

            string ConnString = this.Configuration.GetConnectionString("MyConn");

            try
            {
                switch (id)
                {
                    case "StockBookResave":
                        return StockBookResave(ConnString);

                    case "DispatchResave":
                        int Id = int.Parse(Request.Headers["id"].FirstOrDefault());
                        return DispatchResave(ConnString, Id);


                }
                return Ok(new { Message = "Success" });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = "Not found , " + ex.Message });
            }
        }



        public ActionResult StockBookResave(string ConnString)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute("Delete from Stockbook", "", transaction);
                        string auth = Request.Headers["auth"].FirstOrDefault();
                        string client = Request.Headers["ClientUSID"].FirstOrDefault();
                        List<Slip> slips = conn.Query<Slip>("Select * from Slip", "", transaction).ToList();
                        StockBook stockbook;
                        Slip slip;

                        int count = 0, count1 = 0;
                        foreach (Slip s in slips)
                        {
                            stockbook = new StockBook();
                            stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook", "", transaction);
                            stockbook.VoucherId = s.Id;
                            stockbook.SlipId = s.Id;
                            stockbook.Date = s.Date;
                            stockbook.ReelNumber = s.FormattedNo;
                            stockbook.Quality = s.Quality;
                            if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                            {

                                stockbook.BF = s.ReelDia;
                            }
                            else
                            {
                                stockbook.BF = s.BF;

                            }
                           // stockbook.BF = s.BF;
                            stockbook.GSM = s.GSMId;
                            stockbook.Size = s.SizeId;
                            stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + s.SizeId, "", transaction);
                            stockbook.Godown = s.GodownId;
                            stockbook.NetWeight = s.NetWeight;
                            stockbook.Quantity = 1;
                            stockbook.VoucherType = "Slip";
                            conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia" +
                                          ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia)", stockbook, transaction);

                            count++;
                        }


                        List<Dispatch> dispatches = conn.Query<Dispatch>("select * from Dispatch", "", transaction).ToList();

                        foreach (Dispatch dispatch in dispatches)
                        {
                            dispatch.stock = conn.Query<Stock>("select * from Stock where DispatchId = " + dispatch.Id, "", transaction).ToList();

                            count = 0;
                            foreach (Stock stock in dispatch.stock)
                            {
                                slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId, "", transaction).FirstOrDefault();


                                stockbook = new StockBook();
                                stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook", "", transaction);
                                stockbook.Date = dispatch.Date;
                                stockbook.VoucherId = dispatch.Id;
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
                                    stockbook.BF = slip.BF;// conn.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);
                                    //stockbook.BF = conn.ExecuteScalar<int>("select Bf.Id from  BF left join Slip on Slip.BF=BF.Id  where Slip.Id=  " + stock.SlipId);

                                }

                                //stockbook.BF = slip.BF;
                                stockbook.GSM = slip.GSMId;
                                stockbook.Size = slip.SizeId;
                                stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId, "", transaction);
                                stockbook.NetWeight = (-1) * slip.NetWeight;
                                stockbook.Godown = slip.GodownId;
                                stockbook.Quantity = -1;
                                stockbook.VoucherType = "Dispatch";

                                conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia" +
                               ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia)", stockbook, transaction);

                                count++;
                            }

                            count1++;
                        }

                        List<DispatchReturn> dispatchReturns = conn.Query<DispatchReturn>("select * from DispatchReturn", "", transaction).ToList();

                        count1 = 0;
                        foreach (DispatchReturn dispatchr in dispatchReturns)
                        {
                            dispatchr.stock = conn.Query<DispatchReturnMeta>("select * from DispatchReturnMeta where DispatchId = " + dispatchr.Id, "", transaction).ToList();
                            count = 0;
                            foreach (DispatchReturnMeta stock in dispatchr.stock)
                            {
                                slip = conn.Query<Slip>("select * from Slip where id= " + stock.SlipId, "", transaction).FirstOrDefault();

                                stockbook = new StockBook();
                                stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook", "", transaction);
                                stockbook.Date = dispatchr.Date;
                                stockbook.VoucherId = dispatchr.Id;
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
                                //stockbook.BF = slip.BF;
                                stockbook.GSM = slip.GSMId;
                                stockbook.Size = slip.SizeId;
                                stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId, "", transaction);
                                stockbook.NetWeight = slip.NetWeight;
                                stockbook.Godown = slip.GodownId;
                                stockbook.Quantity = 1;
                                stockbook.VoucherType = "DispatchReturn";

                                conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId,Shift,ReelDia" +
                                ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId,@Shift,@ReelDia)", stockbook, transaction);
                                count++;
                            }

                            count1++;
                        }


                        List<GodownTransfer> godownTransfers = conn.Query<GodownTransfer>("select * from GodownTransfer", "", transaction).ToList();

                        count1 = 0;
                        foreach (GodownTransfer godownTransfer in godownTransfers)
                        {
                            godownTransfer.godownmeta = conn.Query<GodownTransferMeta>("select * from GodownTransferMeta where GodownTransferId = " + godownTransfer.Id, "", transaction).ToList();

                            count = 0;
                            foreach (GodownTransferMeta gm in godownTransfer.godownmeta)
                            {
                                slip = conn.Query<Slip>("select * from Slip where id= " + gm.SlipId, "", transaction).FirstOrDefault();

                                stockbook = new StockBook();
                                stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook", "", transaction);
                                stockbook.VoucherId = godownTransfer.Id;
                                stockbook.Date = godownTransfer.Date;
                                stockbook.SlipId = slip.Id;
                                stockbook.ReelNumber = slip.FormattedNo;
                                stockbook.Quality = slip.Quality;
                                stockbook.BF = slip.BF;
                                stockbook.GSM = slip.GSMId;
                                stockbook.Size = slip.SizeId;
                                stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId, "", transaction);
                                stockbook.Godown = godownTransfer.FromGodown;
                                stockbook.NetWeight = -1 * slip.NetWeight;
                                stockbook.Quantity = -1;
                                stockbook.VoucherType = "GodownTransfer";
                                conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                            ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook, transaction);


                                stockbook = new StockBook();
                                stockbook.Id = conn.ExecuteScalar<int>("Select ifnull(max(Id),0)+1 from stockbook", "", transaction);
                                stockbook.VoucherId = godownTransfer.Id;
                                stockbook.Date = godownTransfer.Date;
                                stockbook.SlipId = slip.Id;
                                stockbook.ReelNumber = slip.FormattedNo;
                                stockbook.Quality = slip.Quality;
                                stockbook.BF = slip.BF;
                                stockbook.GSM = slip.GSMId;
                                stockbook.Size = slip.SizeId;
                                stockbook.Unit = conn.ExecuteScalar<string>("select Unit from  Size where Id= " + slip.SizeId, "", transaction);
                                stockbook.Godown = godownTransfer.ToGodown;
                                stockbook.NetWeight = slip.NetWeight;
                                stockbook.Quantity = 1;
                                stockbook.VoucherType = "GodownTransfer";
                                conn.Execute("insert into StockBook(Id,VoucherId,Date,ReelNumber,Quality,BF,GSM,Size,Unit,NetWeight,Quantity,Godown,VoucherType,SlipId" +
                                            ") values(@Id,@VoucherId,@Date,@ReelNumber,@Quality,@BF,@GSM,@Size,@Unit,@NetWeight,@Quantity,@Godown,@VoucherType,@SlipId)", stockbook, transaction);
                                count++;
                            }

                            count1++;
                        }

                        transaction.Commit();
                        return Ok( "Success" );
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok( "Failed" );
                    }
                }
            }
        }


        public ActionResult DispatchResave(string ConnString, int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {
                Dispatch lst = new Dispatch();
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //  List<Dispatch> dispatch = conn.Query<Dispatch>("select Dispatch.* , Count(Stock.Serial) as Serial , Sum(Slip.NetWeight) as NetWeight " +
                        //"from Dispatch left join Stock on Stock.DispatchId= Dispatch.Id" +
                        // " left join Slip on  Stock.SlipId=Slip.Id " +
                        //" where  group by Dispatch.Id").ToList();

                        List<Stock> stock = conn.Query<Stock>("Select * from Stock Where DispatchId=" + id).ToList();
                        List<StockBook> stockbook = conn.Query<StockBook>("Select * from StockBook Where VoucherId=" + id +" and VoucherType='Dispatch'").ToList();
                        //List<Dispatch> dispatch = conn.Query<Dispatch>("select Dispatch.* , Stock.Serial " +
                        //    " from Dispatch left join Stock on Stock.DispatchId = Dispatch.Id " +
                        //    " left join Slip on  Stock.SlipId = Slip.Id where Dispatch.Id =" + id).ToList();

                        int Date = conn.ExecuteScalar<int>("Select Date from Dispatch where Id = "+id);

                        int serial = 1;
                        foreach (Stock obj in stock)
                        {
                            conn.Execute("Update Stock set Date = " + Date + " where Stock.DispatchId = " + id + " and Stock.Id= " + obj.Id);
                            //conn.Execute("Update Stock set Serial = " + serial + " where Stock.DispatchId = " + id + " and Stock.Id= " + obj.Id);
                            serial++;
                        }
                        foreach (StockBook obj in stockbook)
                        {
                            conn.Execute("Update StockBook set Date = " + Date + " where StockBook.VoucherId = " + id + " and StockBook.Id= " + obj.Id+ " and StockBook.VoucherType='Dispatch'");
                            //conn.Execute("Update Stock set Serial = " + serial + " where Stock.DispatchId = " + id + " and Stock.Id= " + obj.Id);
                            serial++;
                        }
                        transaction.Commit();
                        return Ok("Success");
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok("Failed");
                    }
                }

            }
        }
    }
}
