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

    public class GodownLocationApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<GodownLocationApiController> logger;
        public GodownLocationApiController(ILogger<GodownLocationApiController> _logger, IConfiguration _Configuration)
        {
            _logger = logger;
            Configuration = _Configuration;
        }


        [HttpGet]
        public ActionResult Getch()
        {
            try
            {
                List<GodownLocation> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<GodownLocation>("select * from GodownLocation").ToList();

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
                GodownLocation lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<GodownLocation>("select * from GodownLocation where Id=" + id).FirstOrDefault();


                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                // return BadRequest(new { Message = "Failed" + ex.Message });
            }

        }

        [HttpPost]

        public IActionResult Post(GodownLocation lst)
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
                    if (user.Location == 1)
                    {
                        lst.Id = conn.ExecuteScalar<int>("select max(Id) from GodownLocation") + 1;
                        // Check if the Name already exists in the database
                        var exists = conn.QuerySingleOrDefault<int>(
                            "SELECT COUNT(1) FROM GodownLocation WHERE Name = @Name",
                            new { Name = lst.Name }
                        ) > 0;  // If count > 0, it means the Name exists

                        if (!exists)
                        {
                            // If Name doesn't exist, insert the new record
                            conn.Execute("INSERT INTO GodownLocation (Id, Name) VALUES (@Id, @Name)", lst);
                        }
                        else
                        {
                            // Optionally handle the case where the Name already exists
                            return Ok("Name already exists.");
                        }

                        //conn.Execute("insert into GodownLocation(Id,Name)values (@Id,@Name)", lst);

                        return Ok("Created");
                       
                    }
                    else
                    {
                        return BadRequest("you are not allowed ");
                        
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

        public IActionResult put(int id, GodownLocation lst)

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
                        conn.Execute("update GodownLocation set Name=@Name where Id=" + id, lst);


                        return Ok("Updated");
                        // return Ok(new { Message = "Updated" });

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
                        conn.Execute("Delete from GodownLocation where Id=" + id);



                        return Ok("Deleted");
                        // return Ok(new { Message = "Deleted" });
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
                string a = "constraint failed\r\nFOREIGN KEY constraint failed";
                if (ex.Message == a)
                {
                    return BadRequest("this is not deleted");
                    //return BadRequest(new { Message = "this is not deleted" });
                }
                else
                {
                    return BadRequest(ex.Message);
                    //return BadRequest(new { Message = "Failed" + ex.Message });
                }
            }
        }



    }
}






