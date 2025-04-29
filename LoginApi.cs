using FinishGoodStock.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock
{
    public class LoginApi
    {

        //<<<<<<<<<<<<<<<<<<Item Get All>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static Login GetLogin(string UserName,string Password)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/LoginApi", Method.Get);
            request.AddHeader("UserName", UserName);
            request.AddHeader("Password", Password);
            var response = client.Get(request);
            Login Obj = new Login();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
            }
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    if (response.Content.Contains("message"))
            //    {
            //        throw new Exception(response.Content);
            //    }
            //    else
            //    {
            //        Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
            //    }
            //}
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (!response.Content.Contains("Message"))
                {
                    throw new Exception("Invalid UserName and Password");
                }
                else
                {
                    dynamic error = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
                    throw new Exception(error.Message.Value);
                }
            }
            else if (response.StatusCode == 0)
            {
                throw new Exception("Check Your Network Connection");
            }
           
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }

    }
}
