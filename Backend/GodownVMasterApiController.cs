using Dapper;
using DISPATCHAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

    public class GodownVMasterApiController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<GodownVMasterApiController> logger;
        public GodownVMasterApiController(ILogger<GodownVMasterApiController> _logger, IConfiguration _Configuration)
        {
            _logger = logger;
            Configuration = _Configuration;
        }
         

        [HttpGet]
        public ActionResult Getch()
        {
            try
            {
                List<GodownVMaster> lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    lst = conn.Query<GodownVMaster>("select GodownVMaster.*, " +
                        " GodownLocation.Name as Location from GodownVMaster " +
                        " left join GodownLocation on GodownLocation.Id=GodownVMaster.LocationId").ToList();

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
                GodownVMaster lst;
                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {

                    lst = conn.Query<GodownVMaster>("select * from GodownVMaster where Id=" + id).FirstOrDefault();


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

        public IActionResult Post(GodownVMaster lst)
        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'",transaction).FirstOrDefault();
                            //int Exist = conn.ExecuteScalar<int>("select Count(*) from StockBook where ReelNumber ='" + lst.ReelNo + "'",transaction);
                            //lst.SlipId = conn.QuerySingleOrDefault<int>("SELECT MIN(SlipId) FROM StockBook WHERE ReelNumber = @ReelNumber",new { ReelNumber = lst.ReelNo },transaction);
                            var result = conn.QuerySingleOrDefault<int?>( "SELECT MIN(SlipId) FROM StockBook WHERE ReelNumber = @ReelNumber",new { ReelNumber = lst.ReelNo },transaction);                          
                            if (result == null)
                            {                                
                                lst.SlipId = 0; 
                            }
                            else
                            {
                                lst.SlipId = result.Value;  // Use the actual value if it's not null
                            }


                            if (lst.SlipId ==0)
                            {
                                transaction.Rollback();
                                return NotFound(" Invalid QR Code");
                                //return NotFound(new { Message = stock.Number + "Invalid QR Code" });
                            }
                            int Location = conn.ExecuteScalar<int>("SELECT MIN(LocationId) FROM GodownVMaster WHERE ReelNo = @ReelNo", new { ReelNo = lst.ReelNo }, transaction);
                            if (Location != 0)
                            {
                                transaction.Rollback();
                                return Ok("Location Alloted Already");
                            }
                            lst.Id = conn.ExecuteScalar<int>("select max(Id) from GodownVMaster") + 1;
                            lst.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            conn.Execute("insert into GodownVMaster(Id,ReelNo,LocationId,SlipId,Date)values (@Id,@ReelNo,@LocationId,@SlipId,@Date)", lst,transaction);
                            transaction.Commit();
                            return Ok("Created");//return Ok(new { Message = "Created" });                   

                        }
                        catch(Exception e)
                        {
                            transaction.Rollback();
                            return BadRequest(new { Message = "Failed" + e.Message });
                        }
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

        public IActionResult put(int id, GodownVMaster lst)

        {
            try
            {

                string ConnString = this.Configuration.GetConnectionString("MyConn");
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string auth = Request.Headers["auth"].FirstOrDefault();
                            string decodedStrings = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                            string[] credentials = decodedStrings.Split(':');
                            string username = credentials[0];
                            string password = credentials[1];
                            User user = conn.Query<User>($"Select * from Users where UserName='{username}' and Password='{password}'").FirstOrDefault();
                            if (user.IsEditAllowed == 1)
                            {


                                var result = conn.QuerySingleOrDefault<int?>("SELECT MIN(SlipId) FROM StockBook WHERE ReelNumber = @ReelNumber", new { ReelNumber = lst.ReelNo }, transaction);
                                if (result == null)
                                {
                                    lst.SlipId = 0;
                                }
                                else
                                {
                                    lst.SlipId = result.Value;  // Use the actual value if it's not null
                                }


                                if (lst.SlipId == 0)
                                {
                                    transaction.Rollback();
                                    return NotFound(" Invalid QR Code");
                                    //return NotFound(new { Message = stock.Number + "Invalid QR Code" });
                                }
                                int locationId = lst.LocationId;
                                lst.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

                                conn.Execute("update GodownVMaster set ReelNo=@ReelNo,LocationId="+ locationId + ",SlipId=@SlipId,Date=@Date where Id=" + id, lst,transaction);

                                transaction.Commit();
                                return Ok("Updated");
                                // return Ok(new { Message = "Updated" });


                            }
                            else
                            {
                                return BadRequest("you are not allowed");
                                //return BadRequest(new { Message = "you are not allowed" });
                            }

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return BadRequest(new { Message = "Failed" + ex.Message });
                        }
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
                        conn.Execute("Delete from GodownVMaster where Id=" + id);



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






