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

    public class GodownTransferPdfController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<GodownTransferPdfController> logger;
        private IWebHostEnvironment Environment;
        public GodownTransferPdfController(ILogger<GodownTransferPdfController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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

                List<GodownTransfer> itemprod = new List<GodownTransfer>();
                GodownTransfer itemprod1 = new GodownTransfer();
                GodownTransfer gt;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    //dispatch = conn.Query<Dispatch>("Update Dispatch set  Status=1  where Id=" + id).FirstOrDefault();
                    itemprod = conn.Query<GodownTransfer>("select godownTransfer.*, godowntransfermeta.* from godowntransfer" +
                        " left join godowntransfermeta on godowntransfermeta.godowntransferId= godowntransfer.Id " +
                       
                        " where godowntransfer.Id=" + id + "  order by GSMName, SizeName  ").ToList();

                    Business business = conn.Query<Business>("select * from Business where id=1").FirstOrDefault();
                    itemprod1 = conn.Query<GodownTransfer>("select *,godowntransfer.Date,Godown.Name as FromGodownName,a.Name as ToGodownName" +
                        " from godowntransfer" +
                        " left join godown on godowntransfer.fromgodown=godown.id" +
                        " left join godown as a on godowntransfer.togodown= a.id  " +
                        " where godowntransfer.Id=" + id).FirstOrDefault();


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

                    report = new iTextSharp.text.Paragraph("Godown Transfer ");
                    report.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    report.Font = font;
                    pdfDoc.Add(report);

                    MarwariPdf.DrawText(pdf, "bold", "", BaseColor.BLACK, 10, 0, "Date : " + (itemprod1.Date.ToDate().ToString("dd-MMM-yyyy") ?? "")
                        + "                                  From Godown: " + (itemprod1.FromGodownName.ToString() ?? "") +
                        "                                  To Godown : " + itemprod1.ToGodownName ?? "", 50, 750, 0);

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
                    cell = new PdfPCell(new Phrase("BF", fontB));
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

                    foreach (GodownTransfer ob in itemprod)
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
                        cell = new PdfPCell(new Phrase((ob.BFName), fontN));
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
                       
                        cell = new PdfPCell(new Phrase("KRAFT PAPER", fontN));
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
