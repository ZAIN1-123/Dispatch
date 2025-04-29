using FinishGoodStock.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FinishGoodStock
{
    public class LocationTransferApi
    {
        //<<<<<<<<<<<<<<<<GET ALL>>>>>>>>>>>>>>>>>>>>>>
        public static List<LocationTransfer> GetLocationTransfers(DateTime FromDate, DateTime ToDate)
        {


            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/LocationTransfer", Method.Get);
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
            var response = client.Get(request);
            List<LocationTransfer> obj;
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GodownTransfer>>(response.Content);
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
                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LocationTransfer>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }
            return obj;


        }

        //<<<<<<<<<<<<<<<GodownTransferById>>>>>>>>>>>>>>

        public static LocationTransfer GetLocationTransfer(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/LocationTransfer/" + id, Method.Get);
            var response = client.Get(request);
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<GodownTransfer>(response.Content);
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
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<LocationTransfer>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        //<<<<<<<<<<<<<<<<<<Godown Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PostLocationTransfer(LocationTransfer locationTransfer)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/LocationTransfer", Method.Post);
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(locationTransfer);
            var response = client.Execute(request);
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

        //<<<<<<<<<<<<<<<<<<<Godown Put>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutLocationTransfer(LocationTransfer locationTransfer, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/LocationTransfer/" + id, Method.Put);
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(locationTransfer);
            var response = client.Execute(request);
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
            else
            {
                throw new Exception(response.Content);
            }
        }
        //<<<<<<<<<<<<<<<<<Godown Delete>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>..
        public static string DeleteLocationTransfer(LocationTransfer locationTransfer, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/LocationTransfer/" + id, Method.Delete);
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(locationTransfer);
            var response = client.Execute(request);
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
            var request = new RestRequest("/api/LocationTransferPdf/" + id, Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
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
    }



}
