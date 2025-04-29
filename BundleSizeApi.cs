using FinishGoodStock.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinishGoodStock
{
    public class BundleSizeApi
    {

        //<<<<<<<<<<<<<<<<<<Size Get All>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static List<BundleSizeMaster> GetSize()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleSizeApi", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            var response = client.Get(request);
            List<BundleSizeMaster> Obj;

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BundleSizeMaster>>(response.Content);
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
                    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BundleSizeMaster>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }


        //<<<<<<<<<<<<<<<<<< Size Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static BundleSizeMaster GetSize(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleSizeApi/" + id, Method.Get);
            RestResponse response = client.Get(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<BundleSizeMaster>(response.Content);
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
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<BundleSizeMaster>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<Size Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PostSize(BundleSizeMaster size)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/BundleSizeApi", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(size);
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



        //<<<<<<<<<<<<<<<<<<<<<<<Size Put>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutSize(BundleSizeMaster size, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleSizeApi/" + id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(size);
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


        //<<<<<<<<<<<<<<<<< Size Delete >>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string DeleteSize(BundleSizeMaster size, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleSizeApi/" + Id, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(size);
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
