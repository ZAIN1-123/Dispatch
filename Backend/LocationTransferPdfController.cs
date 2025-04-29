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
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class LocationTransferPdfController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<LocationTransferPdfController> logger;
        private IWebHostEnvironment Environment;
        public LocationTransferPdfController(ILogger<LocationTransferPdfController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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

                List<LocationTransfer> itemprod = new List<LocationTransfer>();
                LocationTransfer itemprod1 = new LocationTransfer();
                LocationTransfer gt;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    //dispatch = conn.Query<Dispatch>("Update Dispatch set  Status=1  where Id=" + id).FirstOrDefault();
                    itemprod = conn.Query<LocationTransfer>("select LocationTransfer.*, LocationTransferMeta.* from LocationTransfer" +
                        " left join LocationTransferMeta on LocationTransferMeta.LocationTransferId= LocationTransfer.Id " +

                        " where LocationTransfer.Id=" + id + "  order by GSMName, SizeName  ").ToList();

                    Business business = conn.Query<Business>("select * from Business where id=1").FirstOrDefault();
                    itemprod1 = conn.Query<LocationTransfer>("select *,LocationTransfer.Date,GodownLocation.Name as FromLocationName,a.Name as ToLocationName" +
                        " from LocationTransfer" +
                        " left join GodownLocation on LocationTransfer.FromLocation=GodownLocation.id" +
                        " left join GodownLocation as a on LocationTransfer.ToLocation= a.id  " +
                        " where LocationTransfer.Id=" + id).FirstOrDefault();


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

                    report = new iTextSharp.text.Paragraph("Location Transfer ");
                    report.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    report.Font = font;
                    pdfDoc.Add(report);

                    MarwariPdf.DrawText(pdf, "bold", "", BaseColor.BLACK, 10, 0, "Date : " + (itemprod1.Date.ToDate().ToString("dd-MMM-yyyy") ?? "")
                        + "                                  From Location: " + (itemprod1.FromLocationName.ToString() ?? "") +
                        "                                  To Location : " + itemprod1.ToLocationName ?? "", 50, 750, 0);

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
                    cell = new PdfPCell(new Phrase("ReelDia", fontB));
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Weight(KG)", fontB));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Size", fontB));
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

                    foreach (LocationTransfer ob in itemprod)
                    {
                        string count = itemprod.Count(o => o.SizeName == ob.SizeName).ToString();
                        string net = itemprod.Where(o => o.SizeName == ob.SizeName).Sum(o => o.VNetWeight).ToString();






                        cell = new PdfPCell(new Phrase(serial.ToString(), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.SizeName != previousSize)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.Number, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.SizeName != previousSize)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.GSMName, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.SizeName != previousSize)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase((ob.ReelDiaName), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.SizeName != previousSize)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.VNetWeight.ToString(), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.SizeName != previousSize)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(ob.SizeName, fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.SizeName != previousSize)
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
                        if (ob.SizeName != previousSize)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        if (ob.SizeName != previousSize)
                        {
                            // Add a cell with borders at the end of each group
                            cell = new PdfPCell(new Phrase(count, fontN)); // Show the group count
                            if (ob.SizeName != previousSize)
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
                            if (ob.SizeName != previousSize)
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
                        if (ob.SizeName != previousSize)
                        {
                            // Add a cell with borders at the end of each group
                            cell = new PdfPCell(new Phrase(net, fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.SizeName != previousSize)
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
                            if (ob.SizeName != previousSize)
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

                        previousSize = ob.SizeName;

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
                    cell = new PdfPCell(new Phrase(itemprod.Sum(o => o.VNetWeight).ToString() + " KG", fontB));
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



    }
}
