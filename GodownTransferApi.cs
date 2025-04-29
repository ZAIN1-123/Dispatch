using FinishGoodStock.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FinishGoodStock
{
    public class GodownTransferApi
    {
        //<<<<<<<<<<<<<<<<GET ALL>>>>>>>>>>>>>>>>>>>>>>
        public static List<GodownTransfer> GetGodownTransfers(DateTime FromDate, DateTime ToDate)
        {


            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/GodownTransfer", Method.Get);
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
            var response = client.Get(request);
            List<GodownTransfer> obj;
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
                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GodownTransfer>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }
            return obj;


        }

        //<<<<<<<<<<<<<<<GodownTransferById>>>>>>>>>>>>>>

        public static GodownTransfer GetGodownTransfer(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/GodownTransfer/" + id, Method.Get);
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
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<GodownTransfer>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        //<<<<<<<<<<<<<<<<<<Godown Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PostGodownTransfer(GodownTransfer godownTransfer)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/GodownTransfer", Method.Post);
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(godownTransfer);
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
        public static string PutGodownTransfer(GodownTransfer godownTransfer, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/GodownTransfer/" + id, Method.Put);
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(godownTransfer);
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
        public static string DeleteGodownTransfer(GodownTransfer godownTransfer, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/GodownTransfer/" + id, Method.Delete);
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(godownTransfer);
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
            var request = new RestRequest("/api/GodownTransferPdf/" + id, Method.Get);
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
