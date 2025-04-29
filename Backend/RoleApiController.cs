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
using System.Threading.Tasks;

namespace DISPATCHAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class RoleApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<RoleApiController> logger;
        public RoleApiController(ILogger<RoleApiController> _logger, IConfiguration _Configuration)
        {
            _logger = logger;
            Configuration = _Configuration;
        }


        [HttpGet]
        public ActionResult Getch()
        {
            try
            {
                List<Role> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<Role>("select * from Roles").ToList();

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
                Role lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<Role>("select * from Roles where Id=" + id).FirstOrDefault();


                    return Ok(lst);
                }

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }

        }

        [HttpPost]

        public IActionResult Post(Role lst)
        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst.Id = conn.ExecuteScalar<int>("select max(Id) from Roles") + 1;

                    conn.Execute("insert into Roles(Id,Roles)values (@Id,@Roles)", lst);

                    //return Ok("Created");
                    return Ok(new { Message = "Created" });

                }

            }

            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

        [HttpPut("{Id}")]

        public IActionResult put(int id, Role lst)

        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    conn.Execute("update Roles set Roles=@Roles where Id=" + id, lst);


                    //return Ok("Updated");
                    return Ok(new { Message = "Updated" });

                }
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });
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

                    conn.Execute("Delete from Roles where Id=" + id);



                    //return Ok("Deleted");
                    return Ok(new { Message = "Deleted" });
                }
            }
            catch (Exception ex)
            {
                string a = "constraint failed\r\nFOREIGN KEY constraint failed";
                if (ex.Message == a)
                {
                    //return BadRequest("this is not deleted");
                    return BadRequest(new { Message = "this is not deleted" });
                }
                else
                {
                    //return BadRequest(ex.Message);
                    return BadRequest(new { Message = "Failed" + ex.Message });
                }
            }
        }



    }
}
