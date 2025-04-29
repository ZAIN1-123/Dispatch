using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class TestBackupController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;

        public TestBackupController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult GetBackup()
        {
            try
            {

                string OrignalFile = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Data.db");
                string TimeString = DateTime.Now.ToString("yyyyMMddHHmmss");
                string Dir = Path.Combine(_hostingEnvironment.ContentRootPath, "Temp", "Backup" + TimeString);

                System.IO.File.WriteAllText("1.txt", Dir);

                Directory.CreateDirectory(Dir);

                System.IO.File.Copy(OrignalFile, Dir + "\\" + "Data.db");
                string zipPath = Dir + ".zip";

                ZipFile.CreateFromDirectory(Dir, zipPath);

                Directory.Delete(Dir, true);
                string content = Convert.ToBase64String(System.IO.File.ReadAllBytes(zipPath));
                System.IO.File.Delete(zipPath);

                List<string> vs = new List<string>();
                vs.Add(content);
                vs.Add(("Data.db").Split('.').FirstOrDefault());

                //string json = Newtonsoft.Json.JsonConvert.SerializeObject(vs);

                //var response = new HttpResponseMessage(HttpStatusCode.OK);

                //response.Content = new StringContent(json, Encoding.UTF8, "application/json");

                return Ok(vs);
            }
            catch (System.Exception ex)
            {
                System.IO.File.WriteAllText("catch.txt", ex.Message);
                if (ex.Message == null)
                {

                    return BadRequest(new { Message = ex.InnerException.InnerException.Message });
                }
                else
                {
                    return BadRequest(new { Message = ex.Message });


                }
            }

        }
    }
}
