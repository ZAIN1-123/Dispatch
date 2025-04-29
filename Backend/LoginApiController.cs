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

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class LoginApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<LoginApiController> logger;
        private readonly IWebHostEnvironment Environment;

        public LoginApiController(ILogger<LoginApiController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet]
        public ActionResult LoginDetails()
        {
            try
            {
                string Username = Request.Headers["Username"].FirstOrDefault();
                string Password = Request.Headers["Password"].FirstOrDefault();
                //string Platform = Request.Headers["platform"].FirstOrDefault();
                if (Username.Length == 0)
                {

                    return BadRequest(new { Message = "Enter a Username." });
                }
                else if (Password.Length == 0)
                {

                    return BadRequest(new { Message = "Enter a Password." });
                }
              
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                   List<User> cnt  = conn.Query<User>("Select * from Users where UserName='" + Username + "' and Password='" + Password + "'").ToList();
                    if (cnt.Count() > 0)
                    {
                        //   System.IO.File.WriteAllText("Text.txt", "abcdef");

                        int Id = conn.ExecuteScalar<int>("Select Id from Users where UserName='" + Username + "' and Password='" + Password + "'");
                        int Date = conn.ExecuteScalar<int>("Select StartDate from Business where id=1");
                        int Date1 = conn.ExecuteScalar<int>("Select EndDate from Business where id=1");

                        string Authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(Username + ":" + Password));

                        User obj = new User()
                        {
                            UserName = Username,
                            Password = Password,
                            Auth = Authorization,
                            Id = Id,
                            StartDate = Date,
                            EndDate = Date1,
                            Quality = cnt.FirstOrDefault().Quality,
                            Godown = cnt.FirstOrDefault().Godown,
                            GSM = cnt.FirstOrDefault().GSM,
                            Size = cnt.FirstOrDefault().Size,
                            Business = cnt.FirstOrDefault().Business,
                            BF = cnt.FirstOrDefault().BF,
                            Slip = cnt.FirstOrDefault().Slip,
                            IsEditAllowed = cnt.FirstOrDefault().IsEditAllowed,
                            IsDeleteAllowed = cnt.FirstOrDefault().IsDeleteAllowed,
                            BackDateAllowed = cnt.FirstOrDefault().BackDateAllowed,
                            manual = cnt.FirstOrDefault().manual,
                            userallowed = cnt.FirstOrDefault().userallowed,
                            Reprintallowed = cnt.FirstOrDefault().Reprintallowed,
                            Dispatch = cnt.FirstOrDefault().Dispatch,
                            Report = cnt.FirstOrDefault().Report,
                            GodownTransfer = cnt.FirstOrDefault().GodownTransfer,
                            LocationTransfer = cnt.FirstOrDefault().LocationTransfer,
                            ReelDia=cnt.FirstOrDefault().ReelDia,
                            Location=cnt.FirstOrDefault().Location


                        };

                        return Ok(obj);

                    }
                    else
                    {
                        //  System.IO.File.WriteAllText("else.txt", "qwertyuuuuu");
                        //return BadRequest(new { Message = "Invalid User." });
                        return Ok(new { Message = "Invalid User." });
                    }
                }
            }
            catch
            (Exception ex)
            {
               

                return BadRequest(new { Message ="Failed" + ex.Message });

            }
        }
      
    }
}


