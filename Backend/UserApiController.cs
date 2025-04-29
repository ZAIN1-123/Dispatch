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

    public class UserApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<UserApiController> logger;
        public UserApiController(ILogger<UserApiController> _logger, IConfiguration _Configuration)
        {
            _logger = logger;
            Configuration = _Configuration;
        }


        [HttpGet]
        public ActionResult Getch()
        {
            try
            {
                List<User> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<User>("select * from Users").ToList();

                    return Ok(lst);
                }
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                return BadRequest(new { Message = "Failed" + ex.Message });

            }
        }


        [HttpGet("{Id}")]

        public IActionResult Get(int id)
        {
            try
            {
                User lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<User>("select * from Users where Id=" + id).FirstOrDefault();


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

        public IActionResult Post(User lst)
        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst.Id = conn.ExecuteScalar<int>("select max(Id) from Users") + 1;

                    conn.Execute("insert into Users(Id,UserName,Password,Role,Godown,GSM,Size,ReelDia,Business,BF,Quality,Slip," +
                        "IsEditAllowed,IsDeleteAllowed,BackDateAllowed,manual,userallowed,Reprintallowed,Dispatch,Report,GodownTransfer,LocationTransfer,Location)" +
                        "values (@Id,@UserName,@Password,@Role,@Godown,@GSM,@Size,@ReelDia,@Business,@BF,@Quality,@Slip," +
                        "@IsEditAllowed,@IsDeleteAllowed,@BackDateAllowed,@manual,@userallowed,@Reprintallowed,@Dispatch,@Report,@GodownTransfer,@LocationTransfer,@Location)", lst);

                    return Ok("Created");
                    //return Ok(new { Message = "Created" });

                }

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest(new { Message = "Failed" + ex.Message });
            }
        }

        [HttpPut("{Id}")]

        public IActionResult put(int id, User lst)

        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    conn.Execute("update Users set UserName=@UserName ,Password=@Password,ReelDia=@ReelDia,Role=@Role," +
                        "Godown=@Godown,GSM=@GSM,Size=@Size,Business=@Business,BF=@BF,Quality=@Quality,Slip=@Slip," +
                        "IsEditAllowed=@IsEditAllowed,IsDeleteAllowed=@IsDeleteAllowed,BackDateAllowed=@BackDateAllowed," +
                        "manual=@manual,userallowed=@userallowed,Reprintallowed=@Reprintallowed,Dispatch=@Dispatch,Report=@Report,GodownTransfer=@GodownTransfer,LocationTransfer=@LocationTransfer,Location=@Location where Id=" + id, lst);


                   return Ok("Updated");
                   // return Ok(new { Message = "Updated" });

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

                    conn.Execute("Delete from Users where Id=" + id);



                    return Ok("Deleted");
                    //return Ok(new { Message = "Deleted" });
                }
            }
            catch (Exception ex)
            {
                string a = "constraint failed FOREIGN KEY constraint failed";
                if (ex.Message == a)
                {
                   return BadRequest("this is not deleted");
                   // return BadRequest(new { Message = "this is not deleted" });
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

