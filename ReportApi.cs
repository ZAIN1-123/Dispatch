using FinishGoodStock.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FinishGoodStock
{
    public class ReportApi
    {


        public static List<BundleStockBook> GetBundleStock(DateTime FromDate, DateTime ToDate, string GodownId, string FormattedNo, string BF, string GSM, string Quality, string ReelDia, string Size, string Alias)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/api/Report", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
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
            if (Alias != null)
            {
                request.AddParameter("Alias", Alias);
            }
            if (ReelDia != null)
            {
                request.AddParameter("ReelDia", int.Parse(ReelDia));
            }
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            var response = client.Get(request);

            List<BundleStockBook> Obj;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BundleStockBook>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }
        public static List<StockBook> Getstock(DateTime FromDate, DateTime ToDate, string GodownId, string FormattedNo, string BF, string GSM, string Quality,string ReelDia, string Size,string Alias)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/api/Report", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
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
            if (Alias != null)
            {
                request.AddParameter("Alias", Alias);
            }
            if (ReelDia != null)
            {
                request.AddParameter("ReelDia", int.Parse(ReelDia));
            }
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            var response = client.Get(request);

            List<StockBook> Obj;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockBook>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }

        public static List<StockBook> GetlocationStock(DateTime FromDate, DateTime ToDate, string GodownId, string FormattedNo, string BF, string GSM, string Quality, string ReelDia, string Size, string location,string LocationId, string Alias)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/api/Report", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
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
            if (Alias != null)
            {
                request.AddParameter("Alias", Alias);
            }
            if (ReelDia != null)
            {
                request.AddParameter("ReelDia", int.Parse(ReelDia));
            }
            if (location != null)
            {
                request.AddParameter("Location", int.Parse(location));
            }
            if (LocationId != null)
            {
                request.AddParameter("LocationId",  (LocationId));
            }
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            var response = client.Get(request);

            List<StockBook> Obj;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockBook>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }
        public static List<Slip> Getstock1(DateTime FromDate, DateTime ToDate, string GodownId, string FormattedNo, string BF, string GSM, string Quality, string ReelDia, string Size, string Alias)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/api/Report", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
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
            if (Alias != null)
            {
                request.AddParameter("Alias", Alias);
            }
            if (ReelDia != null)
            {
                request.AddParameter("ReelDia", int.Parse(ReelDia));
            }
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            var response = client.Get(request);

            List<Slip> Obj;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Slip>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }

        public static List<StockBook> Getstock2(DateTime FromDate, DateTime ToDate, string GodownId, string FormattedNo, string BF, string GSM, string Quality, string ReelDia, string Size, string Alias)
        {
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/api/Report", Method.Get);
            //request.RequestFormat = DataFormat.Json;
            request.AddParameter("FDate", int.Parse(FromDate.ToString("yyyyMMdd")));
            request.AddParameter("TDate", int.Parse(ToDate.ToString("yyyyMMdd")));
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
            if (Alias != null)
            {
                request.AddParameter("Alias", Alias);
            }
            if (ReelDia != null)
            {
                request.AddParameter("ReelDia", int.Parse(ReelDia));
            }
            request.AddHeader("auth", Utility.LAuth);
            request.AddHeader("ClientUSID", Utility.text);


            var response = client.Get(request);

            List<StockBook> Obj;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockBook>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }

            return Obj;
        }


    }
}
