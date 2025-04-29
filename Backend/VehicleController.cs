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

    public class VehicleController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<VehicleController> logger;
        private IWebHostEnvironment Environment;
        public VehicleController(ILogger<VehicleController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
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


                List<Dispatch> lst = new List<Dispatch>();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<Dispatch>("select Dispatch.VehicleNo  from Dispatch group by Dispatch.VehicleNo ").ToList();

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
