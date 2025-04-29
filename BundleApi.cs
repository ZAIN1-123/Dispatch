using FinishGoodStock.Models;
using FinishGoodStock.Views;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FinishGoodStock
{
    public class BundleApi
    {

        //<<<<<<<<<<<<<<<<<<Bundle Get All>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static List<Bundle> GetBundle(DateTime FromDate, DateTime ToDate, string SetNo, string GodownId, string FormattedNo, string BF, string GSM, string Quality, string Size, string ReelDia, string Status)

        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleApi", Method.Get);
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
            if (SetNo != null)
            {
                request.AddParameter("SetNo", (SetNo));
            }
            if (GodownId != null)
            {
                request.AddParameter("GodownId", int.Parse(GodownId));

            }
            else if (Utility.Godown != null)
            {
                request.AddParameter("GodownId", int.Parse(Utility.Godown));

            }
            if (FormattedNo != null)
            {
                request.AddParameter("FormattedNo", int.Parse(FormattedNo));
            }
            if (BF != null)
            {
                request.AddParameter("BF", int.Parse(BF));
            }
            if (Quality != null)
            {
                request.AddParameter("Quality", int.Parse(Quality));
            }
            if (Size != null)
            {
                request.AddParameter("Size", int.Parse(Size));
            }
            if (GSM != null)
            {
                request.AddParameter("GSM", int.Parse(GSM));
            }
            if (ReelDia != null)
            {
                request.AddParameter("ReelDia", int.Parse(ReelDia));
            }
            if (Status != null)
            {
                request.AddParameter("Status", (Status));
            }
            request.AddHeader("auth", Utility.LAuth);
            var response = client.Get(request);
            List<Bundle> Obj;

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Bundle>>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Bundle>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }


        //<<<<<<<<<<<<<<<<<< Bundle Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static Bundle GetBundle(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleApi/" + id, Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            RestResponse response = client.Get(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<Bundle>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Bundle>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

        }


        public static string GetReelExist(string reelno)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/GodownReelApi/" + reelno, Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            RestResponse response = client.Get(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response.Content);
                dynamic responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return responseContent;
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

        public static string resave()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/ReWrite/StockBookResave", Method.Get);
            // request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);

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

        //<<<<<<<<<<<<<<<Bundle Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static Bundle PostBundle(Bundle Bundle)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            //var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/BundleApi", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);
            request.AddJsonBody(Bundle);
            RestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<Bundle>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Bundle>(response.Content);
                }
            }

            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Bundle>(response.Content);

            }
            else
            {
                throw new Exception(response.Content);
            }
        }



        //<<<<<<<<<<<<<<<<<<<<<<<Bundle Put>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutBundle(Bundle Bundle, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleApi/" + id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);
            request.AddJsonBody(Bundle);
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


        //<<<<<<<<<<<<<<<<< Bundle Delete >>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string DeleteBundle(Bundle Bundle, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleApi/" + Id, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(Bundle);
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


        public static byte[] LoadPrint(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/PrintBundle/" + id, Method.Get);
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
        //rollno combo
        public static List<Bundle> RollNo()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/RollNo", Method.Get);

            request.AddHeader("auth", Utility.LAuth);


            RestResponse response = client.Get(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Bundle>>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Bundle>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

        }

        //getparticularroll
        public static dynamic GetRollNo(string id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/RollNo/" + id, Method.Get);
            request.AddHeader("auth", Utility.LAuth);

            RestResponse response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
            }

            else if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
                }
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);

            }
            else
            {
                throw new Exception(response.Content);
            }

        }

        //<<<<<<<<<<<<<<<<<<<<<<<< reel combo >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static List<Bundle> ReelCombo()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/Bundle", Method.Get);

            request.AddHeader("auth", Utility.LAuth);


            RestResponse response = client.Get(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Bundle>>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Bundle>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<<<< Reel Get Particular >>>>>>>>>>>>>>>>>>>>>>>
        public static dynamic GetReel(string id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleCheck/" + id, Method.Get);
            request.AddHeader("auth", Utility.LAuth);

            RestResponse response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
            }

            else if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
                }
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);

            }
            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<<<<<<<<<< reel combo >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static List<BundleStockBook> ReelDispatch()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/BundleDispatchApi/", Method.Get);

            request.AddHeader("auth", Utility.LAuth);


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
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<BundleStockBook>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

        }


        public static List<StockBook> ReelLocation(int godown)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/ReelLocationhApi/" + godown, Method.Get);

            request.AddHeader("auth", Utility.LAuth);


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
