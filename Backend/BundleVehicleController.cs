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

    public class BundleVehicleController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<BundleVehicleController> logger;
        private IWebHostEnvironment Environment;
        public BundleVehicleController(ILogger<BundleVehicleController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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


                List<BundleDispatch> lst = new List<BundleDispatch>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<BundleDispatch>("select BundleDispatch.VehicleNo  from BundleDispatch group by BundleDispatch.VehicleNo ").ToList();

                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }


    }
}
