using Dapper;
using DISPATCHAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class BundleCheckController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<BundleCheckController> logger;
        private IWebHostEnvironment Environment;
        public BundleCheckController(ILogger<BundleCheckController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet]

        public IActionResult Get()
        {
            try
            {


                List<Bundle> lst = new List<Bundle>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<Bundle>("select Bundle.FormattedNo,Bundle.Id  from Bundle ").ToList();



                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

        [HttpGet("{id}")]

        public IActionResult Get(string id)
        {
            try
            {


                Bundle lst = new Bundle();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    int Invalid = conn.ExecuteScalar<int>("select Count(*) from Bundle where FormattedNo ='" + id + "'");

                    if (Invalid == 0)
                    {
                        return Ok(new { Message = "Invalid Bundle no." });
                    }
                    else if (Invalid > 0)
                    {
                        return Ok(new { Message = "Ok" });
                    }
                    else
                    {
                        return BadRequest("Please select Bundle no.");
                        // return BadRequest(new { Message = "Please select Reel no." });
                    }



                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }
    }
}
