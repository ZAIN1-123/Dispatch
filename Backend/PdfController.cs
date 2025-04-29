using Dapper;
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
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class PdfController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<PdfController> logger;
        private IWebHostEnvironment Environment;
        public PdfController(ILogger<PdfController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }
        [HttpGet("{Id}")]
        public IActionResult Getch(int id)
        {

            string where = "";
            try
            {
                string client = Request.Headers["ClientUSID"].FirstOrDefault();
                List<Dispatch> itemprod = new List<Dispatch>();
                 Dispatch itemprod1 = new Dispatch();
                 Dispatch dispatch;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    //dispatch = conn.Query<Dispatch>("Update Dispatch set  Status=1  where Id=" + id).FirstOrDefault();
                    itemprod = conn.Query<Dispatch>("select Slip.*,Dispatch.*,Dispatch.Date as DispatchDate,Stock.*," +
                        "Items.Name as ItemName,BF.Name  as BFName,Items.ShortName as ShortName," +
                        " GSM.Name as GSM ,Users.UserName as EnteredName, ReelDia.Name as ReelDiaName , (Size.Name) as Size ,Size.Unit as Unitname " +
                        "from Dispatch " +
                        " left join Stock on Dispatch.Id=Stock.DispatchId" +
                        " left join Slip on  Stock.SlipId=Slip.Id " +
                        "left join ReelDia on Slip.ReelDia=ReelDia.Id " +
                        "left join Items on Slip.Quality=Items.Id " +
                        "left join BF on Slip.BF=BF.Id " +
                        "left join Users on Slip.EntredBy=Users.Id " +
                        "left join GSM on Slip.GSMId=GSM.Id " +
                        "left join Size on Slip.SizeId=Size.Id" +
                        " where Dispatch.Id=" + id + "  order by Items.Name desc ,GSM.Name asc , Size.Name asc ,BF.Name asc,Items.ShortName ").ToList();

                    Business business = conn.Query<Business>("select * from Business where id=1").FirstOrDefault();
                    itemprod1 = conn.Query<Dispatch>("select *,Dispatch.Date as DispatchDate from Dispatch where Id=" + id).FirstOrDefault();

                    iTextSharp.text.Font fontN = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                    iTextSharp.text.Font fontB = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                    iTextSharp.text.Font fontBR = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                    iTextSharp.text.Font font = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                    MemoryStream stream = new MemoryStream();
                    Document pdfDoc = new Document(PageSize.A4, 15, 15, 15, 15);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    
                    PdfContentByte pdf = pdfWriter.DirectContent;
                    iTextSharp.text.Paragraph report = new iTextSharp.text.Paragraph(business.Name);
                    report.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    report.Font = font;
                    pdfDoc.Add(report);

                    report = new iTextSharp.text.Paragraph(business.Address1 + " " + business.Address2);
                    report.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    report.Font = font;
                    pdfDoc.Add(report);

                    report = new iTextSharp.text.Paragraph("Dispatch ");
                    report.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    report.Font = font;
                    pdfDoc.Add(report);

                    MarwariPdf.DrawText(pdf, "bold", "", BaseColor.BLACK, 10, 0, "Date : " +( itemprod1.DispatchDate.ToDate().ToString("dd-MMM-yyyy")??"")
                        +"                                  Dispatch No. : "+(itemprod1.VNumber.ToString()??"")+ 
                        "                                  Vehicle No : "+ itemprod1.VehicleNo??"", 50, 750, 0);

                    MarwariPdf.DrawText(pdf, "bold", "", BaseColor.BLACK, 10, 0, " Remark : "+itemprod1.Remark??"", 50, 735, 0);
                    iTextSharp.text.Paragraph FirmName = new iTextSharp.text.Paragraph("\n\n");
                    FirmName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    FirmName.Font = fontN;
                    pdfDoc.Add(FirmName);
                    PdfPTable table = new PdfPTable(10);
                    float[] widths = new float[] { .3f, .9f, .6f, .6f, .6f, .6f, .6f, .9f, .3f, .6f };
                    table.SetWidths(widths);
                    table.SpacingBefore = 20;
                    table.TotalWidth = 560;
                    table.LockedWidth = true;

                    PdfPCell cell;

                    cell = new PdfPCell(new Phrase("S. NO.", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Reel No.", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("GSM", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                    {

                        cell = new PdfPCell(new Phrase("ReelDia", fontB));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase("BF", fontB));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                    }
                    
                    cell = new PdfPCell(new Phrase("Weight(KG)", fontB));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Size", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Unit", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Item Description", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Reel nos.", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Weight(KG)", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    int serial = 1;
                   
                    string previousSize = string.Empty;
                    string previousGSM = string.Empty;
                    string previousBF = string.Empty;
                    string previousshortname = string.Empty;
                   
                    foreach (Dispatch ob in itemprod)
                    {
                        //string count = itemprod.Count(o => o.Size == ob.Size && o.BFName == ob.BFName && o.GSM == ob.GSM && o.ShortName== ob.ShortName).ToString();
                        string count = itemprod.Count(o => o.Size == ob.Size && o.BFName == ob.BFName && o.GSM == ob.GSM && o.ShortName== ob.ShortName && o.ItemName==ob.ItemName).ToString();
                        //string net = itemprod.Where(o => o.Size == ob.Size && o.BFName == ob.BFName && o.GSM == ob.GSM && o.ShortName == ob.ShortName).Sum(o => o.NetWeight).ToString();
                        string net = itemprod.Where(o => o.Size == ob.Size && o.BFName == ob.BFName && o.GSM == ob.GSM && o.ShortName == ob.ShortName && o.ItemName==ob.ItemName).Sum(o => o.NetWeight).ToString();

                        cell = new PdfPCell(new Phrase(serial.ToString(), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize || ob.GSM!= previousGSM || ob.BFName !=previousBF ||  ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.FormattedNo, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.GSM, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        if (client.Contains("BHAGESHWARIDISPATCH") || client.Contains("MARWARIZAINAB"))
                        {
                            cell = new PdfPCell(new Phrase((ob.ReelDiaName)??"", fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.Size != previousSize || ob.GSM != previousGSM)
                            {
                                // Add a line when ob.Size changes
                                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                            }
                            else
                            {
                                cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                            }
                            table.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase((ob.BFName) + " " + (ob.ShortName), fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.Size changes
                                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                            }
                            else
                            {
                                cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                            }
                            table.AddCell(cell);
                        }
                        
                        cell = new PdfPCell(new Phrase(ob.NetWeight.ToString(), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.Size, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.Unitname, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.ItemName, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a cell with borders at the end of each group
                            cell = new PdfPCell(new Phrase(count, fontN)); // Show the group count
                            if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.Size changes
                                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                            }
                            else
                            {
                                cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                            }
                            
                            table.AddCell(cell);
                            // Reset the group count for the next group
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(" ", fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.Size changes
                                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                            }
                            else
                            {
                                cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                            }
                          
                            table.AddCell(cell);
                            
                        }
                        if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a cell with borders at the end of each group
                            cell = new PdfPCell(new Phrase(net, fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.Size changes
                                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                            }
                            else
                            {
                                cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                            }
                            table.AddCell(cell);
                            // Reset the group count for the next group
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(" ", fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.Size != previousSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.Size changes
                                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                            }
                            else
                            {
                                cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                           
                            }

                            table.AddCell(cell);

                        }
                      
                        
                        serial++;
                     
                        previousSize = ob.Size;
                        previousBF = ob.BFName;
                        previousGSM = ob.GSM;
                        previousshortname = ob.ShortName;

                    }

                    cell = new PdfPCell(new Phrase("Total", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(" ", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(itemprod.Count().ToString(), fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(itemprod.Sum(o => o.NetWeight).ToString() + " KG", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);

                    pdfDoc.Add(table);
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();

                    byte[] byteA = stream.ToArray();

                    return Ok(byteA);
                }
            }
            catch (Exception ex)
            {
                string a = "constraint failed\r\nFOREIGN KEY constraint failed";
                if (ex.Message == a)
                {
                    return BadRequest("this is not deleted");
                }
                else
                {
                    return BadRequest(ex.Message);
                }
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
                    string auth = Request.Headers["auth"].FirstOrDefault();
                    string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                    string[] credentials = decodedStrings.Split(':');
                    string username = credentials[0];
                    string password = credentials[1];

                    lst = conn.Query<Dispatch>("Update Dispatch set  Status=1  where Id=" + id,lst).FirstOrDefault();

                    return Ok("Updated");

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        public IActionResult StockSummaryPrint(int Date)
        {
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
                string weight = conn.ExecuteScalar("Select SelectedValue from ControlRooms where Feature='Weight Required'").ToString();

                business = conn.Query<Business>("Select * from Businesses").FirstOrDefault();               

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

            iTextSharp.text.Font fontN = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font fontN12 = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font fontB = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font fontBB = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font font = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            MemoryStream stream = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 15, 15);

            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
            pdfDoc.Open();
            PdfContentByte pdf = pdfWriter.DirectContent;

            iTextSharp.text.Paragraph FirmName;
            PdfPTable table;
            PdfPCell cell;


            float[] widths;


            table = new PdfPTable(11);
            widths = new float[] { 9f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f };

            table.SetWidths(widths);
            table.SpacingBefore = 20;
            table.TotalWidth = 560;
            table.LockedWidth = true;



            FirmName = new iTextSharp.text.Paragraph(business.Name, fontBB);
            FirmName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            FirmName.Font = fontBB;
            pdfDoc.Add(FirmName);         
            FirmName = new iTextSharp.text.Paragraph("Finish Goods Stock Summary", fontN12);
            FirmName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            FirmName.Font = fontN12;
            pdfDoc.Add(FirmName);
            FirmName = new iTextSharp.text.Paragraph("For the period of " + Date.ToDate().ToString("dd-MMM-yyyy") + " to " + Date.ToDate().ToString("dd-MMM-yyyy"), fontN12);
            //FirmName = new iTextSharp.text.Paragraph("For the period of 30-Aug-2024 to 30-Aug-2024", fontN12);
            FirmName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            FirmName.Font = fontN12;
            pdfDoc.Add(FirmName);



            cell = new PdfPCell(new Phrase("Quality\n", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = 1;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;           
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Opening\n(Reels)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = 1;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;            
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Opening\n(Weight)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;           
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Production\n(Reels)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;           
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Production\n(Weight)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;            
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Repacking\n(Reels)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;            
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Repacking\n(Weight)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Dispatch\n(Reels)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Dispatch\n(Weight)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Closing\n(Reels)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Closing\n(Weight)", fontB));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
           

            foreach (var rec in Report.ToList())
            {
                cell = new PdfPCell(new Phrase(rec.Item, fontN));
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.OpnQty != 0 ? (rec.OpnQty.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.OpnWht != 0 ? (rec.OpnWht.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.InwardQty != 0 ? (rec.InwardQty.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.InwardWht != 0 ? (rec.InwardWht.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.RepackQuantity != 0 ? (rec.RepackQuantity.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.RepackWeight != 0 ? (rec.RepackWeight.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.OutwardQty != 0 ? (rec.OutwardQty.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.OutwardWht != 0 ? (rec.OutwardWht.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.Quantity != 0 ? (rec.Quantity.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(rec.Weight != 0 ? (rec.Weight.ToString("N2")) : "-", fontN));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

            }
            cell = new PdfPCell(new Phrase(" ", fontN));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", fontN));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Report.Sum(o => o.OpnQty) != 0 ? Report.Sum(o => o.OpnQty).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Report.Sum(o => o.OpnWht) != 0 ? Report.Sum(o => o.OpnWht).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Report.Sum(o => o.InwardQty) != 0 ? Report.Sum(o => o.InwardQty).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Report.Sum(o => o.InwardWht) != 0 ? Report.Sum(o => o.InwardWht).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Report.Sum(o => o.RepackQuantity) != 0 ? Report.Sum(o => o.RepackQuantity).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Report.Sum(o => o.RepackWeight) != 0 ? Report.Sum(o => o.RepackWeight).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Report.Sum(o => o.OutwardQty) != 0 ? Report.Sum(o => o.OutwardQty).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Report.Sum(o => o.OutwardWht) != 0 ? Report.Sum(o => o.OutwardWht).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Report.Sum(o => o.Quantity) != 0 ? Report.Sum(o => o.Quantity).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase(Report.Sum(o => o.Weight) != 0 ? Report.Sum(o => o.Weight).ToString("N2") : "-", fontB));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);





            pdfDoc.Add(table);

            //FirmName = new iTextSharp.text.Paragraph(" ");
            //FirmName.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
            //FirmName.Font = fontN;
            //pdfDoc.Add(FirmName);



            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] MyPdf = stream.ToArray();
            Utility.SendWhatsapp("",MyPdf,"", ConnString);
            return Ok("");



        }

    }
}
