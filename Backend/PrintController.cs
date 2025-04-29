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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class PrintController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<PrintController> logger;
        private IWebHostEnvironment Environment;
        public PrintController(ILogger<PrintController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet("{Id}")]

        public IActionResult Getch(int id)
        {
            try
            {
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    Slip itemprod = conn.Query<Slip>("select Slip.*, GSM.Name as GSM , Items.Name as ItemName, BF.Name as BFName, Size.Name as Size," +
                        " Size.Unit as Unit , ReelDia.Name as ReelDiaName  ,ReelDia.Unit as Unit1 from Slip" +
                        " left join GSM on Slip.GSMId=GSM.Id left join Size on Slip.SizeId=Size.Id left join ReelDia on Slip.ReelDia=ReelDia.Id " +
                        "left join Items on Slip.Quality=Items.Id left join BF on Slip.BF= BF.Id where Slip.Id=" + id).FirstOrDefault();

                    Business business = conn.Query<Business>("select * from Business where id=1").FirstOrDefault();
                   
                    
                        MemoryStream fs = new MemoryStream();
                    if (business.PrintName == "Print1")
                    {
                        Document doc = new Document(new Rectangle(PageSize.A4));
                        doc.SetMargins(0, 0, 0, 0); // Optional: Set margins to 0
                        doc.SetPageSize(new Rectangle(288, 144));
                        PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                        doc.Open();
                        PdfContentByte cb = writer.DirectContent;
                        string wwwPath = this.Environment.WebRootPath;

                        string QRcodeStr = itemprod.FormattedNo;//"http://www.google.com/";

                        //MarwariPdf.DrawImage(writer, MarwariQR.GenerateQR(QRcodeStr), ImageFormat.Png, 457, 670, 110, 100);

                        int point = 824;
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 10, point - 688, 275, point - 688);
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 10, point - 815, 275, point - 815);
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 10, point - 815, 10, point - 688);
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 275, point - 815, 275, point - 688);

                        MarwariPdf.DrawImage(writer, MarwariQR.GenerateQR(QRcodeStr), ImageFormat.Png, 190, point - 805, 80, 80);                      

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 13, 1, business.Name ?? "", 140, point - 703, 0);
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 25, point - 705, 245, point - 705);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "Date", 25, point - 720, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": " + itemprod.Date.ToDate().ToString("dd-MM-yyyy"), 100, point - 720, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "Reel No.", 25, point - 730, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": " + (itemprod.FormattedNo ?? ""), 100, point - 730, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "Product", 25, point - 742, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": KRAFT PAPER", 100, point - 742, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "Quality", 25, point - 753, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": " + (itemprod.ItemName ?? ""), 100, point - 753, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "BF", 25, point - 764, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": " + (itemprod.BFName ?? ""), 100, point - 764, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 11, 0, "GSM", 25, point - 777, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 11, 0, ": " + (itemprod.GSM ?? ""), 100, point - 777, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 11, 0, "Size", 25, point - 791, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 16, 0, ": " + (itemprod.Size ?? "") + " " + (itemprod.Unit ?? ""), 100, point - 791, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 11, 0, "Net Weight", 25, point - 807, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 16, 0, ": " + itemprod.NetWeight + " KG", 100, point - 807, 0);

                        doc.NewPage();
                        wwwPath = this.Environment.WebRootPath;

                        QRcodeStr = itemprod.FormattedNo;//"http://www.google.com/";
                        point = 824;
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 10, point - 688, 275, point - 688);
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 10, point - 815, 275, point - 815);
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 10, point - 815, 10, point - 688);
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 275, point - 815, 275, point - 688);

                        MarwariPdf.DrawImage(writer, MarwariQR.GenerateQR(QRcodeStr), ImageFormat.Png, 190, point - 805, 80, 80);                        

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 13, 1, business.Name ?? "", 140, point - 703, 0);
                        MarwariPdf.DrawLine(cb, BaseColor.BLACK, 1, 25, point - 705, 245, point - 705);


                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "Date", 25, point - 720, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": " + itemprod.Date.ToDate().ToString("dd-MM-yyyy"), 100, point - 720, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "Reel No.", 25, point - 730, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": " + (itemprod.FormattedNo ?? ""), 100, point - 730, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "Product", 25, point - 742, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": KRAFT PAPER", 100, point - 742, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "Quality", 25, point - 753, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": " + (itemprod.ItemName ?? ""), 100, point - 753, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, "BF", 25, point - 764, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 10, 0, ": " + (itemprod.BFName ?? ""), 100, point - 764, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 11, 0, "GSM", 25, point - 777, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 11, 0, ": " + (itemprod.GSM ?? ""), 100, point - 777, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 11, 0, "Size", 25, point - 791, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 16, 0, ": " + (itemprod.Size ?? "") + " " + (itemprod.Unit ?? ""), 100, point - 791, 0);

                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 11, 0, "Net Weight", 25, point - 807, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 16, 0, ": " + itemprod.NetWeight + " KG", 100, point - 807, 0);

                        doc.Close();

                    }
                    else
                    {
                        Document doc = new Document(new Rectangle(PageSize.A4));
                        doc.SetMargins(0, 0, 0, 0); // Optional: Set margins to 0
                        doc.SetPageSize(new Rectangle(468,418));
                        PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                        doc.Open();
                        PdfContentByte cb = writer.DirectContent;
                        string wwwPath = this.Environment.WebRootPath;

                        string QRcodeStr = itemprod.FormattedNo;//"http://www.google.com/";

                        //MarwariPdf.DrawImage(writer, MarwariQR.GenerateQR(QRcodeStr), ImageFormat.Png, 457, 670, 110, 100);

                        int point = 1030;
                      

                        MarwariPdf.DrawImage(writer, MarwariQR.GenerateQR(QRcodeStr), ImageFormat.Png, 360, point - 965, 100, 100);

                       // MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 15, 0,  (itemprod.ItemName ?? ""), 150, point - 731, 0);//quality
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 14, 0,  (itemprod.ItemName ?? ""), 150, point - 731, 0);//quality
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 14, 0, (itemprod.FormattedNo ?? ""), 360, point - 731, 0);//roll no
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 14, 0, (itemprod.SetNo ?? ""), 160, point - 774, 0);//set no
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 14, 0, itemprod.NetWeight + " KG", 160, point - 861, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 14, 0, (itemprod.GSM ?? ""), 360, point - 774, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 14, 0, (itemprod.Shift ?? "") , 160, point - 817, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 14, 0, (itemprod.Size ?? "") + " " + (itemprod.Unit ?? ""), 360, point - 817, 0);
                        //MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 15, 0, ("-"), 160, point - 817, 0);
                        MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 14, 0, (itemprod.ReelDiaName??"")+" " +(itemprod.Unit1??""), 160, point - 901, 0);
                       // MarwariPdf.DrawText(cb, "bold", "", BaseColor.BLACK, 15, 0, (itemprod.Shift??""), 160, point - 916, 0);

                        doc.Close();
                    }

                        byte[] byteinfo = fs.ToArray();
                    

                    return Ok(byteinfo);
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
