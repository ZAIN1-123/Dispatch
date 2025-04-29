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
    public class GodownDispatchController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<GodownDispatchController> logger;
        private IWebHostEnvironment Environment;
        public GodownDispatchController(ILogger<GodownDispatchController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }
        [HttpGet("{id}")]

        public IActionResult Get(string id)
        {
            try
            {

                string where = "";
                Slip lst ;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    if (id != null)
                    {
                        where = " where FormattedNo ='" + id+" '";
                    }

                    lst = conn.Query<Slip>("Select GodownId " +
                        " from Slip  "+ where).FirstOrDefault();




                    return Ok(lst);
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
