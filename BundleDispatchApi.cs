using FinishGoodStock.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace FinishGoodStock
{
    public class BundleDispatchApi
    {


        public static string resave(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/ReWrite/BundleDispatchResave", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("id", id);
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
        public static List<BundleDispatch> GetBundleDispatchlist(DateTime FromDate, DateTime ToDate, string vehicle, string BundleDispatchno)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleDispatch", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
            if (vehicle != null)
            {
                request.AddParameter("vehicle", vehicle);

            }
            if (BundleDispatchno != null)
            {
                request.AddParameter("BundleDispatchno", int.Parse(BundleDispatchno));

            }
            var response = client.Get(request);
            List<BundleDispatch> Obj = new List<BundleDispatch>();

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BundleDispatch>>(response.Content);
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
                    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BundleDispatch>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }



        //<<<<<<<<<<<<<<<<<< BundleDispatch Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static BundleDispatch GetBundleDispatch(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleDispatch/" + id, Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            RestResponse response = client.Get(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<BundleDispatch>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<BundleDispatch Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PostBundleDispatch(BundleDispatch BundleDispatch)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/BundleDispatch", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            request.AddJsonBody(BundleDispatch);
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
                    //return Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
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
        //<<<<<<<<<<<<<<<BundleDispatch Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutBundleDispatch(BundleDispatch BundleDispatch, int id)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/BundleDispatch/" + id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            request.AddJsonBody(BundleDispatch);
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
                    //Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
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
        //<<<<<<<<<<<<<<<<< BundleDispatch Delete >>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string DeleteBundleDispatch(BundleDispatch BundleDispatch, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleDispatch/" + Id, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

            request.AddJsonBody(BundleDispatch);
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
                    //Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
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

        public static string DoneBundleDispatch(BundleDispatch BundleDispatch, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundlePdf/" + Id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

            request.AddJsonBody(BundleDispatch);
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
        public static byte[] LoadPrint(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundlePdf/" + id, Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (!response.Content.Contains("Message"))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(response.Content);
                }
                else
                {
                    dynamic error = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
                    throw new Exception(error.Message.Value);
                }
            }
            else
            {
                throw new Exception(response.ErrorMessage);
            }

        }

        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        public static List<BundleDispatch> Getvehicle()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleVehicle", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            var response = client.Get(request);
            List<BundleDispatch> Obj = new List<BundleDispatch>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BundleDispatch>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }

        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static Slip ReelGodown(string reel)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/GodownBundleDispatch/" + reel, Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

            RestResponse response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Slip>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

        }
    }
}
