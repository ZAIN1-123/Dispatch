using DeviceId;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinishGoodStock
{
    static class MarwariCRM
    {
        public static string errorMessage { get; set; }

        public static decimal APIBalanceAsync()
        {
            string USID = File.ReadAllText("ClientUSID.txt");
            string deviceId = new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().AddUserName().ToString();
            string ComputerName = System.Environment.MachineName;
            RestClient newClient = new RestClient("http://crm.marwariplus.com/api/");
            RestRequest request = new RestRequest("MarwariAPI");
            request.AddHeader("Authorization", "TWFyd2FyU290d2FyZTpNYXJ3YXJpQCMxMjM=");
            //request.AddParameter("USID", "nalandaxpress@gmail.com");
            request.AddParameter("USID", USID);
            request.AddParameter("DeviceId", deviceId);
            request.AddParameter("ComputerName", ComputerName);


            var response = newClient.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                return decimal.Parse(response.Content);
            }
            else
            {
                errorMessage = response.StatusCode + " " + response.Content;
                return 0;
            }

        }

        public static decimal APIBalance()
        {
           

            string USID = File.ReadAllText("ClientUSID.txt");
            string deviceId = new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().AddUserName().ToString();
            string ComputerName = System.Environment.MachineName;
            RestClient newClient = new RestClient("http://crm.marwariplus.com/api/");
            RestRequest request = new RestRequest("MarwariAPI");
            request.AddHeader("Authorization", "TWFyd2FyU290d2FyZTpNYXJ3YXJpQCMxMjM=");
            //request.AddParameter("USID", "nalandaxpress@gmail.com");
            request.AddParameter("USID", USID);
            request.AddParameter("DeviceId", deviceId);
            request.AddParameter("ComputerName", ComputerName);
            var response = newClient.Get(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                return decimal.Parse(response.Content);
            }
            else
            {
                errorMessage = response.StatusCode + " " + response.Content;
                

                return 0;
            }

        }


        public static void DeductAPI(string remark, decimal quantity)
        {
            try
            {
                string USID = File.ReadAllText("ClientUSID.txt");
                string deviceId = new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().AddUserName().ToString();
                string ComputerName = System.Environment.MachineName;
                RestClient newClient = new RestClient("http://crm.marwariplus.com/api/");
                RestRequest request = new RestRequest("MarwariAPI", Method.Post);
                request.RequestFormat = RestSharp.DataFormat.Json;

                request.AddHeader("Authorization", "TWFyd2FyU290d2FyZTpNYXJ3YXJpQCMxMjM=");
                request.AddHeader("USID", USID);
                request.AddHeader("DeviceId", deviceId);
                request.AddHeader("ComputerName", ComputerName);

                Dictionary<string, string> obj = new Dictionary<string, string>();
                obj.Add("Remark", remark.Substring(0, Math.Min(remark.Length, 250)));
                obj.Add("Quantity", quantity.ToString());

                request.AddJsonBody(obj);

                RestResponse response = newClient.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                }
                else
                {
                    throw new Exception(response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static int Validate()
        {

            File.WriteAllText("1.txt", MarwariCRM.errorMessage);

            string deviceId = new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().AddUserName().ToString();


            Microsoft.Win32.RegistryKey keydeviceid;
            keydeviceid = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Marwari Software");

            //if (!File.Exists("ClientUSID.txt"))
            //{
            //    File.Create("ClientUSID.txt").Dispose();
            //    TextWriter tw = new StreamWriter("ClientUSID.txt");
            //    string dongleno = Database.GetOtherScalarText("Select Value from Activate where Column='Dongle'");
            //    tw.WriteLine(dongleno);
            //    tw.Close();
            //}
            File.WriteAllText("2.txt","123");

            string USID = File.ReadAllText("ClientUSID.txt");
            if (!Utility.CheckForInternetConnection())
            {
                if (keydeviceid.ValueCount == 0)
                {
                    
                    return 0;
                }
                string tokenJson = keydeviceid.GetValue(USID).ToString().Base64Decode();

                //off line
                if (tokenJson != null)
                {

                    Utility.myToken = JsonConvert.DeserializeObject<MyToken>(tokenJson);


                    if (deviceId != Utility.myToken.DeviceId)
                    {
                      

                        return 0;
                    }

                    if (long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) < Utility.myToken.ExpireOn)
                    {
                        if (Utility.myToken.LastOffLineRun == 0)
                        {
                            if (long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) < Utility.myToken.TokenOn)
                            {
                               

                                return 0;
                            }
                        }
                        else if (long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) < Utility.myToken.LastOffLineRun)
                        {
                           

                            return 0;
                        }

                        Utility.myToken.LastOffLineRun = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        //string json = JsonConvert.SerializeObject(Utility.myToken);

                        //File.WriteAllText("token\\useless.dat", json.Base64Encode());
                        string json = JsonConvert.SerializeObject(Utility.myToken);
                        // File.WriteAllText("token\\useless.dat", json.Base64Encode());
                        keydeviceid.SetValue(USID, json.Base64Encode());


                        return 1;
                    }
                    else
                    {
                      
                        return 0;
                    }
                }
                else
                {
                    
                    return 0;
                }
            }


            string ComputerName = System.Environment.MachineName;
            // RestClient newClient = new RestClient("http://localhost:4311/api/");

            RestClient newClient = new RestClient("http://crm.marwariplus.com/api/");
            RestRequest request = new RestRequest("Licence");
            request.AddHeader("Authorization", "TWFyd2FyU290d2FyZTpNYXJ3YXJpQCMxMjM=");
            //request.AddParameter("USID", "nalandaxpress@gmail.com");
            request.AddHeader("USID", USID);
            request.AddHeader("DeviceId", deviceId);
            request.AddHeader("ComputerName", ComputerName);

            string WindowVersion = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");

                foreach (ManagementObject os in searcher.Get())
                {
                    //MessageBox.Show(Newtonsoft.Json.JsonConvert.SerializeObject(os));
                    WindowVersion = os["Caption"].ToString();
                    break;
                }
            }
            catch(Exception ex)
            {
                WindowVersion = "Unable to Identify";

            }

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string ExeVersion = versionInfo.FileVersion;
            request.AddHeader("WindowVersion", WindowVersion);
            request.AddHeader("ExeVersion", ExeVersion);
            File.WriteAllText("abc.txt", "USID : " + USID + Environment.NewLine +
                "DeviceId : " + deviceId + Environment.NewLine + "ComputerName : " + ComputerName + Environment.NewLine +
                "WindowVersion : " + WindowVersion + Environment.NewLine + "ExeVersion : " + ExeVersion);

            var response = newClient.Get(request);

            //MessageBox.Show(response.StatusCode.ToString());
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //MessageBox.Show(response.Content);

                if (!Directory.Exists("token"))
                {
                    System.IO.Directory.CreateDirectory("token");
                }
                keydeviceid.SetValue(USID, response.Content.Base64Encode());
                //File.WriteAllText("token\\useless.dat", response.Content.Base64Encode());

                //MyToken token = new MyToken();
                //token.AuditTrail = 0;
                //token.DeviceId = deviceId;
                //token.ExpireOn = 20220806144510;
                //token.RegistionTime = 20220722143921;
                //token.SoftwareName = "M21 Standard Edition";
                //token.LastOffLineRun = 0;
                //token.LastPingTime = 20220722144008;
                //token.LicenceValidUpto = 20230331;
                //token.SoftwareSetting = "MHO181;MHO180;MHO179;MHO178;MHO177;MHO169;MHO170;MHO171;MHO172;MHO173;MHO174;MHO168;MHO164;MHO132;MHO163;MHO161;MHO162;MHO160;MHO159;MHO157;" +
                //    "MHO158;MHO156;MHO154;MHO155;MHO153;MHO151;MHO152;MHO129;MHO150;MHO149;MHO147;MHO148;MHO146;MHO145;MHO144;MHO143;MHO141;MHO142;MHO106;MHO130;MHO88;MHO137;MHO138;MHO139;" +
                //    "MHO136;MHO134;MHO135;MHO104;MHO124;MHO123;MHO87;MHO118;MHO117;MHO116;MHO108;MHO105;MHO103;MHO102;MHO99;MHO1;MHO2;MHO3;MHO4;MHO10;MHO13;MHO14;MHO15;MHO16;MHO17;MHO18;MHO19;" +
                //    "MHO20;MHO21;MHO22;MHO23;MHO24;MHO25;MHO26;MHO27;MHO32;MHO33;MHO34;MHO36;MHO37;MHO38;MHO39;MHO40;MHO41;MHO42;MHO43;MHO44;MHO47;MHO50;MHO51;MHO52;MHO53;MHO54;MHO55;MHO35;MHO56;" +
                //    "MHO57;MHO58;MHO59;MHO60;MHO61;MHO66;MHO67;MHO69;MHO70;MHO72;MHO73;MHO75;MHO80;MHO82;MHO83;MHO81;MHO85;MHO86;MHO84;MHO90;MHO91;MHO89;MHO93;MHO95;MHO96;MHO97;MHO98;MHO100;MHO101;";
                //token.TokenOn = 20220722144510;

                //{ "DeviceId":"HlRwe13dYphYZ4W04Ks0zmDp8bsBDRhE3U5xouoeUNw","RegistionTime":20220722143921,"LastPingTime":20220722144008,"TokenOn":20220722144510,"ExpireOn":20220806144510,"LastOffLineRun":0,"SoftwareName":"M21 Standard Edition","SoftwareSetting":"MHO181;MHO180;MHO179;MHO178;MHO177;MHO169;MHO170;MHO171;MHO172;MHO173;MHO174;MHO168;MHO164;MHO132;MHO163;MHO161;MHO162;MHO160;MHO159;MHO157;MHO158;MHO156;MHO154;MHO155;MHO153;MHO151;MHO152;MHO129;MHO150;MHO149;MHO147;MHO148;MHO146;MHO145;MHO144;MHO143;MHO141;MHO142;MHO106;MHO130;MHO88;MHO137;MHO138;MHO139;MHO136;MHO134;MHO135;MHO104;MHO124;MHO123;MHO87;MHO118;MHO117;MHO116;MHO108;MHO105;MHO103;MHO102;MHO99;MHO1;MHO2;MHO3;MHO4;MHO10;MHO13;MHO14;MHO15;MHO16;MHO17;MHO18;MHO19;MHO20;MHO21;MHO22;MHO23;MHO24;MHO25;MHO26;MHO27;MHO32;MHO33;MHO34;MHO36;MHO37;MHO38;MHO39;MHO40;MHO41;MHO42;MHO43;MHO44;MHO47;MHO50;MHO51;MHO52;MHO53;MHO54;MHO55;MHO35;MHO56;MHO57;MHO58;MHO59;MHO60;MHO61;MHO66;MHO67;MHO69;MHO70;MHO72;MHO73;MHO75;MHO80;MHO82;MHO83;MHO81;MHO85;MHO86;MHO84;MHO90;MHO91;MHO89;MHO93;MHO95;MHO96;MHO97;MHO98;MHO100;MHO101;","LicenceValidUpto":20230331,"AuditTrail":0}

                //try
                //{
                Utility.myToken = JsonConvert.DeserializeObject<MyToken>(response.Content);
                Utility.AuditTrail = Utility.myToken.AuditTrail;
                //}
                //catch
                //{
                //    Utility.myToken = token;
                //    MessageBox.Show(JsonConvert.SerializeObject(token));
                //    Utility.AuditTrail = 0;
                //}
                return 1;
            }
            else
            {
                return 0;
            }

        }


        public static int Validateold()
        {
            string deviceId = new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().AddUserName().ToString();

            if (!Utility.CheckForInternetConnection())
            {
                //off line
                if (File.Exists("token\\useless.dat"))
                {
                    string tokenJson = File.ReadAllText("token\\useless.dat", Encoding.UTF8).Base64Decode();

                    Utility.myToken = JsonConvert.DeserializeObject<MyToken>(tokenJson);


                    if (deviceId != Utility.myToken.DeviceId)
                    {
                        return 0;
                    }

                    if (long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) < Utility.myToken.ExpireOn)
                    {
                        if (Utility.myToken.LastOffLineRun == 0)
                        {
                            if (long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) < Utility.myToken.TokenOn)
                            {
                                return 0;
                            }
                        }
                        else if (long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) < Utility.myToken.LastOffLineRun)
                        {
                            return 0;
                        }

                        Utility.myToken.LastOffLineRun = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        string json = JsonConvert.SerializeObject(Utility.myToken);

                        File.WriteAllText("token\\useless.dat", json.Base64Encode());

                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }

            string USID = File.ReadAllText("ClientUSID.txt");

            string ComputerName = System.Environment.MachineName;
            // RestClient newClient = new RestClient("http://localhost:4311/api/");

            RestClient newClient = new RestClient("http://crm.marwariplus.com/api/");
            RestRequest request = new RestRequest("Licence");
            request.AddHeader("Authorization", "TWFyd2FyU290d2FyZTpNYXJ3YXJpQCMxMjM=");
            //request.AddParameter("USID", "nalandaxpress@gmail.com");
            request.AddHeader("USID", USID);
            request.AddHeader("DeviceId", deviceId);
            request.AddHeader("ComputerName", ComputerName);

            var response = newClient.Get(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (!Directory.Exists("token"))
                {
                    System.IO.Directory.CreateDirectory("token");
                }

                File.WriteAllText("token\\useless.dat", response.Content.Base64Encode());

                Utility.myToken = JsonConvert.DeserializeObject<MyToken>(response.Content);

                return 1;
            }
            else
            {
                return 0;
            }

        }

        public static int Surrender()
        {
            try
            {
                string USID = File.ReadAllText("ClientUSID.txt");
                string deviceId = new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().AddUserName().ToString();
                string ComputerName = System.Environment.MachineName;
                RestClient newClient = new RestClient("http://crm.marwariplus.com//api/");
                RestRequest request = new RestRequest("Surrender");
                request.AddHeader("Authorization", "TWFyd2FyU290d2FyZTpNYXJ3YXJpQCMxMjM=");
                //request.AddParameter("USID", "nalandaxpress@gmail.com");
                request.AddParameter("USID", USID);
                request.AddParameter("DeviceId", deviceId);
                request.AddParameter("ComputerName", ComputerName);
                var response = newClient.Get(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(response.Content);
                    //bool status = (response.Content == "1");
                    //if (status == false) { errorMessage = "STOP"; }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }
    }

    public class MyToken
    {
        public string DeviceId { get; set; }
        public long RegistionTime { get; set; }
        public long LastPingTime { get; set; }
        public long TokenOn { get; set; }
        public long ExpireOn { get; set; }
        public long LastOffLineRun { get; set; } = 0;
        public string SoftwareName { get; set; }
        public string SoftwareSetting { get; set; }
        public int LicenceValidUpto { get; set; }
        public int AuditTrail { get; set; }

    }
}
