using Dapper;
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
using System.IO;
using System.Linq;
using System.Text;
using DISPATCHAPI.Models;
using DISPATCHAPI;

namespace DispatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class BundlePdfController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<BundlePdfController> logger;
        private IWebHostEnvironment Environment;
        public BundlePdfController(ILogger<BundlePdfController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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
                List<BundleDispatch> itemprod = new List<BundleDispatch>();
                BundleDispatch itemprod1 = new BundleDispatch();
                BundleDispatch BundleDispatch;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    //BundleDispatch = conn.Query<BundleDispatch>("Update BundleDispatch set  Status=1  where Id=" + id).FirstOrDefault();
                    itemprod = conn.Query<BundleDispatch>("select Bundle.*,BundleDispatch.*,BundleDispatch.Date as BundleDispatchDate,BundleStock.*," +
                        "Items.Name as ItemName,Items.ShortName as ShortName," +
                        " GSM.Name as GSM ,(BundleSize.Name) as BundleSize ,BundleSize.Unit as Unitname " +
                        "from BundleDispatch " +
                        " left join BundleStock on BundleDispatch.Id=BundleStock.BundleDispatchId" +
                        " left join Bundle on  BundleStock.BundleId=Bundle.Id " +                        
                        "left join Items on Bundle.Quality=Items.Id " +                                               
                        "left join GSM on Bundle.GSMId=GSM.Id " +
                        "left join BundleSize on Bundle.BundleSizeId=BundleSize.Id" +
                        " where BundleDispatch.Id=" + id + "  order by Items.Name desc ,GSM.Name asc , BundleSize.Name asc ,Items.ShortName ").ToList();

                    Business business = conn.Query<Business>("select * from Business where id=1").FirstOrDefault();
                    itemprod1 = conn.Query<BundleDispatch>("select *,BundleDispatch.Date as DispatchDate from BundleDispatch where Id=" + id).FirstOrDefault();

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

                    report = new iTextSharp.text.Paragraph("BundleDispatch ");
                    report.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    report.Font = font;
                    pdfDoc.Add(report);

                    MarwariPdf.DrawText(pdf, "bold", "", BaseColor.BLACK, 10, 0, "Date : " + (itemprod1.DispatchDate.ToDate().ToString("dd-MMM-yyyy") ?? "")
                        + "                                  BundleDispatch No. : " + (itemprod1.VNumber.ToString() ?? "") +
                        "                                  Vehicle No : " + itemprod1.VehicleNo ?? "", 50, 750, 0);

                    MarwariPdf.DrawText(pdf, "bold", "", BaseColor.BLACK, 10, 0, " Remark : " + itemprod1.Remark ?? "", 50, 735, 0);
                    iTextSharp.text.Paragraph FirmName = new iTextSharp.text.Paragraph("\n\n");
                    FirmName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    FirmName.Font = fontN;
                    pdfDoc.Add(FirmName);
                    PdfPTable table = new PdfPTable(9);
                    float[] widths = new float[] { .3f, .9f, .6f, .6f, .6f, .6f, .9f, .3f, .6f };
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
                   

                    cell = new PdfPCell(new Phrase("Weight(KG)", fontB));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Bundle Size", fontB));
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

                    cell = new PdfPCell(new Phrase("Bundle nos.", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Weight(KG)", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    int serial = 1;

                    string previousBundleSize = string.Empty;
                    string previousGSM = string.Empty;
                    string previousBF = string.Empty;
                    string previousshortname = string.Empty;

                    foreach (BundleDispatch ob in itemprod)
                    {
                        //string count = itemprod.Count(o => o.BundleSize == ob.BundleSize && o.BFName == ob.BFName && o.GSM == ob.GSM && o.ShortName== ob.ShortName).ToString();
                        string count = itemprod.Count(o => o.BundleSize == ob.BundleSize && o.BFName == ob.BFName && o.GSM == ob.GSM && o.ShortName == ob.ShortName && o.ItemName == ob.ItemName).ToString();
                        //string net = itemprod.Where(o => o.BundleSize == ob.BundleSize && o.BFName == ob.BFName && o.GSM == ob.GSM && o.ShortName == ob.ShortName).Sum(o => o.NetWeight).ToString();
                        string net = itemprod.Where(o => o.BundleSize == ob.BundleSize && o.BFName == ob.BFName && o.GSM == ob.GSM && o.ShortName == ob.ShortName && o.ItemName == ob.ItemName).Sum(o => o.BundleWeight).ToString();

                        cell = new PdfPCell(new Phrase(serial.ToString(), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.BundleSize changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.FormattedNo, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.BundleSize changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.GSM, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.BundleSize changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);                       
                        cell = new PdfPCell(new Phrase(ob.BundleWeight.ToString(), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.BundleSize changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.BundleSize, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.BundleSize changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.Unitname, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.BundleSize changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.ItemName, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a line when ob.BundleSize changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a cell with borders at the end of each group
                            cell = new PdfPCell(new Phrase(count, fontN)); // Show the group count
                            if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.BundleSize changes
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
                            if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.BundleSize changes
                                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                            }
                            else
                            {
                                cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                            }

                            table.AddCell(cell);

                        }
                        if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                        {
                            // Add a cell with borders at the end of each group
                            cell = new PdfPCell(new Phrase(net, fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.BundleSize changes
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
                            if (ob.BundleSize != previousBundleSize || ob.GSM != previousGSM || ob.BFName != previousBF || ob.ShortName != previousshortname)
                            {
                                // Add a line when ob.BundleSize changes
                                cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                            }
                            else
                            {
                                cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;

                            }

                            table.AddCell(cell);

                        }


                        serial++;

                        previousBundleSize = ob.BundleSize;
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
                    cell = new PdfPCell(new Phrase(itemprod.Count().ToString(), fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(itemprod.Sum(o => o.BundleWeight).ToString() + " KG", fontB));
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

        public IActionResult put(int id, BundleDispatch lst)
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

                    lst = conn.Query<BundleDispatch>("Update BundleDispatch set  Status=1  where Id=" + id, lst).FirstOrDefault();

                    return Ok("Updated");

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }       

    }
}
