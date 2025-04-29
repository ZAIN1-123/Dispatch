using Dapper;
using DISPATCHAPI.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
  

    public class TestPdfController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<TestPdfController> logger;
        private IWebHostEnvironment Environment;
        public TestPdfController(ILogger<TestPdfController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }




        //[AllowAnonymous]
        public IActionResult StockSummaryPrintZ()
        {
            int Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            List<StockBook> Report = new List<StockBook>();
            Business business = new Business();
            string ConnString = this.Configuration.GetConnectionString("MyConn");
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {

                //int Date = 20240830;
                //int Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                if (Date == 0)
                {
                    Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                }
                //string weight = conn.ExecuteScalar("Select SelectedValue from ControlRooms where Feature='Weight Required'").ToString();

                business = conn.Query<Business>("Select * from Business").FirstOrDefault();

                Report = conn.Query<StockBook>("SELECT CAST(ROUND(SUM(res.OQuantity), 2) AS DOUBLE) AS OpnQty, " +
                            "   CAST(ROUND(SUM(res.OWeight), 2) AS DOUBLE) AS OpnWht, CAST(ROUND(SUM(res.InQuantity), 2) AS DOUBLE) AS InwardQty, " +
                            "   CAST(ROUND(SUM(res.InWeight), 2) AS DOUBLE) AS InwardWht," +
                            "   CAST(ROUND(SUM(res.RQuantity), 2) AS DOUBLE) AS RepackQuantity, " +
                            "   CAST(ROUND(SUM(res.RWeight), 2) AS DOUBLE) AS RepackWeight,  " +
                            "    0 AS TotalQty, 0 AS TotalWht," +
                            "    CAST(ROUND(SUM(res.OutQuantity), 2) AS DOUBLE) AS OutwardQty, " +
                            "    CAST(ROUND(SUM(res.OutWeight), 2) AS DOUBLE) AS OutwardWht,  " +
                            "    0 AS Quantity, 0 AS Weight,      " +
                            "   res.Item" +
                            " FROM ( SELECT  CAST(SUM(StockBook.Quantity) AS DOUBLE) AS OQuantity, " +
                            " CAST(SUM(StockBook.NetWeight) AS DOUBLE) AS OWeight, " +
                            "  0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity,0 AS RWeight, 0 AS OutQuantity, " +
                            "  0 AS OutWeight,     Items.Name AS Item  " +
                            "   FROM StockBook     LEFT JOIN Slip ON StockBook.SlipId = Slip.Id  " +
                            "  LEFT JOIN Items ON Slip.Quality = Items.Id  WHERE StockBook.Date < " + Date + " AND StockBook.Quality IS NOT NULL " +
                            "    GROUP BY Items.Name  " +
                            "   HAVING SUM(StockBook.Quantity) > 0  " +
                            "   UNION ALL " +
                            "     SELECT " +
                            "     0 AS OQuantity,     0 AS OWeight,  " +
                            "  CAST(ROUND(SUM(CASE WHEN Slip.FormattedNo IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS InQuantity,  " +
                            "  CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS InWeight,  " +
                            "   0 AS RQuantity,  0 AS RWeight," +
                            "  0 AS OutQuantity,    0 AS OutWeight,    Items.Name AS Item " +
                            "  FROM Slip   LEFT JOIN Items ON Items.Id = Slip.Quality " +
                            "  WHERE Slip.Date >= " + Date + "    AND Slip.Date <= " + Date + " " +
                            "  AND Slip.Quality IS NOT NULL   " +
                            " AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 0) " +
                            "  GROUP BY Items.Name " +
                            " UNION ALL " +
                            " SELECT     0 AS OQuantity,     0 AS OWeight,  " +
                            "0 AS InQuantity,  0 AS InWeight, " +
                            " CAST(ROUND(SUM(CASE WHEN Slip.FormattedNo IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS RQuantity,  " +
                            " CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS RWeight, " +
                            "   0 AS OutQuantity,   " +
                            "   0 AS OutWeight,     Items.Name AS Item    FROM Slip " +
                            "   LEFT JOIN Items ON Items.Id = Slip.Quality     WHERE Slip.Date >= " + Date + "       AND Slip.Date <= " + Date + " " +
                            "   AND Slip.Quality IS NOT NULL   AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 1) GROUP BY Items.Name  " +
                            "   UNION ALL  " +
                            "  SELECT  0 AS OQuantity,    0 AS OWeight,   0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity," +
                            " 0 AS RWeight,  CAST(ROUND(SUM(CASE WHEN Stock.Serial IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS OutQuantity, " +
                            "  CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS OutWeight, " +
                            "  Items.Name AS Item " +
                            " FROM Dispatch" +
                            " LEFT JOIN Stock ON Stock.DispatchId = Dispatch.Id  LEFT JOIN Slip ON Stock.SlipId = Slip.Id  " +
                            "   LEFT JOIN Items ON Items.Id = Slip.Quality " +
                            "   WHERE Dispatch.Date >= " + Date + " " +
                            "     AND Dispatch.Date <= " + Date + "  GROUP BY Items.Name ) AS res WHERE res.Item IS NOT NULL GROUP BY res.Item;").ToList();





                foreach (var drcr in Report)
                {
                    drcr.TotalQty = drcr.OpnQty + drcr.InwardQty;
                    drcr.TotalWht = drcr.OpnWht + drcr.InwardWht;

                    //drcr.Quantity = drcr.OpnQty + drcr.InwardQty - drcr.OutwardQty;
                    //drcr.Weight = drcr.OpnWht + drcr.InwardWht - drcr.OutwardWht;
                    drcr.Quantity = Math.Max(0, drcr.OpnQty + drcr.InwardQty + drcr.RepackQuantity - drcr.OutwardQty);
                    drcr.Weight = Math.Max(0, drcr.OpnWht + drcr.InwardWht + drcr.RepackWeight - drcr.OutwardWht);


                }
            }

            iTextSharp.text.Font fontN = FontFactory.GetFont(FontFactory.HELVETICA, 8.8f);
            iTextSharp.text.Font fontN12 = FontFactory.GetFont(FontFactory.HELVETICA, 14);
            iTextSharp.text.Font fontB = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f);
            iTextSharp.text.Font fontBB = FontFactory.GetFont(FontFactory.HELVETICA, 16f);
            iTextSharp.text.Font font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f);

            MemoryStream stream = new MemoryStream();
            //Document pdfDoc = new Document(PageSize.A4, 10, 10, 15, 15);
            Document pdfDoc = new Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
            pdfDoc.Open();
            PdfContentByte pdf = pdfWriter.DirectContent;

            iTextSharp.text.Paragraph FirmName;
            PdfPTable table;
            PdfPCell cell;
            Paragraph header = new Paragraph();
            header.Alignment = Element.ALIGN_CENTER;

            table = new PdfPTable(11);
            float[] columnWidths = { 9f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f };
            table.SetWidths(columnWidths);
            table.SpacingBefore = 13;
            table.TotalWidth = 800; // Increased width for landscape
            table.LockedWidth = true;

            
            Document doc = new Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 20, 20);

            FirmName = new iTextSharp.text.Paragraph(business.Name, fontBB);
            FirmName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            FirmName.Font = fontBB;
            pdfDoc.Add(FirmName);
           

            Font headingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f);
            header.Add(new Phrase("Stock Summary\n ", new Font(Font.FontFamily.HELVETICA, 14)));
            //header.Add(Environment.NewLine);
            header.Add(new Phrase("From  " + Date.ToDate().ToString("dd-MMM-yyyy") + "  To  " + Date.ToDate().ToString("dd-MMM-yyyy"), new Font(Font.FontFamily.HELVETICA, 14)));
            // header.Add(Environment.NewLine);

            header.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(header);
            // Create the table headers
            PdfPCell textItem = new PdfPCell();
            textItem.HorizontalAlignment = Element.ALIGN_LEFT;
            textItem.FixedHeight = 20;
            textItem.Phrase = new Phrase("Quality\n", headingFont); // Split "Quality" across two lines
            table.AddCell(textItem);

            PdfPCell numTaxable = new PdfPCell();
            numTaxable.HorizontalAlignment = Element.ALIGN_CENTER;
            numTaxable.FixedHeight = 20;
            numTaxable.Phrase = new Phrase("Opening\n(Reels)", headingFont); // Split "Opening(Reels)" across two lines
            table.AddCell(numTaxable);

            PdfPCell num = new PdfPCell();
            num.HorizontalAlignment = Element.ALIGN_CENTER;
            num.FixedHeight = 20;
            num.Phrase = new Phrase("Opening\n(Weight)", headingFont); // Split "Opening(Weight)" across two lines
            table.AddCell(num);

            PdfPCell numCGST = new PdfPCell();
            numCGST.HorizontalAlignment = Element.ALIGN_CENTER;
            numCGST.FixedHeight = 20;
            numCGST.Phrase = new Phrase("Production\n(Reels)", headingFont); // Split "Production(Reels)" across two lines
            table.AddCell(numCGST);

            PdfPCell numSGST = new PdfPCell();
            numSGST.HorizontalAlignment = Element.ALIGN_CENTER;
            numSGST.FixedHeight = 20;
            numSGST.Phrase = new Phrase("Production\n(Weight)", headingFont); // Split "Production(Weight)" across two lines
            table.AddCell(numSGST);

            PdfPCell numAmountt = new PdfPCell();
            numAmountt.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmountt.FixedHeight = 20;
            numAmountt.Phrase = new Phrase("Repacking\n(Reels)", headingFont); // Split "Repacking(Reels)" across two lines
            table.AddCell(numAmountt);

            PdfPCell numAmountt1 = new PdfPCell();
            numAmountt1.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmountt1.FixedHeight = 20;
            numAmountt1.Phrase = new Phrase("Repacking\n(Weight)", headingFont); // Split "Repacking(Weight)" across two lines
            table.AddCell(numAmountt1);

            PdfPCell numAmount = new PdfPCell();
            numAmount.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmount.FixedHeight = 20;
            numAmount.Phrase = new Phrase("Dispatch\n(Reels)", headingFont); // Split "Dispatch(Reels)" across two lines
            table.AddCell(numAmount);

            PdfPCell numAmount1 = new PdfPCell();
            numAmount1.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmount1.FixedHeight = 20;
            numAmount1.Phrase = new Phrase("Dispatch\n(Weight)", headingFont); // Split "Dispatch(Weight)" across two lines
            table.AddCell(numAmount1);

            PdfPCell numAmount2 = new PdfPCell();
            numAmount2.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmount2.FixedHeight = 20;
            numAmount2.Phrase = new Phrase("Closing\n(Reels)", headingFont); // Split "Closing(Reels)" across two lines
            table.AddCell(numAmount2);

            PdfPCell numAmount3 = new PdfPCell();
            numAmount3.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmount3.FixedHeight = 20;
            numAmount3.Phrase = new Phrase("Closing\n(Weight)", headingFont); // Split "Closing(Weight)" across two lines
            table.AddCell(numAmount3);


            var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8.8f);
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9.2f); // Bold font for "Total"

            //foreach (var obj in Report.ToList().OrderBy(o=>o.Item))
            //{

            //    textItem.Phrase = new Phrase((obj as StockBook).Item ?? "", smallFont);
            //    numTaxable.Phrase = new Phrase((obj as StockBook).OpnQty.ToString(), smallFont);
            //    num.Phrase = new Phrase((obj as StockBook).OpnWht.ToString(), smallFont);
            //    numCGST.Phrase = new Phrase((obj as StockBook).InwardQty.ToString(), smallFont);
            //    numSGST.Phrase = new Phrase((obj as StockBook).InwardWht.ToString(), smallFont);
            //    numAmountt.Phrase = new Phrase((obj as StockBook).RepackQuantity.ToString(), smallFont);
            //    numAmountt1.Phrase = new Phrase((obj as StockBook).RepackWeight.ToString(), smallFont);
            //    numAmount.Phrase = new Phrase((obj as StockBook).OutwardQty.ToString(), smallFont);
            //    numAmount1.Phrase = new Phrase((obj as StockBook).OutwardWht.ToString(), smallFont);
            //    numAmount2.Phrase = new Phrase((obj as StockBook).Quantity.ToString(), smallFont);
            //    numAmount3.Phrase = new Phrase((obj as StockBook).Weight.ToString(), smallFont);
            //    // Regular for others

            //    table.AddCell(textItem);
            //    table.AddCell(numTaxable);
            //    table.AddCell(num);
            //    table.AddCell(numCGST);
            //    table.AddCell(numSGST);
            //    table.AddCell(numAmountt);
            //    table.AddCell(numAmountt1);
            //    table.AddCell(numAmount);
            //    table.AddCell(numAmount1);
            //    table.AddCell(numAmount2);
            //    table.AddCell(numAmount3);
            //}

            // Initialize variables to store totals for each column
            double totalOpnQty = 0, totalOpnWht = 0, totalInwardQty = 0, totalInwardWht = 0;
            double totalRepackQty = 0, totalRepackWht = 0, totalOutwardQty = 0, totalOutwardWht = 0;
            double totalClosingQty = 0, totalClosingWht = 0;

            // Loop through the data and add rows while calculating totals
            foreach (var obj in Report.ToList().OrderBy(o => o.Item))
            {
                var stock = obj as StockBook; // Ensure object is of type StockBook

                // Add up totals
                // Change total variables to double
               
                totalOpnQty += (double)obj.OpnQty;
                totalOpnWht += (double)obj.OpnWht;
                totalInwardQty += (double)obj.InwardQty;
                totalInwardWht += (double)obj.InwardWht;
                totalRepackQty += (double)obj.RepackQuantity;
                totalRepackWht += (double)obj.RepackWeight;
                totalOutwardQty +=(double)obj.OutwardQty;
                totalOutwardWht +=(double)obj.OutwardWht;
                totalClosingQty +=(double)obj.Quantity;
                totalClosingWht +=(double)obj.Weight;


                // Add regular data rows
                textItem.Phrase = new Phrase(stock.Item ?? "", smallFont);
                numTaxable.Phrase = new Phrase(stock.OpnQty.ToString(), smallFont);
                num.Phrase = new Phrase(stock.OpnWht.ToString(), smallFont);
                numCGST.Phrase = new Phrase(stock.InwardQty.ToString(), smallFont);
                numSGST.Phrase = new Phrase(stock.InwardWht.ToString(), smallFont);
                numAmountt.Phrase = new Phrase(stock.RepackQuantity.ToString(), smallFont);
                numAmountt1.Phrase = new Phrase(stock.RepackWeight.ToString(), smallFont);
                numAmount.Phrase = new Phrase(stock.OutwardQty.ToString(), smallFont);
                numAmount1.Phrase = new Phrase(stock.OutwardWht.ToString(), smallFont);
                numAmount2.Phrase = new Phrase(stock.Quantity.ToString(), smallFont);
                numAmount3.Phrase = new Phrase(stock.Weight.ToString(), smallFont);

                table.AddCell(textItem);
                table.AddCell(numTaxable);
                table.AddCell(num);
                table.AddCell(numCGST);
                table.AddCell(numSGST);
                table.AddCell(numAmountt);
                table.AddCell(numAmountt1);
                table.AddCell(numAmount);
                table.AddCell(numAmount1);
                table.AddCell(numAmount2);
                table.AddCell(numAmount3);
            }

            // Add totals row at the end
            PdfPCell totalCell = new PdfPCell(new Phrase("Total", boldFont))
            {
                Colspan = 1, // Spanning only the "Quality" column
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 20
            };
            table.AddCell(totalCell); // Add the "Total" label cell

            // Add total values for each column
            table.AddCell(new PdfPCell(new Phrase(totalOpnQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalOpnWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalInwardQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalInwardWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalRepackQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalRepackWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalOutwardQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalOutwardWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalClosingQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalClosingWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

            // Add the table to the document
            pdfDoc.Add(table);
           

            //FirmName = new iTextSharp.text.Paragraph(" ");
            //FirmName.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
            //FirmName.Font = fontN;
            //pdfDoc.Add(FirmName);



            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] MyPdf = stream.ToArray();
            Utility.SendWhatsapp("", MyPdf, "", ConnString);
            return Ok("");



        }
        public byte[] StockSummaryPrint()
        {
            int Date = int.Parse(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"));
            List<StockBook> Report = new List<StockBook>();
            List<Business> business = new List<Business>();
            //string ConnString = "Data Source=App_data/Data.db;foreign Keys=true;";
            string ConnString = this.Configuration.GetConnectionString("MyConn");
            using (SQLiteConnection conn = new SQLiteConnection(ConnString))
            {

                //int Date = 20240830;
                //int Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                if (Date == 0)
                {
                    Date = int.Parse(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"));
                }
                //string weight = conn.ExecuteScalar("Select SelectedValue from ControlRooms where Feature='Weight Required'").ToString();

                business = conn.Query<Business>("Select * from Business").ToList();

                Report = conn.Query<StockBook>("SELECT CAST(ROUND(SUM(res.OQuantity), 2) AS DOUBLE) AS OpnQty, " +
                            "   CAST(ROUND(SUM(res.OWeight), 2) AS DOUBLE) AS OpnWht, CAST(ROUND(SUM(res.InQuantity), 2) AS DOUBLE) AS InwardQty, " +
                            "   CAST(ROUND(SUM(res.InWeight), 2) AS DOUBLE) AS InwardWht," +
                            "   CAST(ROUND(SUM(res.RQuantity), 2) AS DOUBLE) AS RepackQuantity, " +
                            "   CAST(ROUND(SUM(res.RWeight), 2) AS DOUBLE) AS RepackWeight,  " +
                            "    0 AS TotalQty, 0 AS TotalWht," +
                            "    CAST(ROUND(SUM(res.OutQuantity), 2) AS DOUBLE) AS OutwardQty, " +
                            "    CAST(ROUND(SUM(res.OutWeight), 2) AS DOUBLE) AS OutwardWht,  " +
                            "    0 AS Quantity, 0 AS Weight,      " +
                            "   res.Item" +
                            " FROM ( SELECT  CAST(SUM(StockBook.Quantity) AS DOUBLE) AS OQuantity, " +
                            " CAST(SUM(StockBook.NetWeight) AS DOUBLE) AS OWeight, " +
                            "  0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity,0 AS RWeight, 0 AS OutQuantity, " +
                            "  0 AS OutWeight,     Items.Name AS Item  " +
                            "   FROM StockBook     LEFT JOIN Slip ON StockBook.SlipId = Slip.Id  " +
                            "  LEFT JOIN Items ON Slip.Quality = Items.Id  WHERE StockBook.Date < " + Date + " AND StockBook.Quality IS NOT NULL " +
                            "    GROUP BY Items.Name  " +
                            "   HAVING SUM(StockBook.Quantity) > 0  " +
                            "   UNION ALL " +
                            "     SELECT " +
                            "     0 AS OQuantity,     0 AS OWeight,  " +
                            "  CAST(ROUND(SUM(CASE WHEN Slip.FormattedNo IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS InQuantity,  " +
                            "  CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS InWeight,  " +
                            "   0 AS RQuantity,  0 AS RWeight," +
                            "  0 AS OutQuantity,    0 AS OutWeight,    Items.Name AS Item " +
                            "  FROM Slip   LEFT JOIN Items ON Items.Id = Slip.Quality " +
                            "  WHERE Slip.Date >= " + Date + "    AND Slip.Date <= " + Date + " " +
                            "  AND Slip.Quality IS NOT NULL   " +
                            " AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 0) " +
                            "  GROUP BY Items.Name " +
                            " UNION ALL " +
                            " SELECT     0 AS OQuantity,     0 AS OWeight,  " +
                            "0 AS InQuantity,  0 AS InWeight, " +
                            " CAST(ROUND(SUM(CASE WHEN Slip.FormattedNo IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS RQuantity,  " +
                            " CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS RWeight, " +
                            "   0 AS OutQuantity,   " +
                            "   0 AS OutWeight,     Items.Name AS Item    FROM Slip " +
                            "   LEFT JOIN Items ON Items.Id = Slip.Quality     WHERE Slip.Date >= " + Date + "       AND Slip.Date <= " + Date + " " +
                            "   AND Slip.Quality IS NOT NULL   AND (Slip.SlipStatus IS NULL OR Slip.SlipStatus = 1) GROUP BY Items.Name  " +
                            "   UNION ALL  " +
                            "  SELECT  0 AS OQuantity,    0 AS OWeight,   0 AS InQuantity,  0 AS InWeight,   0 AS RQuantity," +
                            " 0 AS RWeight,  CAST(ROUND(SUM(CASE WHEN Stock.Serial IS NOT NULL THEN 1 ELSE 0 END), 2) AS DOUBLE) AS OutQuantity, " +
                            "  CAST(ROUND(SUM(CASE WHEN Slip.NetWeight > 0 THEN Slip.NetWeight ELSE 0 END), 2) AS DOUBLE) AS OutWeight, " +
                            "  Items.Name AS Item " +
                            " FROM Dispatch" +
                            " LEFT JOIN Stock ON Stock.DispatchId = Dispatch.Id  LEFT JOIN Slip ON Stock.SlipId = Slip.Id  " +
                            "   LEFT JOIN Items ON Items.Id = Slip.Quality " +
                            "   WHERE Dispatch.Date >= " + Date + " " +
                            "     AND Dispatch.Date <= " + Date + "  GROUP BY Items.Name ) AS res WHERE res.Item IS NOT NULL GROUP BY res.Item;").ToList();





                foreach (var drcr in Report)
                {
                    drcr.TotalQty = drcr.OpnQty + drcr.InwardQty;
                    drcr.TotalWht = drcr.OpnWht + drcr.InwardWht;

                    //drcr.Quantity = drcr.OpnQty + drcr.InwardQty - drcr.OutwardQty;
                    //drcr.Weight = drcr.OpnWht + drcr.InwardWht - drcr.OutwardWht;
                    drcr.Quantity = Math.Max(0, drcr.OpnQty + drcr.InwardQty + drcr.RepackQuantity - drcr.OutwardQty);
                    drcr.Weight = Math.Max(0, drcr.OpnWht + drcr.InwardWht + drcr.RepackWeight - drcr.OutwardWht);


                }
            }

            iTextSharp.text.Font fontN = FontFactory.GetFont(FontFactory.HELVETICA, 8.8f);
            iTextSharp.text.Font fontN12 = FontFactory.GetFont(FontFactory.HELVETICA, 14);
            iTextSharp.text.Font fontB = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f);
            iTextSharp.text.Font fontBB = FontFactory.GetFont(FontFactory.HELVETICA, 16f);
            iTextSharp.text.Font font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f);

            MemoryStream stream = new MemoryStream();
            //Document pdfDoc = new Document(PageSize.A4, 10, 10, 15, 15);
            Document pdfDoc = new Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
            pdfDoc.Open();
            PdfContentByte pdf = pdfWriter.DirectContent;

            iTextSharp.text.Paragraph FirmName;
            PdfPTable table;
            PdfPCell cell;
            Paragraph header = new Paragraph();
            header.Alignment = Element.ALIGN_CENTER;

            table = new PdfPTable(11);
            float[] columnWidths = { 9f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f };
            table.SetWidths(columnWidths);
            table.SpacingBefore = 13;
            table.TotalWidth = 800; // Increased width for landscape
            table.LockedWidth = true;


            Document doc = new Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 20, 20);

            FirmName = new iTextSharp.text.Paragraph(business.FirstOrDefault().Name, fontBB);
            FirmName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            FirmName.Font = fontBB;
            pdfDoc.Add(FirmName);


            Font headingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f);
            header.Add(new Phrase("Stock Summary\n ", new Font(Font.FontFamily.HELVETICA, 14)));
            //header.Add(Environment.NewLine);
            header.Add(new Phrase("From  " + Date.ToDate().ToString("dd-MMM-yyyy") + "  To  " + Date.ToDate().ToString("dd-MMM-yyyy"), new Font(Font.FontFamily.HELVETICA, 14)));
            // header.Add(Environment.NewLine);

            header.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(header);
            // Create the table headers
            PdfPCell textItem = new PdfPCell();
            textItem.HorizontalAlignment = Element.ALIGN_LEFT;
            textItem.FixedHeight = 20;
            textItem.Phrase = new Phrase("Quality\n", headingFont); // Split "Quality" across two lines
            table.AddCell(textItem);

            PdfPCell numTaxable = new PdfPCell();
            numTaxable.HorizontalAlignment = Element.ALIGN_CENTER;
            numTaxable.FixedHeight = 20;
            numTaxable.Phrase = new Phrase("Opening\n(Reels)", headingFont); // Split "Opening(Reels)" across two lines
            table.AddCell(numTaxable);

            PdfPCell num = new PdfPCell();
            num.HorizontalAlignment = Element.ALIGN_CENTER;
            num.FixedHeight = 20;
            num.Phrase = new Phrase("Opening\n(Weight)", headingFont); // Split "Opening(Weight)" across two lines
            table.AddCell(num);

            PdfPCell numCGST = new PdfPCell();
            numCGST.HorizontalAlignment = Element.ALIGN_CENTER;
            numCGST.FixedHeight = 20;
            numCGST.Phrase = new Phrase("Production\n(Reels)", headingFont); // Split "Production(Reels)" across two lines
            table.AddCell(numCGST);

            PdfPCell numSGST = new PdfPCell();
            numSGST.HorizontalAlignment = Element.ALIGN_CENTER;
            numSGST.FixedHeight = 20;
            numSGST.Phrase = new Phrase("Production\n(Weight)", headingFont); // Split "Production(Weight)" across two lines
            table.AddCell(numSGST);

            PdfPCell numAmountt = new PdfPCell();
            numAmountt.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmountt.FixedHeight = 20;
            numAmountt.Phrase = new Phrase("Repacking\n(Reels)", headingFont); // Split "Repacking(Reels)" across two lines
            table.AddCell(numAmountt);

            PdfPCell numAmountt1 = new PdfPCell();
            numAmountt1.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmountt1.FixedHeight = 20;
            numAmountt1.Phrase = new Phrase("Repacking\n(Weight)", headingFont); // Split "Repacking(Weight)" across two lines
            table.AddCell(numAmountt1);

            PdfPCell numAmount = new PdfPCell();
            numAmount.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmount.FixedHeight = 20;
            numAmount.Phrase = new Phrase("Dispatch\n(Reels)", headingFont); // Split "Dispatch(Reels)" across two lines
            table.AddCell(numAmount);

            PdfPCell numAmount1 = new PdfPCell();
            numAmount1.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmount1.FixedHeight = 20;
            numAmount1.Phrase = new Phrase("Dispatch\n(Weight)", headingFont); // Split "Dispatch(Weight)" across two lines
            table.AddCell(numAmount1);

            PdfPCell numAmount2 = new PdfPCell();
            numAmount2.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmount2.FixedHeight = 20;
            numAmount2.Phrase = new Phrase("Closing\n(Reels)", headingFont); // Split "Closing(Reels)" across two lines
            table.AddCell(numAmount2);

            PdfPCell numAmount3 = new PdfPCell();
            numAmount3.HorizontalAlignment = Element.ALIGN_CENTER;
            numAmount3.FixedHeight = 20;
            numAmount3.Phrase = new Phrase("Closing\n(Weight)", headingFont); // Split "Closing(Weight)" across two lines
            table.AddCell(numAmount3);


            var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8.8f);
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9.2f); // Bold font for "Total"

            double totalOpnQty = 0, totalOpnWht = 0, totalInwardQty = 0, totalInwardWht = 0;
            double totalRepackQty = 0, totalRepackWht = 0, totalOutwardQty = 0, totalOutwardWht = 0;
            double totalClosingQty = 0, totalClosingWht = 0;

            // Loop through the data and add rows while calculating totals
            foreach (var obj in Report.ToList().OrderBy(o => o.Item))
            {
                var stock = obj as StockBook; // Ensure object is of type StockBook

                // Add up totals
                // Change total variables to double

                totalOpnQty += (double)obj.OpnQty;
                totalOpnWht += (double)obj.OpnWht;
                totalInwardQty += (double)obj.InwardQty;
                totalInwardWht += (double)obj.InwardWht;
                totalRepackQty += (double)obj.RepackQuantity;
                totalRepackWht += (double)obj.RepackWeight;
                totalOutwardQty += (double)obj.OutwardQty;
                totalOutwardWht += (double)obj.OutwardWht;
                totalClosingQty += (double)obj.Quantity;
                totalClosingWht += (double)obj.Weight;


                // Add regular data rows
                textItem.Phrase = new Phrase(stock.Item ?? "", smallFont);
                numTaxable.Phrase = new Phrase(stock.OpnQty.ToString(), smallFont);
                num.Phrase = new Phrase(stock.OpnWht.ToString(), smallFont);
                numCGST.Phrase = new Phrase(stock.InwardQty.ToString(), smallFont);
                numSGST.Phrase = new Phrase(stock.InwardWht.ToString(), smallFont);
                numAmountt.Phrase = new Phrase(stock.RepackQuantity.ToString(), smallFont);
                numAmountt1.Phrase = new Phrase(stock.RepackWeight.ToString(), smallFont);
                numAmount.Phrase = new Phrase(stock.OutwardQty.ToString(), smallFont);
                numAmount1.Phrase = new Phrase(stock.OutwardWht.ToString(), smallFont);
                numAmount2.Phrase = new Phrase(stock.Quantity.ToString(), smallFont);
                numAmount3.Phrase = new Phrase(stock.Weight.ToString(), smallFont);

                table.AddCell(textItem);
                table.AddCell(numTaxable);
                table.AddCell(num);
                table.AddCell(numCGST);
                table.AddCell(numSGST);
                table.AddCell(numAmountt);
                table.AddCell(numAmountt1);
                table.AddCell(numAmount);
                table.AddCell(numAmount1);
                table.AddCell(numAmount2);
                table.AddCell(numAmount3);
            }

            // Add totals row at the end
            PdfPCell totalCell = new PdfPCell(new Phrase("Total", boldFont))
            {
                Colspan = 1, // Spanning only the "Quality" column
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 20
            };
            table.AddCell(totalCell); // Add the "Total" label cell

            // Add total values for each column
            table.AddCell(new PdfPCell(new Phrase(totalOpnQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalOpnWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalInwardQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalInwardWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalRepackQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalRepackWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalOutwardQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalOutwardWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalClosingQty.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(totalClosingWht.ToString(), boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

            // Add the table to the document
            pdfDoc.Add(table);


            //FirmName = new iTextSharp.text.Paragraph(" ");
            //FirmName.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
            //FirmName.Font = fontN;
            //pdfDoc.Add(FirmName);



            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] MyPdf = stream.ToArray();
            //Utility.SendWhatsapp("", MyPdf, "", ConnString);
            return stream.ToArray();






        }

    }
}
