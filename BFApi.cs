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
    public class BFApi
    {

        //<<<<<<<<<<<<<<<<<<BF Get All>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static List<BF> GetBF()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BFApi", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            var response = client.Get(request);
            List<BF> Obj;

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BF>>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    // Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<BF>(response.Content);
                    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BF>>(response.Content);
                }
            }

            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }


        //<<<<<<<<<<<<<<<<<< BF Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static BF GetBF(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BFApi/" + id, Method.Get);
            RestResponse response = client.Get(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<BF>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<BF Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PostBF(BF Bf)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/BFApi", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(Bf);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);
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



        //<<<<<<<<<<<<<<<<<<<<<<<BF Put>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutBF(BF Bf, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BFApi/" + id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(Bf);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);
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


        //<<<<<<<<<<<<<<<<< BF Delete >>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string DeleteBF(BF Bf, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BFApi/" + Id, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(Bf);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);
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
