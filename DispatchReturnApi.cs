using FinishGoodStock.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace FinishGoodStock
{
    public class DispatchReturnApi
    {
        public static List<DispatchReturn> GetDispatchlist(DateTime FromDate, DateTime ToDate, string vehicle, string Dispatchno)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/DispatchReturn", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
            if (vehicle != null)
            {
                request.AddParameter("vehicle", vehicle);

            }
            if (Dispatchno != null)
            {
                request.AddParameter("Dispatchno", int.Parse(Dispatchno));

            }
            var response = client.Get(request);
            List<DispatchReturn> Obj = new List<DispatchReturn>();

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DispatchReturn>>(response.Content);
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
                    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DispatchReturn>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }



        //<<<<<<<<<<<<<<<<<< Dispatch Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static DispatchReturn GetDispatch(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/DispatchReturn/" + id, Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            RestResponse response = client.Get(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<DispatchReturn>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    //  Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<DispatchReturn>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<Dispatch Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PostDispatch(DispatchReturn dispatch)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/DispatchReturn", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            request.AddJsonBody(dispatch);
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
        //<<<<<<<<<<<<<<<Dispatch Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutDispatch(DispatchReturn dispatch, int id)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/DispatchReturn/" + id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            request.AddJsonBody(dispatch);
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
        //<<<<<<<<<<<<<<<<< Dispatch Delete >>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string DeleteDispatch(DispatchReturn dispatch, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/DispatchReturn/" + Id, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

            request.AddJsonBody(dispatch);
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

        public static string DoneDispatch(DispatchReturn dispatch, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/DispatchReturnPdf/" + Id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

            request.AddJsonBody(dispatch);
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
            var request = new RestRequest("/api/DispatchReturnPdf/" + id, Method.Get);
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

        public static List<DispatchReturn> Getvehicle()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/Vehicle", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            var response = client.Get(request);
            List<DispatchReturn> Obj = new List<DispatchReturn>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DispatchReturn>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }

        public static List<StockBook> ReelDispatch(int godown)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/DispatchReturnReel/" + godown, Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

            RestResponse response = client.Get(request);
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockBook>>(response.Content);
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
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockBook>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

        }
    }
}
