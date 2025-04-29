using Dapper;
using DISPATCHAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class BusinessController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<BusinessController> logger;
        public BusinessController(ILogger<BusinessController> _logger, IConfiguration _Configuration)
        {
            _logger = logger;
            Configuration = _Configuration;
        }


        [HttpGet]
        public ActionResult Getch()
        {
            try
            {
                List<Business> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<Business>("select * from Business").ToList();

                    return Ok(lst);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });

            }
        }


        [HttpGet("{Id}")]

        public IActionResult Get(int id)
        {
            try
            {
                Business lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<Business>("select * from Business where Id=" + id).FirstOrDefault();


                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }

        }

        [HttpPut("{Id}")]

        public IActionResult put(int id, Business lst)

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
                    User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                    if (user.IsEditAllowed == 1)
                    {

                        conn.Execute("update Business set Name=@Name,Address1=@Address1," +
                        "Address2=@Address2,StartDate=@StartDate,EndDate=@EndDate,PrintName=@PrintName,WhatsappPassword=@WhatsappPassword,Whatsappuser=@Whatsappuser,WhatsappGroupId=@WhatsappGroupId where Id=" + id, lst);

                        return Ok("Updated");
                        //return Ok(new { Message = "Updated" });
                    }
                    else
                    {
                        return BadRequest("You are not allowed");
                        //return BadRequest(new { Message = "You are not allowed" });
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