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

    public class SizeApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<SizeApiController> logger;
        public SizeApiController(ILogger<SizeApiController> _logger, IConfiguration _Configuration)
        {
            _logger = logger;
            Configuration = _Configuration;
        }


        [HttpGet]
        public ActionResult Getch()
        {
            try
            {
                List<Size> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<Size>("select Size.*, (Size.Name|| ' ' || Size.Unit) as SizeName  from Size").ToList();

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
                Size lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<Size>("select * from Size where Id=" + id).FirstOrDefault();


                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }

        }

        [HttpPost]

        public IActionResult Post(Size lst)
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
                    if (user.Size == 1)
                    {
                        int size = conn.Query<int>($"Select Count(*) from Size where Name='{lst.Name}' and Unit='{lst.Unit}'").FirstOrDefault();
                        if (size == 0)
                        {
                            lst.Id = conn.ExecuteScalar<int>("select max(Id) from Size") + 1;

                            conn.Execute("insert into Size(Id,Name,Unit)values (@Id,@Name,@Unit)", lst);
                            return Ok("Created");
                            //return Ok(new { Message = "Created" });
                        }
                        else
                        {
                            return BadRequest("This size is already created.");
                            //return BadRequest(new { Message = "This size is already created." });
                        }

                       
                    }
                    else
                    {
                        return BadRequest("you are not allowed");
                        //return BadRequest(new { Message = "you are not allowed" });
                    }

                }

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

        [HttpPut("{Id}")]

        public IActionResult put(int id, Size lst)

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
                        int size = conn.Query<int>($"Select Count(*) from Size where Name='{lst.Name}' and Unit='{lst.Unit}'").FirstOrDefault();
                       
                            conn.Execute("update Size set Name=@Name, Unit=@Unit where Id=" + id, lst);


                            return Ok("Updated");
                            //return Ok(new { Message = "Updated" });
                     
                    }
                    else
                    {
                        return BadRequest(new { Message = "you are not allowed" });
                    }

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

        [HttpDelete("{Id}")]

        public IActionResult delete(int id)

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
                    if (user.IsDeleteAllowed == 1)
                    {
                        conn.Execute("Delete from Size where Id=" + id);



                        return Ok("Deleted");
                        //return Ok(new { Message = "Deleted" });
                    }
                    else
                    {
                       return BadRequest("you are not allowed");
                       // return BadRequest(new { Message = "you are not allowed" });
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






