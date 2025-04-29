using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class BackupController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;

        public BackupController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public IActionResult GetBackup()
        {
            try
            {
                System.IO.File.WriteAllText("1.txt", "1");
                //string OrignalFile = Directory.GetCurrentDirectory() + "\\App_Data\\Data.db";
                string OrignalFile = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Data.db");

                string TimeString = DateTime.Now.ToString("yyyyMMddHHmmss");
                //string Dir = Directory.GetCurrentDirectory() + "\\Temp\\Backup" + TimeString;
                string Dir = Path.Combine(_hostingEnvironment.ContentRootPath, "Temp", "Backup" + TimeString);

                Directory.CreateDirectory(Dir);

                System.IO.File.Copy(OrignalFile, "Temp\\Backup" + TimeString + "\\" + "Data.db");
                string zipPath = Dir + ".zip";
                //System.IO.File.WriteAllText("2.txt", zipPath);

                ZipFile.CreateFromDirectory(Dir, zipPath);
                Directory.Delete(Dir, true);
                string content = Convert.ToBase64String(System.IO.File.ReadAllBytes(zipPath));
                System.IO.File.Delete(zipPath);
                List<string> vs = new List<string>();

                vs.Add(content);
                vs.Add(("Data.db").Split('.').FirstOrDefault());

                return Ok(vs);
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText("catch.txt", exp.Message);
                if (exp.Message == null)
                {

                    return BadRequest(new { Message = exp.InnerException.InnerException.Message });
                }
                else
                {
                    return BadRequest(new { Message = exp.Message });


                }
            }
        }


    }
}
