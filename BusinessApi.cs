using FinishGoodStock.Models;
using FinishGoodStock.Views;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinishGoodStock
{
    public class BusinessApi
    {

        //<<<<<<<<<<<<<<<<<<Business Get All>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static List<Business> GetBusiness()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/Business", Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            var response = client.Get(request);
            List<Business> Obj;

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Business>>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    // Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
                    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Business>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }


        //<<<<<<<<<<<<<<<<<< Business Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static Business GetBusiness(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/Business/" + id, Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            RestResponse response = client.Get(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<Business>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    // Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Business>(response.Content);
                }
            }

            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<<<<<<<<<Business Put>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutBusiness(Business business, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/Business/" + id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(business);
            RestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    // Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);
                }
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);

            }
            else
            {
                throw new Exception(response.Content);
            }
        }
            
    }
}
