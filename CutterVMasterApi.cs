using DocumentFormat.OpenXml.Spreadsheet;
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
    public class CutterVMasterApi
    {

        //<<<<<<<<<<<<<<<<<<Item Get All>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        //public static List<CutterVMaster> GetCutterVoucher()
        public static List<CutterVMaster> GetCutterVoucher(DateTime FromDate, DateTime ToDate, string FormattedNo, string GSM, string Quality, string Size)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/CutterVMasterApi", Method.Get);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));


            if (FormattedNo != null)
            {
                request.AddParameter("FormattedNo", int.Parse(FormattedNo));
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

            request.AddHeader("auth", Utility.LAuth);
            var response = client.Get(request);
            List<CutterVMaster> Obj;


            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {
                    // Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(response.Content);
                    Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CutterVMaster>>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }


        //<<<<<<<<<<<<<<<<<< Item Get Particular>>>>>>>>>>>>>>>>>>>>>>>
        public static CutterVMaster GetCutterVoucher(int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/CutterVMasterApi/" + id, Method.Get);
            RestResponse response = client.Get(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content.Contains("message"))
                {
                    throw new Exception(response.Content);
                }
                else
                {

                    return Newtonsoft.Json.JsonConvert.DeserializeObject<CutterVMaster>(response.Content);
                }
            }
            else
            {
                throw new Exception(response.Content);
            }

        }



        //<<<<<<<<<<<<<<<Item Post>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PostCutterVoucher(CutterVMaster vg)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("/api/CutterVMasterApi", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(vg);
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

        //<<<<<<<<<<<<<<<<<<<<<<<Item Put>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string PutCutterVoucher(CutterVMaster vg, int id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/CutterVMasterApi/" + id, Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(vg);
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


        //<<<<<<<<<<<<<<<<< Item Delete >>>>>>>>>>>>>>>>>>>>>>>>>>>
        public static string DeleteCutterVoucher(CutterVMaster vg, int Id)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/api/CutterVMasterApi/" + Id, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("auth", Utility.LAuth);
            request.AddJsonBody(vg);
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
    }
}
