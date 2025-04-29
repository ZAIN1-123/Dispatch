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
using System.Threading.Tasks;
using static iTextSharp.awt.geom.Point2D;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class ReportController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<ReportController> logger;
        private IWebHostEnvironment Environment;
        public ReportController(ILogger<ReportController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet]
        public IActionResult stock(int Fdate, int Tdate, int GodownId, int FormattedNo, int BF, int GSM, int Size, int ReelDia, int Quality, string location, string LocationId, string Alias)
        {
            string where = " ";
            string bundlewhere = " ";
            string Lwhere = "1=1";
            try
            {
                List<Slip> lst1 = new List<Slip>();
                List<Slip> Rlst = new List<Slip>();
                List<Dispatch> lst2 = new List<Dispatch>();
                List<StockBook> lst = new List<StockBook>();
                List<BundleStockBook> bunldlelst = new List<BundleStockBook>();
                List<StockBook> Locationlist = new List<StockBook>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    int fdt = 0, tdt = 0;
                    if (Alias == "LocationReport")
                    {

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

                        if (GodownId != 0)
                        {
                            Lwhere = " and( Slip.GodownId =" + GodownId + ") ";
                        }
                        if (FormattedNo != 0)
                        {
                            Lwhere += " and (Slip.Id=" + FormattedNo + " ) ";
                        }
                        if (BF != 0)
                        {
                            Lwhere += " and (Slip.BF=" + BF + " ) ";
                        }
                        if (GSM != 0)
                        {
                            Lwhere += " and (Slip.GSMId=" + GSM + " ) ";
                        }
                        if (Size != 0)
                        {
                            Lwhere += " and (Slip.SizeId=" + Size + " ) ";
                        }
                        if (Quality != 0)
                        {
                            Lwhere += " and (Slip.Quality=" + Quality + " ) ";
                        }
                        if (ReelDia != 0)
                        {
                            Lwhere += " and (Slip.ReelDia=" + ReelDia + " ) ";
                        }
                        if (location != null)
                        {
                            if (location == "1")
                            {
                                Lwhere += "  AND (GodownLocation.Name IS NULL OR GodownLocation.Name = 'Not Allotted')   ";
                            }
                            else if (location == "0")
                            {
                                Lwhere += "  AND GodownLocation.Name IS NOT NULL ";

                            }

                            //else
                            //{
                            //    where += " and (Slip.SlipStatus=" + Status + " ) ";

                            //}


                        }
                        if (LocationId != null)
                        {
                            Lwhere += " AND GodownLocation.Id IN (" + LocationId + ")";
                        }
                    }
                    else if (Alias != "ProductionReport" && Alias != "RepackingReport" && Alias != "LocationReport" && Alias != "StockSummary" && Alias != "Stock" && Alias != "ConsolidateStock" && Alias!="BundleStock" && Alias!="ConsolidateBundleStock")
                    {
                        if (Fdate != 0 && Tdate != 0)
                        {
                            fdt = Fdate;
                            tdt = Tdate;

                            where = " (StockBook.Date >= " + fdt + " and StockBook.Date <= " + tdt + ")  ";
                        }

                        if (GodownId != 0)
                        {

                            where += " and ( StockBook.Godown =" + GodownId + ") ";

                        }
                        if (ReelDia != 0)
                        {

                            where += " and ( StockBook.BF =" + ReelDia + ") ";

                        }
                        if (FormattedNo != 0)
                        {

                            where += " and (StockBook.SlipId=" + FormattedNo + " ) ";

                        }
                        if (BF != 0)
                        {

                            where += " and (StockBook.BF=" + BF + " ) ";
                        }
                        if (GSM != 0)
                        {
                            where += " and (StockBook.GSM=" + GSM + " ) ";
                        }
                        if (Size != 0)
                        {
                            where += " and (StockBook.Size=" + Size + " ) ";
                        }
                        if (Quality != 0)
                        {
                            where += " and (StockBook.Quality=" + Quality + " ) ";
                        }

                        if (Alias == "DispatchMeta")
                        {
                            where += " and StockBook.VoucherType='Dispatch' ";
                        }
                    }

                    else if (Alias != "StockSummary" && Alias!="BundleStock" && Alias !="ConsolidateBundleStock")
                    {

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
                        if (Alias != "ProductionReport" && Alias != "RepackingReport" && Alias!="BundleStock" && Alias!="ConsolidateBundleStock")
                        {
                            where = " (StockBook.Date >= " + fdt + " and StockBook.Date <= " + tdt + ")  ";
                            bundlewhere = " (BundleStockBook.Date >= " + fdt + " and BundleStockBook.Date <= " + tdt + ")  ";
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

                    }
                    if(Alias=="BundleStock" || Alias=="ConsolidateBundleStock")
                    {
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
                        if (Alias == "BundleStock" || Alias == "ConsolidateBundleStock")
                        {                           
                            bundlewhere = " (BundleStockBook.Date >= " + fdt + " and BundleStockBook.Date <= " + tdt + ")  ";
                        }
                        if (FormattedNo != 0)
                        {
                            bundlewhere += " and (Bundle.Id=" + FormattedNo + " ) ";
                        }                        
                        if (GSM != 0)
                        {
                            bundlewhere += " and (Bundle.GSMId=" + GSM + " ) ";
                        }
                        if (Size != 0)
                        {
                            bundlewhere += " and (Bundle.BundleSizeId=" + Size + " ) ";
                        }
                        if (Quality != 0)
                        {
                            bundlewhere += " and (Bundle.Quality=" + Quality + " ) ";
                        }
                    }



                    if (Alias == "Stock")
                    {

                        //lst = conn.Query<StockBook>(
                        //    " select StockBook.*, StockBook.ReelNumber, StockBook.date, StockBook.NetWeight, " +
                        //    "Items.Name as ItemName, BF.Name as BFName, GSM.Name as GSMName, (Size.Name || ' ' || Size.Unit) as SizeName" +
                        //    " from StockBook left join Items on StockBook.Quality = Items.Id " +
                        //    "left join BF on StockBook.BF = BF.Id left join GSM on StockBook.GSM = GSM.Id " +
                        //    "left join Size on StockBook.Size = Size.Id where  " + where+
                        //    " order by StockBook.Date").ToList();

                        lst = conn.Query<StockBook>("select Slip.FormattedNo as Reelnumber,StockBook.SlipId,Items.Name as ItemName,BF.Name as BFName,GSM.Name as GSMName, (Size.Name || ' ' || Size.Unit) " +
                     "as SizeName , Slip.SetNo as RollNo,ReelDia.Name as ReelDiaName, Sum(StockBook.Quantity) as Qty,Sum(StockBook.Cutter) as Cut, Cast(sum(Stockbook.NetWeight) as double ) as TotalWeight from StockBook   " +
                     "left join Slip on StockBook.SlipId = Slip.Id left join Items on Slip.Quality = Items.Id left join BF on Slip.BF = BF.Id left join GSM on Slip.GSMId = GSM.Id left join ReelDia on StockBook.ReelDia = ReelDia.Id  " +
                     "left join Size on Slip.SizeId = Size.Id where " + where + " and StockBook.VoucherType<>'Cutter'  group by Slip.FormattedNo, StockBook.SlipId, Items.Name, BF.Name, GSM.Name, " +
                     "(Size.Name || ' ' || Size.Unit), Slip.SetNo having Sum(StockBook.Quantity) > 0  order by StockBook.Date").ToList();//and Sum(StockBook.Cutter) >0

                        //stockbook
                        // lst = conn.Query<StockBook>(
                        //  "select Stockbook.reelnumber,Cast(Sum(Quantity) as double ) as Qty,Cast(sum(Stockbook.NetWeight) as double ) as TotalWeight,Stockbook.Godown, StockBook.SlipId,Items.Name as ItemName,BF.Name as BFName, " +
                        //  "GSM.Name as GSMName,ReelDia.Name as ReelDiaName, (Size.Name || ' ' || Size.Unit) as SizeName ,  Slip.SetNo as RollNo from StockBook left join Items on StockBook.Quality = Items.Id " +
                        //  "left join Slip on StockBook.SlipId = Slip.Id left join BF on StockBook.BF = BF.Id left join GSM on StockBook.GSM = GSM.Id left join ReelDia on StockBook.ReelDia = ReelDia.Id " +
                        //  " left join Size on StockBook.Size = Size.Id where " + where +
                        //  "  group by Stockbook.reelnumber, StockBook.SlipId, StockBook.Quality, StockBook.BF, StockBook.GSM, StockBook.Size " +
                        //  "having Sum(Quantity) > 0 order by StockBook.Date").ToList();


                    }
                    
                    if (Alias == "BundleStock")
                    {


                        bunldlelst = conn.Query<BundleStockBook>("select Bundle.FormattedNo as BundleNumber,BundleStockBook.BundleId,Items.Name as ItemName,GSM.Name as GSMName," +
                            "(BundleSize.Name || ' ' || BundleSize.Unit) as SizeName ,Sum(BundleStockBook.Quantity) as Qty," +
                            "Cast(sum(BundleStockBook.BundleWeight) as double ) as TotalWeight from BundleStockBook " +
                            " left join Bundle on BundleStockBook.BundleId = Bundle.Id left join Items on Bundle.Quality = Items.Id " +
                            " left join GSM on Bundle.GSMId = GSM.Id " +
                            " left join BundleSize on Bundle.BundleSizeId = BundleSize.Id " +
                            " where  "+ bundlewhere + "  " +
                            " group by Bundle.FormattedNo, BundleStockBook.BundleId, Items.Name, GSM.Name, (BundleSize.Name || ' ' || BundleSize.Unit)" +
                            "  having Sum(BundleStockBook.Quantity) > 0  order by BundleStockBook.Date").ToList();//and Sum(StockBook.Cutter) >0

                    }


                    if (Alias == "LocationReport")
                    {
                        Locationlist = conn.Query<StockBook>(
                            "SELECT " +
                            "    Slip.FormattedNo AS Reelnumber, " +
                            "    StockBook.SlipId, " +
                            "    Items.Name AS ItemName, " +
                            "    BF.Name AS BFName, " +
                            "    GSM.Name AS GSMName, " +
                            "    (Size.Name || ' ' || Size.Unit) AS SizeName, " +
                            "    Slip.SetNo AS RollNo, " +
                            "    ReelDia.Name AS ReelDiaName, " +
                            "    SUM(StockBook.Quantity) AS Qty," +
                            "    Sum(StockBook.Cutter) as Cut, " +
                            "    CAST(SUM(StockBook.NetWeight) AS DOUBLE) AS TotalWeight, " +
                            "    COALESCE(GodownLocation.Name, 'Not Allotted') AS Location " +  // Use COALESCE for 'Not Allotted' in case Location is NULL
                            "FROM " +
                            "    StockBook " +
                            "    LEFT JOIN Slip ON StockBook.SlipId = Slip.Id " +
                            "    LEFT JOIN Items ON Slip.Quality = Items.Id " +
                            "    LEFT JOIN BF ON Slip.BF = BF.Id " +
                            "    LEFT JOIN GSM ON Slip.GSMId = GSM.Id " +
                            "    LEFT JOIN ReelDia ON StockBook.ReelDia = ReelDia.Id " +
                            "    LEFT JOIN Size ON Slip.SizeId = Size.Id " +
                            "    LEFT JOIN GodownVMaster ON StockBook.SlipId = GodownVMaster.SlipId " +  // Linking StockBook to GodownVMaster
                            "    LEFT JOIN GodownLocation ON GodownLocation.Id = GodownVMaster.LocationId " +  // Fetching Location from GodownLocation table
                            "WHERE " + Lwhere + " " +
                            //"WHERE " + Lwhere + " " +
                            " GROUP BY " +
                            "    Slip.FormattedNo, StockBook.SlipId, Items.Name, BF.Name, GSM.Name, " +
                            "    (Size.Name || ' ' || Size.Unit), Slip.SetNo, GodownLocation.Name " +
                            "HAVING " +
                            "    SUM(StockBook.Quantity) > 0 and Sum(StockBook.Cutter) > 0"


                        ).ToList();


                    }

                    if (Alias == "LocationReportConsolidate")
                    {
                        Locationlist = conn.Query<StockBook>(
                            "SELECT " +
                            " GodownLocation.Name AS Location, " +
                            "SUM(StockBook.Quantity) AS TotalQuantity," +
                            " Sum(StockBook.Cutter) as Cut," +
                            " CAST(SUM(StockBook.NetWeight) AS DOUBLE) AS TotalWeight, " +
                            " GROUP_CONCAT(DISTINCT COALESCE(Slip.FormattedNo, '')) AS ReelNumbers, " +
                            " GROUP_CONCAT(DISTINCT COALESCE(Items.Name, '')) AS ItemNames, " +
                            " GROUP_CONCAT(DISTINCT COALESCE(GSM.Name, '')) AS GSMNames,  " +
                            " GROUP_CONCAT(DISTINCT COALESCE((Size.Name || ' ' || Size.Unit), '')) AS SizeNames, " +
                            "  GROUP_CONCAT(DISTINCT COALESCE(Slip.SetNo, '')) AS RollNumbers, " +
                            "  GROUP_CONCAT(DISTINCT COALESCE(ReelDia.Name, '')) AS ReelDiaNames  FROM " +
                            "  StockBook " +
                            " LEFT JOIN Slip ON StockBook.SlipId = Slip.Id" +
                            " LEFT JOIN Items ON Slip.Quality = Items.Id " +
                            "  LEFT JOIN GSM ON Slip.GSMId = GSM.Id" +
                            "  LEFT JOIN ReelDia ON StockBook.ReelDia = ReelDia.Id " +
                            "  LEFT JOIN Size ON Slip.SizeId = Size.Id " +
                            "  LEFT JOIN GodownVMaster ON StockBook.SlipId = GodownVMaster.SlipId  " +
                            "  LEFT JOIN GodownLocation ON GodownLocation.Id = GodownVMaster.LocationId  " +
                            "WHERE " + Lwhere + " " +
                            " AND GodownLocation.Name IS NOT NULL  AND Slip.FormattedNo IS NOT NULL   " +
                            "  AND Items.Name IS NOT NULL   " +
                            "  AND GSM.Name IS NOT NULL   " +
                            "  AND Size.Name IS NOT NULL   " +
                            "AND ReelDia.Name IS NOT NULL " +
                            "GROUP BY   GodownLocation.Name   " +
                            "HAVING  SUM(StockBook.Quantity) > 0 and Sum(StockBook.Cutter)>0 ORDER BY TotalWeight DESC;  "
                        ).ToList();


                    }
                    else if (Alias == "ConsolidateStock")
                    {
                        //stockbook
                        //lst = conn.Query<StockBook>("  select Sum(StockBook.Quantity) as Qty, Sum(StockBook.NetWeight) as TotalWeight, " +
                        //    " Items.Name as ItemName, BF.Name as BFName,ReelDia.Name as ReelDiaName, GSM.Name as GSMName,Items.Id as Quality,GSM.Id as GSM, Size.Id as Size,BF.Id as BF, (Size.Name || ' ' || Size.Unit) as SizeName from StockBook  " +
                        //    " left join Items on StockBook.Quality = Items.Id left join BF on StockBook.BF = BF.Id left join ReelDia on StockBook.ReelDia = ReelDia.Id " +
                        //    " left  join GSM on StockBook.GSM = GSM.Id left  join Size on StockBook.Size = Size.Id  where " + where +
                        //    " group by StockBook.Quality, StockBook.BF, StockBook.GSM, StockBook.Size" +
                        //    " having Sum(Stockbook.Quantity) > 0  order by StockBook.Quality,StockBook.BF, StockBook.GSM, StockBook.Size").ToList();



                        lst = conn.Query<StockBook>(" select Items.Name as ItemName,BF.Name as BFName , GSM.Name as GSMName , Items.Id as Quality,GSM.Id as GSM, Size.Id as Size,BF.Id as BF, (Size.Name || ' ' || Size.Unit) as SizeName, ReelDia.Name as ReelDiaName,   " +
                           "Sum(StockBook.Quantity) as Qty,Sum(StockBook.Cutter) as Cut,Cast(sum(Stockbook.NetWeight) as double)as TotalWeight from StockBook left join Slip on StockBook.SlipId = Slip.Id left join Items on Slip.Quality = Items.Id " +
                           "left join BF on Slip.BF = BF.Id left join GSM on Slip.GSMId = GSM.Id left join Size on Slip.SizeId = Size.Id   left join ReelDia on StockBook.ReelDia = ReelDia.Id where " + where +
                           " and StockBook.VoucherType<>'Cutter' group by Items.Name, BF.Name, GSM.Name, (Size.Name || ' ' || Size.Unit) having Sum(StockBook.Quantity) > 0  order by StockBook.Quality,StockBook.BF, StockBook.GSM, StockBook.Size").ToList();



                        //lst = conn.Query<StockBook>(" select Items.Name as ItemName,BF.Name as BFName ,ReelDia.Name as ReelDiaName, GSM.Name as GSMName , Items.Id as Quality,GSM.Id as GSM, Size.Id as Size,BF.Id as BF, " +
                        //    "(Size.Name || ' ' || Size.Unit) as SizeName,Cast(Sum(Quantity) as double ) as Qty,Cast(sum(Stockbook.NetWeight) as double ) as TotalWeight  from StockBook left join Slip on " +
                        //    "StockBook.SlipId = Slip.Id left join Items on Slip.Quality = Items.Id left join ReelDia on StockBook.ReelDia = ReelDia.Id left join BF on Slip.BF = BF.Id left join GSM on Slip.GSMId = GSM.Id left join Size on " +
                        //    "Slip.SizeId = Size.Id where " + where + " group by Items.Name, BF.Name,ReelDia.Name , GSM.Name, (Size.Name || ' ' || Size.Unit) having Sum(StockBook.Quantity) > 0 " +
                        //    "order by StockBook.Quality,StockBook.BF,StockBook.ReelDia, StockBook.GSM, StockBook.Size").ToList();

                    }
                    else if (Alias == "ConsolidateBundleStock")
                    {

                        bunldlelst = conn.Query<BundleStockBook>(" select Items.Name as ItemName, GSM.Name as GSMName , Items.Id as Quality,GSM.Id as GSM, BundleSize.Id as Size," +
                            "(BundleSize.Name || ' ' || BundleSize.Unit) as SizeName,  Sum(BundleStockBook.Quantity) as Qty," +
                            " Cast(sum(BundleStockBook.BundleWeight) as double)as TotalWeight from BundleStockBook " +
                            " left join Bundle on BundleStockBook.BundleId = Bundle.Id" +
                            " left join Items on Bundle.Quality = Items.Id  " +
                            "  left join GSM on Bundle.GSMId = GSM.Id " +
                            "  left join BundleSize on Bundle.BundleSizeId = BundleSize.Id  " +
                            "  where  "+ bundlewhere +"  " +
                            "   group by Items.Name, GSM.Name, (BundleSize.Name || ' ' || BundleSize.Unit) having Sum(BundleStockBook.Quantity) > 0 " +
                            "   order by BundleStockBook.Quality, BundleStockBook.GSM, BundleStockBook.BundleSize").ToList();


                    }
                    else if (Alias == "ConsolidateStock2" || Alias == "QualityReport")
                    {
                        lst = conn.Query<StockBook>("select Cast(Sum(Quantity) as double )as Qty, Cast(sum(Stockbook.NetWeight) as double)as TotalWeight, Items.Name as ItemName,items.Rate as Rate,Cast((Items.Rate*sum(NetWeight))as double) as Amount  " +
                            "from StockBook  left join Items on StockBook.Quality = Items.Id    " +
                            "where " + where + "   group by StockBook.Quality, StockBook.BF, StockBook.GSM, StockBook.Size having " +
                            "Sum(Stockbook.Quantity) > 0 and Sum(StockBook.Cutter)>0 order by StockBook.Quality,StockBook.BF, StockBook.GSM, StockBook.Size").ToList();
                    }

                    else if (Alias == "ProductionReport")
                    {
                        lst1 = conn.Query<Slip>("select Slip.quality,slip.ReelDia,slip.SizeId,Cast (Sum(Slip.NetWeight) as double ) as NetWeight,count(slip.FormattedNo) as ReelNum , Items.Name as ItemName, BF.Name as BFName, GSM.Name as GSM ,ReelDia.Name as ReelDiaName,Users.UserName as EnteredName , (Size.Name|| ' ' || Size.Unit) as Size ,Size.Unit as Unitname " +
                        "from Slip left join Items on Slip.Quality=Items.Id left join BF on Slip.BF=BF.Id left join ReelDia on Slip.ReelDia=ReelDia.Id left join Users on Slip.EntredBy=Users.Id left join GSM on Slip.GSMId=GSM.Id " +
                        "left join Size on Slip.SizeId=Size.Id" +
                        " where (Slip.Date>=" + fdt + " and Slip.Date <=" + tdt + ")  AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 0)    " + where + " group by ItemName, BF.Name, GSM.Name, (Size.Name || ' ' || Size.Unit) order by Slip.Id Desc").ToList();
                    }


                    else if (Alias == "StockSummary")
                    {


                        //lst = conn.Query<StockBook>("SELECT CAST(ROUND(SUM(res.OQuantity), 2) AS DOUBLE) AS OpnQty, " +
                        //"   CAST(ROUND(SUM(res.OWeight), 2) AS DOUBLE) AS OpnWht, CAST(ROUND(SUM(res.InQuantity), 2) AS DOUBLE) AS InwardQty, " +
                        //"   CAST(ROUND(SUM(res.InWeight), 2) AS DOUBLE) AS InwardWht," +
                        //"   CAST(ROUND(SUM(res.RQuantity), 2) AS DOUBLE) AS RepackQuantity, " +
                        //"   CAST(ROUND(SUM(res.RWeight), 2) AS DOUBLE) AS RepackWeight,  " +
                        //"    0 AS TotalQty, 0 AS TotalWht," +
                        //"    CAST(ROUND(SUM(res.OutQuantity), 2) AS DOUBLE) AS OutwardQty, " +
                        //"    CAST(ROUND(SUM(res.OutWeight), 2) AS DOUBLE) AS OutwardWht,  " +
                        //"    0 AS Quantity, 0 AS Weight,      " +
                        //"   res.Item" +
                        //" FROM ( SELECT  CAST(SUM(StockBook.Quantity) AS DOUBLE) AS OQuantity, " +
                        //" CAST(SUM(StockBook.NetWeight) AS DOUBLE) AS OWeight, " +
                        //"  0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity,0 AS RWeight, 0 AS OutQuantity, " +
                        //"  0 AS OutWeight,     Items.Name AS Item  " +
                        //"   FROM StockBook     left join Slip on StockBook.SlipId = Slip.Id left join Items on Slip.Quality = Items.Id " +
                        //" left join BF on StockBook.BF = BF.Id left join GSM on StockBook.GSM = GSM.Id " +
                        //" left join ReelDia on StockBook.ReelDia = ReelDia.Id  left join Size on Slip.SizeId = Size.Id " +
                        //" WHERE StockBook.Date < " + Fdate + " AND StockBook.Quality IS NOT NULL  and StockBook.Cutter=0 " +
                        //"    GROUP BY Items.Name  " +
                        //"   HAVING SUM(StockBook.Quantity) > 0  " +
                        //"   UNION ALL " +
                        //"     SELECT " +
                        //"     0 AS OQuantity,     0 AS OWeight,  " +
                        //"  CAST(ROUND(SUM(CASE WHEN Slip.FormattedNo IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS InQuantity,  " +
                        //"  CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS InWeight,  " +
                        //"   0 AS RQuantity,  0 AS RWeight," +
                        //"  0 AS OutQuantity,    0 AS OutWeight,    Items.Name AS Item " +
                        //"  FROM Slip   LEFT JOIN Items ON Items.Id = Slip.Quality " +
                        //"  WHERE Slip.Date >= " + Fdate + "    AND Slip.Date <= " + Tdate +
                        //"  AND Slip.Quality IS NOT NULL   " +
                        //" AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 0) " +
                        //"  GROUP BY Items.Name " +
                        //" UNION ALL " +
                        //" SELECT     0 AS OQuantity,     0 AS OWeight,  " +
                        //"0 AS InQuantity,  0 AS InWeight, " +
                        //" CAST(ROUND(SUM(CASE WHEN Slip.FormattedNo IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS RQuantity,  " +
                        //" CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS RWeight, " +
                        //"   0 AS OutQuantity,   " +
                        //"   0 AS OutWeight,     Items.Name AS Item    FROM Slip " +
                        //"   LEFT JOIN Items ON Items.Id = Slip.Quality     WHERE Slip.Date >= " + Fdate + "       AND Slip.Date <= " + Tdate + " " +
                        //"   AND Slip.Quality IS NOT NULL   AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 1) GROUP BY Items.Name  " +
                        //"   UNION ALL " +
                        //"  SELECT  0 AS OQuantity,    0 AS OWeight,   0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity," +
                        //" 0 AS RWeight, Count(Stock.Serial) AS OutQuantity, " +
                        //" CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS OutWeight, " +
                        //" Items.Name AS Item  from Stock LEFT JOIN Slip ON Stock.SlipId = Slip.Id   " +
                        //"  LEFT JOIN Items ON Items.Id = Slip.Quality   WHERE " +
                        //" Stock.Date >= " + Fdate + " AND Stock.Date <= " + Tdate + "  GROUP BY Items.Name " +
                        //" ) AS res WHERE res.Item IS NOT NULL GROUP BY res.Item;").ToList();



                        lst = conn.Query<StockBook>("SELECT CAST(ROUND(SUM(res.OQuantity), 2) AS DOUBLE) AS OpnQty, " +
                            "   CAST(ROUND(SUM(res.OWeight), 2) AS DOUBLE) AS OpnWht," +
                            "CAST(ROUND(SUM(res.InQuantity), 2) AS DOUBLE) AS InwardQty,  " +
                            "  CAST(ROUND(SUM(res.InWeight), 2) AS DOUBLE) AS InwardWht, " +
                            " CAST(ROUND(SUM(res.RQuantity), 2) AS DOUBLE) AS RepackQuantity, " +
                            "   CAST(ROUND(SUM(res.RWeight), 2) AS DOUBLE) AS RepackWeight,  " +
                            " 0 AS TotalQty, 0 AS TotalWht,    CAST(ROUND(SUM(res.OutQuantity), 2) AS DOUBLE) AS OutwardQty,  " +
                            "   CAST(ROUND(SUM(res.OutWeight), 2) AS DOUBLE) AS OutwardWht, " +
                            " CAST(ROUND(SUM(res.IssueQuantity), 2) AS DOUBLE) AS IssueQty,  " +
                            " CAST(ROUND(SUM(res.IssueWeight), 2) AS DOUBLE) AS IssueWht,      0 AS Quantity, 0 AS Weight, " +
                            "   res.Item  " +
                            "   FROM ( SELECT  CAST(SUM(StockBook.Quantity) AS DOUBLE) AS OQuantity, " +
                            " CAST(SUM(StockBook.NetWeight) AS DOUBLE) AS OWeight," +
                            "   0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity,0 AS RWeight, 0 AS OutQuantity,  " +
                            " 0 AS OutWeight,0 AS IssueQuantity,   0 AS IssueWeight,     Items.Name AS Item  " +
                            "  FROM StockBook     left join Slip on StockBook.SlipId = Slip.Id left join Items on Slip.Quality = Items.Id  " +
                            "  left join BF on StockBook.BF = BF.Id left join GSM on StockBook.GSM = GSM.Id " +
                            "  left join ReelDia on StockBook.ReelDia = ReelDia.Id  left join Size on Slip.SizeId = Size.Id  " +
                            "  WHERE StockBook.Date < "+Fdate+ " AND StockBook.Quality IS NOT NULL and StockBook.VoucherType<>'Cutter'   GROUP BY Items.Name " +
                            "   HAVING SUM(StockBook.Quantity) > 0  UNION ALL SELECT      0 AS OQuantity,     0 AS OWeight, " +
                            "CAST(ROUND(SUM(CASE WHEN Slip.FormattedNo IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS InQuantity,  " +
                            "  CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS InWeight,     0 AS RQuantity,  0 AS RWeight," +
                            " 0 AS OutQuantity,    0 AS OutWeight, 0 AS IssueQuantity,   0 AS IssueWeight,    Items.Name AS Item   FROM Slip   LEFT JOIN Items ON Items.Id = Slip.Quality " +
                            " WHERE Slip.Date >= "+Fdate+"    AND Slip.Date <= "+Tdate+"   AND Slip.Quality IS NOT NULL  " +
                            "  AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 0)   GROUP BY Items.Name " +
                            " UNION ALL   SELECT     0 AS OQuantity,     0 AS OWeight,  0 AS InQuantity,  0 AS InWeight,  " +
                            "CAST(ROUND(SUM(CASE WHEN Slip.FormattedNo IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS RQuantity, " +
                            " CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS RWeight," +
                            "  0 AS OutQuantity,      0 AS OutWeight, 0 AS IssueQuantity,   0 AS IssueWeight,     Items.Name AS Item    FROM Slip    LEFT JOIN Items ON Items.Id = Slip.Quality " +
                            "  WHERE Slip.Date >= "+Fdate+"        AND Slip.Date <= "+Tdate+"    AND Slip.Quality IS NOT NULL   AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 1) " +
                            " GROUP BY Items.Name  UNION ALL        SELECT  0 AS OQuantity,    0 AS OWeight, " +
                            "   0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity, 0 AS RWeight, Count(Stock.Serial) AS OutQuantity," +
                            " CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS OutWeight,0 AS IssueQuantity,   0 AS IssueWeight, " +
                            " Items.Name AS Item  from Stock LEFT JOIN Slip ON Stock.SlipId = Slip.Id   " +
                            " LEFT JOIN Items ON Items.Id = Slip.Quality   WHERE  Stock.Date >= "+Fdate+"  AND Stock.Date <= "+Tdate+"  " +
                            "  GROUP BY Items.Name  Union all    SELECT  0 AS OQuantity,    0 AS OWeight, " +
                            "0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity, 0 AS RWeight,0 AS IssueQuantity, " +
                            "  0 AS IssueWeight,Count(CutterVMaster.Serial) AS IssueQuantity, " +
                            "CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS IssueWeight," +
                            "Items.Name AS Item  from CutterVMaster LEFT JOIN Slip ON CutterVMaster.SlipId = Slip.Id  " +
                            "LEFT JOIN Items ON Items.Id = Slip.Quality   WHERE  CutterVMaster.Date >= "+Fdate+"  AND CutterVMaster.Date >= "+Tdate+"   " +
                            " GROUP BY Items.Name  ) AS res WHERE res.Item IS NOT NULL GROUP BY res.Item;").ToList();



                        foreach (var drcr in lst)
                        {
                            drcr.TotalQty = drcr.OpnQty + drcr.InwardQty;
                            drcr.TotalWht = drcr.OpnWht + drcr.InwardWht;

                            drcr.Quantity = drcr.TotalQty + drcr.RepackQuantity - drcr.OutwardQty-drcr.IssueQty;
                            drcr.Weight = drcr.TotalWht + drcr.RepackWeight - drcr.OutwardWht-drcr.IssueWht;
                            //drcr.Quantity = Math.Max(0, drcr.TotalQty + drcr.RepackQuantity - drcr.OutwardQty);
                            //drcr.Weight = Math.Max(0, drcr.TotalWht + drcr.RepackWeight - drcr.OutwardWht);
                        }
                    }

                    else if (Alias == "RepackingReport")
                    {
                        Rlst = conn.Query<Slip>("select Slip.quality,slip.ReelDia,slip.SizeId,Cast (Sum(Slip.NetWeight) as double ) as NetWeight,count(slip.FormattedNo) as ReelNum , Items.Name as ItemName, BF.Name as BFName, GSM.Name as GSM ,ReelDia.Name as ReelDiaName,Users.UserName as EnteredName , (Size.Name|| ' ' || Size.Unit) as Size ,Size.Unit as Unitname " +
                        "from Slip left join Items on Slip.Quality=Items.Id left join BF on Slip.BF=BF.Id left join ReelDia on Slip.ReelDia=ReelDia.Id left join Users on Slip.EntredBy=Users.Id left join GSM on Slip.GSMId=GSM.Id " +
                        "left join Size on Slip.SizeId=Size.Id" +
                        " where (Slip.Date>=" + fdt + " and Slip.Date <=" + tdt + ")  AND (Slip.SlipStatus = 1) " + where + " group by ItemName, BF.Name, GSM.Name, (Size.Name || ' ' || Size.Unit) order by Slip.Id Desc").ToList();
                    }
                    else if (Alias == "DispatchMeta")
                    {

                        lst = conn.Query<StockBook>(" select Items.Name as ItemName,BF.Name as BFName , GSM.Name as GSMName , Items.Id as Quality,GSM.Id as GSM, Size.Id as Size,BF.Id as BF, (Size.Name || ' ' || Size.Unit) as SizeName, ReelDia.Name as ReelDiaName,   " +
                          "SUM(ABS(StockBook.Quantity)) AS Qty,CAST(SUM(ABS(Stockbook.NetWeight)) AS DOUBLE) AS TotalWeight  from StockBook left join Slip on StockBook.SlipId = Slip.Id left join Items on Slip.Quality = Items.Id " +
                          "left join BF on Slip.BF = BF.Id left join GSM on Slip.GSMId = GSM.Id left join Size on Slip.SizeId = Size.Id   left join ReelDia on StockBook.ReelDia = ReelDia.Id where " + where +
                          " group by Items.Name, BF.Name, GSM.Name, (Size.Name || ' ' || Size.Unit)  order by StockBook.Quality,StockBook.BF, StockBook.GSM, StockBook.Size").ToList();



                        //lst2 = conn.Query<Dispatch>("select Dispatch.* , Count(Stock.Serial) as Serial , Cast(sum(Slip.NetWeight) as double ) as NetWeight " +
                        //    "from Dispatch left join Stock on Stock.DispatchId= Dispatch.Id" +
                        //     " left join Slip on  Stock.SlipId=Slip.Id " +
                        //    " where Dispatch.Date>=" + fdt + " and Dispatch.Date<=" + tdt + " group by Dispatch.Id").ToList();
                    }

                    if (Alias == "ProductionReport")
                    {
                        return Ok(lst1);
                    }
                    else if (Alias == "RepackingReport")
                    {
                        return Ok(Rlst);
                    }
                    else if (Alias == "LocationReport" || Alias == "LocationReportConsolidate")
                    {
                        return Ok(Locationlist);
                    }
                    else if (Alias == "DispatchMeta")
                    {
                        return Ok(lst);
                    }

                    else if (Alias == "StockSummary")
                    {
                        return Ok(lst);
                    }
                    else if (Alias == "BundleStock" || Alias== "ConsolidateBundleStock")
                    {
                        return Ok(bunldlelst);
                    }
                    else
                    {
                        return Ok(lst);
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
