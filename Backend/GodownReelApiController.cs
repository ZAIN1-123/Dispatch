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
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DISPATCHAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class GodownReelApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<GodownReelApiController> logger;
        private IWebHostEnvironment Environment;
        public GodownReelApiController(ILogger<GodownReelApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Configuration = _Configuration;
            Environment = _environment;
        }




        [HttpGet("{reelno}")]

        public IActionResult Get(string reelno)
        {
            try
            {
                Dispatch lst = new Dispatch();
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int Duplicate = conn.ExecuteScalar<int>("select Sum(Quantity) from Stockbook where ReelNumber ='" + reelno + "'", transaction);

                            if (Duplicate == 0)
                            {
                                transaction.Rollback();
                                return Ok("Not Found");
                            }

                            //int Exist = conn.ExecuteScalar<int>("select Count(*) from Slip where FormattedNo ='" + reelno + "'", transaction);
                            //if (Exist == 0)
                            //{
                            //    transaction.Rollback();
                            //    return NotFound(reelno + "Invalid QR Code");
                            //}

                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return BadRequest(new { Message = "Failed" + e.Message });
                        }
                    }                    
                    return Ok("Success");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed: " + ex.Message });
            }
        }




    }
}






