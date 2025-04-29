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

    public class PrintBundleController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<PrintBundleController> logger;
        private IWebHostEnvironment Environment;
        public PrintBundleController(ILogger<PrintBundleController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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
                    Bundle itemprod = conn.Query<Bundle>("select Bundle.Id, Bundle.NoOfRim,Bundle.RimWeight,Bundle.BundleWeight,Bundle.FormattedNo," +
                        "Items.Name as QualityName,GSM.Name as GSM," +
                        "BundleSize.Name as Size from Bundle " +
                        "left join Items on Items.id=Bundle.Quality " +
                        "left join GSM on GSM.Id=Bundle.GSMId " +
                        "left join BundleSize on BundleSize.Id=Bundle.BundleSizeId where Bundle.Id=" + id + " " +
                        "group by Bundle.Id, Bundle.NoOfRim,Bundle.RimWeight,Bundle.BundleWeight,Items.Name,GSM.Name," +
                        "BundleSize.Name ").FirstOrDefault();

                    Business business = conn.Query<Business>("select * from Business where id=1").FirstOrDefault();


                    MemoryStream fs = new MemoryStream();
                    //if (business.PrintName == "Print1")
                    //{
                    System.Globalization.CultureInfo Indian = new System.Globalization.CultureInfo("hi-IN");
                    Document doc = new Document(new Rectangle(288, 432), 0, 0, 0, 0);
                    PdfWriter PD = PdfWriter.GetInstance(doc, fs);
                    doc.Open();
                    PdfContentByte pdf = PD.DirectContent;

                    int counter = 1;
                    int y = 834;




                    string QRcodeStr = itemprod.FormattedNo;
                    MarwariPdf.DrawImage(PD, MarwariQR.GenerateQR(QRcodeStr), System.Drawing.Imaging.ImageFormat.Png, 60, y - 820, 170, 160);

                    //MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 27, 1, meta.Remark3, 135, y - 790, 0);
                    //MarwariPdf.DrawText(pdf, "", "", BaseColor.BLACK, 12, 2, meta.Packing, 267, y - 820, 0);
                    MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 16, 0, "Shree Bhageshwari Papers Pvt.Ltd.", 14, y - 420, 0);

                    MarwariPdf.DrawText(pdf, "", "", BaseColor.BLACK, 15, 0, "Code", 14, y - 450, 0);
                    MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 18, 2, itemprod.FormattedNo, 267, y - 450, 0);
                    MarwariPdf.DrawLine(pdf, BaseColor.BLACK, 1, 20, y - 455, 270, y - 455);

                    MarwariPdf.DrawText(pdf, "", "", BaseColor.BLACK, 15, 0, "Quality", 14, y - 480, 0);
                    MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 18, 2, itemprod.QualityName ?? "", 267, y - 480, 0);
                    MarwariPdf.DrawLine(pdf, BaseColor.BLACK, 1, 20, y - 490, 270, y - 490);

                    MarwariPdf.DrawText(pdf, "", "", BaseColor.BLACK, 15, 0, "GSM", 14, y - 510, 0);
                    MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 18, 2, itemprod.GSM ?? "", 267, y - 510, 0);
                    MarwariPdf.DrawLine(pdf, BaseColor.BLACK, 1, 20, y - 520, 270, y - 520);

                    MarwariPdf.DrawText(pdf, "", "", BaseColor.BLACK, 16, 0, "Size", 14, y - 545, 0);
                    MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 21, 2, itemprod.Size ?? "", 267, y - 545, 0);
                    MarwariPdf.DrawLine(pdf, BaseColor.BLACK, 1, 20, y - 555, 270, y - 555);

                    MarwariPdf.DrawText(pdf, "", "", BaseColor.BLACK, 16, 0, "Rim Weight", 14, y - 585, 0);
                    MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 21, 2, itemprod.RimWeight.ToString() ?? "", 267, y - 585, 0);
                    MarwariPdf.DrawLine(pdf, BaseColor.BLACK, 1, 20, y - 595, 270, y - 595);

                    MarwariPdf.DrawText(pdf, "", "", BaseColor.BLACK, 15, 0, "No of Rim", 14, y - 620, 0);
                    MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 18, 2, itemprod.NoOfRim.ToString() ?? "", 267, y - 620, 0);
                    MarwariPdf.DrawLine(pdf, BaseColor.BLACK, 1, 20, y - 630, 270, y - 630);

                    MarwariPdf.DrawText(pdf, "", "", BaseColor.BLACK, 15, 0, "Bundle Weight", 14, y - 660, 0);
                    MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 18, 2, itemprod.BundleWeight.ToString() ?? "", 267, y - 660, 0);
                    MarwariPdf.DrawLine(pdf, BaseColor.BLACK, 1, 20, y - 670, 270, y - 670);

                    //MarwariPdf.DrawText(pdf, "BOLD", "", BaseColor.BLACK, 24, 1, itemprod.FormattedNo ?? "", 144, y - 820, 0);
                    doc.NewPage();


                    counter++;



                    PD.CloseStream = false;
                    doc.Close();
                    byte[] MyPdf = fs.ToArray();
                    // }




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
