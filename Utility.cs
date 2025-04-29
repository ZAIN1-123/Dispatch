using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using FinishGoodStock.Models;
using Microsoft.AspNet.SignalR.Messaging;
using RJCP.IO.Ports;

namespace FinishGoodStock
{
    static public class Utility
    {
        public static string baseURL = File.ReadAllText("BaseUrl.txt");
        public static string text = File.ReadAllText("ClientUSID.txt");

        public static Login LoginUser { get; set; }
        public static Slip holdSlip { get; set; } = new Slip();
        public static Bundle holdbundle { get; set; } = new Bundle();
        public static GodownVMaster holdVoucher { get; set; } = new GodownVMaster();
        public static CutterVMaster holdCutterVoucher { get; set; } = new CutterVMaster();
        public static string Godown { get; set; }
        public static int LoginDate { get; set; }
        public static int StartDate { get; set; }
        public static int EndDate { get; set; }
        public static Win_Main mainWindow;
       
        public static string LName = "";
        public static string LUserName = "";
        public static string LPassword = "";
        public static string LAuth = "";
        public static System.Windows.Controls.Ribbon.Ribbon MainMenu { get; set; }
        //public static MenuList menuList;
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://crm.marwariplus.com"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
        public static decimal Round(this decimal myVal, int decimals = 2)
        {
            return Math.Round(myVal, decimals, MidpointRounding.AwayFromZero);
        }
        public static MyToken myToken { get; set; }
        public static int AuditTrail { get; set; }
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string LocalReading { get; set; }
        public static SerialPortStream serialPortLocal = new SerialPortStream("COM3");
        public static string displayLocal { get; set; }
        public static void GetReading()
        {
            //temp
            try
            {
                Utility.serialPortLocal.BaudRate = 1200;
                Utility.serialPortLocal.DtrEnable = true;
                Utility.serialPortLocal.DataBits = 8;
                Utility.serialPortLocal.StopBits = StopBits.One;
                Utility.serialPortLocal.Parity = Parity.None;
                Utility.serialPortLocal.DataReceived += OnDataReceivedLocal;
                if (!(Utility.serialPortLocal.IsOpen))
                {
                    Utility.serialPortLocal.Open();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private static void OnDataReceivedLocal(object sender, SerialDataReceivedEventArgs e)
        {
            string dataIN = Utility.serialPortLocal.ReadExisting();
            displayLocal += dataIN;

            //if (dataIN.Contains(Environment.NewLine))
            //{

            //    string[] words = displayLocal.Split('\n');

            //    MessageBox.Show(words.Count().ToString());

            //    for (int i = words.Length - 1; i >= 0; i--)
            //    {
            //        if (words[i].Contains("\r"))
            //        {
            //            string myval = words[i].Replace("\r", "");
            //            myval = myval.Replace(" KG", "");
            //            LocalReading = myval.Substring(1, myval.Length - 1).Trim();
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("inelse");
            //    displayLocal = dataIN;
            //}
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names

                dataTable.Columns.Add(prop.Name, type);

            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static void DisplayReading()
        {
            try
            {
                if (!string.IsNullOrEmpty(displayLocal))
                {
                    string str = displayLocal.Replace("wn", "").Replace("kg", Environment.NewLine);
                    displayLocal = "";
                    if (str.Contains(Environment.NewLine))
                    {
                        List<string> vs = str.Split('\n').ToList();
                        if (vs.Where(abc => abc.Trim().Length >= 4).Count() > 0)
                        {
                            str = vs.Where(abc => abc.Trim().Length >= 4).LastOrDefault();
                            // str = vs.LastOrDefault().Trim();
                            if (str.Length >= 4)
                            {
                                //Console.WriteLine(str.Replace("", "").Replace("k", "").Trim());
                                Utility.LocalReading = str.Replace("", "").Replace("k", "").Trim();
                            }
                            else if (str.Length == 1 && str == "0")
                            {
                                //Console.WriteLine(str.Replace("k", "").Trim());
                            }
                        }
                    }
                }
                //Utility.LocalReading = "90";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        public static void DisplayReadingOld()
        {
            try
            {
                if (!string.IsNullOrEmpty(displayLocal))
                {
                    string str = displayLocal.Replace(" ", Environment.NewLine);
                    displayLocal = "";
                    if (str.Contains(Environment.NewLine))
                    {
                        List<string> vs = str.Split('\n').ToList();
                        if (vs.Where(abc => abc.Trim().Length >= 4).Count() > 0)
                        {
                            str = vs.Where(abc => abc.Trim().Length >= 4).LastOrDefault();
                            // str = vs.LastOrDefault().Trim();
                            if (str.Length >= 4)
                            {
                                Utility.LocalReading = str.Replace("", "").Trim();
                            }
                            else if (str.Length == 1 && str == "0")
                            {
                                Utility.LocalReading = str.Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void DisplayReadingTehri()
        {
            try
            {
                if (!string.IsNullOrEmpty(displayLocal))
                {
                    string str = displayLocal.Replace(" ", Environment.NewLine);
                    displayLocal = "";
                    if (str.Contains(Environment.NewLine))
                    {
                        List<string> vs = str.Split('\n').ToList();
                        if (vs.Where(abc => abc.Trim().Length >= 4).Count() > 0)
                        {
                            str = vs.Where(abc => abc.Trim().Length >= 4).LastOrDefault();
                            // str = vs.LastOrDefault().Trim();
                            if (str.Length >= 4)
                            {
                                Utility.LocalReading = str.Replace("", "").Trim();
                            }
                            else if (str.Length == 1 && str == "0")
                            {
                                Utility.LocalReading = str.Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        public static DateTime? ToDate(this int? date)
        {
            if (date == null)
            {
                return null;
            }
            return date.ToDate();
        }
        public static void Key_Up(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Decimal)
                {

                }
                else if (e.Key == Key.OemPeriod)
                {

                }
                else
                {
                    TextBox ut = sender as TextBox;
                    if (ut != null)
                    {
                        BindingExpression be = ut.GetBindingExpression(TextBox.TextProperty);
                        if (be != null)
                        {
                            be.UpdateSource();
                        }
                    }

                    AutoCompleteBox acb = sender as AutoCompleteBox;
                    if (acb != null)
                    {
                        BindingExpression be = acb.GetBindingExpression(AutoCompleteBox.TextProperty);
                        if (be != null)
                        {
                            be.UpdateSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void Key_Down(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    TextBox T = sender as TextBox;

                    if (T != null)
                    {
                        T.Background = new SolidColorBrush(Colors.White);
                        T.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                        return;
                    }

                    PasswordBox P = sender as PasswordBox;

                    if (P != null)
                    {
                        P.Background = new SolidColorBrush(Colors.White);
                        P.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        return;
                    }

                    ComboBox C = sender as ComboBox;

                    if (C != null)
                    {
                        C.Background = new SolidColorBrush(Colors.White);
                        C.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        return;
                    }

                    //AutoCompleteBox A = sender as AutoCompleteBox;

                    //if (A != null)
                    //{
                    //    A.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    //    return;
                    //}

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox T = sender as TextBox;
                if (T != null)
                {
                    T.Background = new SolidColorBrush(Colors.LightCyan);
                    return;
                }

                PasswordBox P = sender as PasswordBox;
                if (P != null)
                {
                    //P.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    P.Background = new SolidColorBrush(Colors.LightCyan);
                    return;
                }

                ComboBox C = sender as ComboBox;
                if (C != null)
                {
                    //C.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    C.Background = new SolidColorBrush(Colors.LightCyan);
                    return;
                }

                //AutoCompleteBox ACB = sender as AutoCompleteBox;
                //if (ACB != null)
                //{
                //    //C.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                //    ACB.Background = new SolidColorBrush(Colors.LightCyan);
                //    return;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public static void LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox T = sender as TextBox;
                if (T != null)
                {
                    T.Background = new SolidColorBrush(Colors.White);
                    return;
                }

                PasswordBox P = sender as PasswordBox;
                if (P != null)
                {
                    //P.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    P.Background = new SolidColorBrush(Colors.White);
                    return;
                }

                ComboBox C = sender as ComboBox;
                if (C != null)
                {
                    //C.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    C.Background = new SolidColorBrush(Colors.White);
                    return;
                }

                AutoCompleteBox ACB = sender as AutoCompleteBox;
                if (ACB != null)
                {
                    //C.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    ACB.Background = new SolidColorBrush(Colors.White);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static DateTime ToDate(this string Mydate)
        {
            List<string> mylist = Mydate.Split('.', '/', '\\', '\'', ';', '-', ',').ToList();

            if (mylist.Count == 3)
            {
                try
                {
                    int d = int.Parse(mylist[0]);
                    int m = 0;
                    try
                    {
                        m = DateTime.ParseExact(mylist[1], "MMM", CultureInfo.CurrentCulture).Month;
                    }
                    catch
                    {
                        m = int.Parse(mylist[1]);
                    }

                    int y = int.Parse(mylist[2]);

                    if (y < 100) { y += 2000; }

                    return new DateTime(y, m, d);
                }
                catch
                {
                    return LoginDate.ToDate();
                }
            }
            else if (mylist.Count == 2)
            {
                try
                {
                    int d = int.Parse(mylist[0]);
                    int m = 0;
                    try
                    {
                        m = DateTime.ParseExact(mylist[1], "MMM", CultureInfo.CurrentCulture).Month;
                    }
                    catch
                    {
                        m = int.Parse(mylist[1]);
                    }

                    int y = LoginDate.ToDate().Year;

                    //if (m >= 4)
                    //{
                    //    y = StartDate.ToDate().Year;
                    //}
                    //else
                    //{
                    //    y = EndDate.ToDate().Year;
                    //}

                    return new DateTime(y, m, d);
                }
                catch
                {
                    return LoginDate.ToDate();
                }
            }
            else if (mylist.Count == 1)
            {
                try
                {
                    int d = int.Parse(mylist[0]);
                    int m = LoginDate.ToDate().Month;
                    int y = LoginDate.ToDate().Year;
                    return new DateTime(y, m, d);
                }
                catch
                {
                    return LoginDate.ToDate();
                }
            }
            return LoginDate.ToDate();
        }

        public static DateTime ToDate(this int date)
        {
            int d = date % 100;
            int m = (date / 100) % 100;
            int y = date / 10000;

            return new DateTime(y, m, d);
        }

        public static DateTime ToTime(this int date)
        {
            int SS = date % 100;
            int MM = (date / 100) % 100;
            int HH = date / 10000;

            return new DateTime(DateTime.Now.Year, 1, 1, HH, MM, SS);

        }

        public enum MessageResult
        {
            Button1,
            Button2,
            Button3
        }


        public static MessageResult Show(string strMessage, string str1 = "Yes", string str2 = "No", string str3 = "", int Width = 200)
        {
            Win_Message _Message = new Win_Message(strMessage, str1, str2, str3, Width);
            _Message.ShowDialog();
            return _Message.MyResult;
        }

    }
}
