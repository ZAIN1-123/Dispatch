using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FinishGoodStock
{
    public class Backup
    {
        public static List<string> LoadBackup()
     
        { 
            var options = new RestClientOptions(Utility.baseURL)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("api/Backup",Method.Get);
            request.RequestFormat = DataFormat.Json;

            //request.AddHeader("CloudEmail",CloudEmail);
            var response = client.Get(request);
            
            //dubara clana pdega space aara tha
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response.Content);
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

    }
}
