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
    public class SlipApi 
    {

        //<<<<<<<<<<<<<<<<<<Slip Get All>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static List<Slip> GetSlip(DateTime FromDate, DateTime ToDate,string SetNo, string GodownId, string FormattedNo,string BF,string GSM,string Quality,string Size,string ReelDia,string Status)
        
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/api/SlipApi", Method.Get);
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
            if (SetNo != null)
            {
                request.AddParameter("SetNo",(SetNo));
            }
            if (GodownId != null)
            {
                request.AddParameter("GodownId", int.Parse(GodownId));
               
            }
            else if(Utility.Godown!=null)
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
            List<Slip> Obj;

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Slip>>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Slip>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }


        //<<<<<<<<<<<<<<<<<< Slip Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static Slip GetSlip(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/SlipApi/" + id, Method.Get);
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);
          

            RestResponse response = client.Get(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<Slip>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Slip>(response.Content);
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

        //<<<<<<<<<<<<<<<Slip Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static Slip PostSlip(Slip slip)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            //var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/SlipApi", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth",Utility.LAuth);
            request.AddHeader("ClientUSID",Utility.text);
            request.AddJsonBody(slip);
            RestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<Slip>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Slip>(response.Content);
                }
            }

            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Slip>(response.Content);

            }
            else
            {
                throw new Exception(response.Content);
            }
        }



        //<<<<<<<<<<<<<<<<<<<<<<<Slip Put>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutSlip(Slip slip, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/SlipApi/" + id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);
            request.AddJsonBody(slip);
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


        //<<<<<<<<<<<<<<<<< Slip Delete >>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string DeleteSlip(Slip slip, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/SlipApi/" + Id, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(slip);
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
            var request = new RestRequest("/api/Print/" + id, Method.Get);
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
        public static List<Slip> RollNo()
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
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Slip>>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Slip>>(response.Content);
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
        public static List<Slip> ReelCombo()
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/Reel", Method.Get);

            request.AddHeader("auth", Utility.LAuth);


            RestResponse response = client.Get(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Slip>>(response.Content);
            //}
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Slip>>(response.Content);
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
            var request = new RestRequest("/api/Reel/" + id, Method.Get);
            request.AddHeader("auth", Utility.LAuth);

            RestResponse response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
            }

            else   if (response.StatusCode == HttpStatusCode.OK)
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
        public static List<StockBook> ReelDispatch(int godown)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/ReelDispatchApi/"+godown, Method.Get);

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
