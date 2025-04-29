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
     class ExcelApi
    {
        public static string Import(string id, string file)
        {
            RestRequest request;
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            request = new RestRequest("Import/" + id, Method.Post);
            request.RequestFormat = DataFormat.Json;

           
            request.AddHeader("Content-Type", "application/json");           
            request.AddParameter("application/json", "\"" + file + "\"", ParameterType.RequestBody);
            RestResponse response = client.Get(request);    
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content;
            }
            else if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                Dictionary<int, string> errors = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(response.Content);
                string errorMsg = "";
                foreach (KeyValuePair<int, string> err in errors.OrderBy(abc => abc.Key))
                {
                    errorMsg += "Row(" + err.Key + ") " + err.Value + Environment.NewLine;
                }

                System.Windows.Clipboard.SetText(errorMsg);

                return errorMsg;
            }
            else
            {
                throw new Exception(response.Content);
            }
        }
    }
}
