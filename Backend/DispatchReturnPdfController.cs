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
using System.Text;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class DispatchReturnPdfController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<DispatchReturnPdfController> logger;
        private IWebHostEnvironment Environment;
        public DispatchReturnPdfController(ILogger<DispatchReturnPdfController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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
                List<DispatchReturn> itemprod = new List<DispatchReturn>();
                DispatchReturn itemprod1 = new DispatchReturn();
                DispatchReturn dispatch;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    //dispatch = conn.Query<Dispatch>("Update Dispatch set  Status=1  where Id=" + id).FirstOrDefault();
                    itemprod = conn.Query<DispatchReturn>("select Slip.*,DispatchReturn.*,DispatchReturn.Date as DispatchDate,DispatchReturnMeta.*," +
                        "Items.Name as ItemName,BF.Name as BFName,ReelDia.Name as ReelDiaName,Items.ShortName as ShortName, GSM.Name as GSM ,Users.UserName as EnteredName ," +
                        " (Size.Name) as Size ,Size.Unit as Unitname " +
                        "from DispatchReturn " +
                        " left join DispatchReturnMeta on DispatchReturn.Id=DispatchReturnMeta.DispatchId" +
                        " left join Slip on  DispatchReturnMeta.SlipId=Slip.Id " +
                        "left join Items on Slip.Quality=Items.Id " +
                        "left join ReelDia on Slip.ReelDia=ReelDia.Id " +
                        "left join BF on Slip.BF=BF.Id " +
                        "left join Users on Slip.EntredBy=Users.Id " +
                        "left join GSM on Slip.GSMId=GSM.Id " +
                        "left join Size on Slip.SizeId=Size.Id" +
                        " where DispatchReturn.Id=" + id + "  order by GSM.Name, Size.Name  ").ToList();

                    Business business = conn.Query<Business>("select * from Business where id=1").FirstOrDefault();
                    itemprod1 = conn.Query<DispatchReturn>("select *,DispatchReturn.Date as DispatchDate from DispatchReturn where Id=" + id).FirstOrDefault();


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

                    report = new iTextSharp.text.Paragraph("Dispatch Return ");
                    report.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    report.Font = font;
                    pdfDoc.Add(report);

                    MarwariPdf.DrawText(pdf, "bold", "", BaseColor.BLACK, 10, 0, "Date : " + (itemprod1.DispatchDate.ToDate().ToString("dd-MMM-yyyy") ?? "")
                        + "                                  Dispatch Return No. : " + (itemprod1.VNumber.ToString() ?? "") +
                        "                                  Vehicle No : " + itemprod1.VehicleNo ?? "", 50, 750, 0);

                    MarwariPdf.DrawText(pdf, "bold", "", BaseColor.BLACK, 10, 0, " Remark : " + itemprod1.Remark ?? "", 50, 735, 0);
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
                    if (client.Contains("BHAGESHWARIDISPATCH"))
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

                    foreach (DispatchReturn ob in itemprod)
                    {
                        string count = itemprod.Count(o => o.Size == ob.Size).ToString();
                        string net = itemprod.Where(o => o.Size == ob.Size).Sum(o => o.NetWeight).ToString();






                        cell = new PdfPCell(new Phrase(serial.ToString(), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize)
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
                        if (ob.Size != previousSize)
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
                        if (ob.Size != previousSize)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        if (client.Contains("BHAGESHWARIDISPATCH"))
                        {
                            cell = new PdfPCell(new Phrase((ob.ReelDiaName) ?? "", fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.Size != previousSize )
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
                            if (ob.Size != previousSize )
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
                        cell = new PdfPCell(new Phrase(ob.Weight.ToString(), fontN));
                        cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                        if (ob.Size != previousSize)
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
                        if (ob.Size != previousSize)
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
                        if (ob.Size != previousSize)
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
                        if (ob.Size != previousSize)
                        {
                            // Add a line when ob.Size changes
                            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;  // Add a bottom border to the cell

                        }
                        else
                        {
                            cell.Border = PdfPCell.NO_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
                        }
                        table.AddCell(cell);
                        if (ob.Size != previousSize)
                        {
                            // Add a cell with borders at the end of each group
                            cell = new PdfPCell(new Phrase(count, fontN)); // Show the group count
                            if (ob.Size != previousSize)
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
                            if (ob.Size != previousSize)
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
                        if (ob.Size != previousSize)
                        {
                            // Add a cell with borders at the end of each group
                            cell = new PdfPCell(new Phrase(net, fontN));
                            cell.HorizontalAlignment = 1; //0=left, 1=center, 2=right
                            if (ob.Size != previousSize)
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
                            if (ob.Size != previousSize)
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
                    cell = new PdfPCell(new Phrase(" ", fontB));
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

        public IActionResult put(int id, DispatchReturn lst)

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

                    lst = conn.Query<DispatchReturn>("Update DispatchReturn set  Status=1  where Id=" + id, lst).FirstOrDefault();


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
    