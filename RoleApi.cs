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
    public class RoleApi
    {

        //<<<<<<<<<<<<<<<<<<Item Get All>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static List<Role> GetRole()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/RoleApi", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            var response = client.Get(request);
            List<Role> Obj;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Role>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }


        //<<<<<<<<<<<<<<<<<< Item Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static Role GetRole(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/RoleApi/" + id, Method.Get);
            RestResponse response = client.Get(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Role>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<Item Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PostRole(Role role)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/RoleApi", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(role);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }
        }



        //<<<<<<<<<<<<<<<<<<<<<<<Item Put>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutRole(Role role, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/RoleApi/" + Id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(role);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }
        }


        //<<<<<<<<<<<<<<<<< Item Delete >>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string DeleteRole(Role role, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/RoleApi/" + Id, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(role);
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
