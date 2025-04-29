using FinishGoodStock.Models;
using FinishGoodStock.Views;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinishGoodStock
{
    /// <summary>
    /// Interaction logic for Win_Main.xaml
    /// </summary>
    public partial class Win_Main : Window
    {
        public Win_Main()
        {
            File.WriteAllText("test2.txt", "123");

            InitializeComponent();
            Utility.mainWindow = this;           
            Utility.GetReading();
            int RegStatus = MarwariCRM.Validate();
            if (RegStatus == 0)
            {

                MessageBox.Show("This system is not Registered.");
                Environment.Exit(0);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Escape)
            //{
            //    RemoveMenu();
            //    MarwariNavigator.GoBack();
            //}

        }

        private void ThemedWindow_ContentRendered(object sender, EventArgs e)
        {
            File.WriteAllText("test1.txt","123");
            VLogin login = new VLogin();
         

            Utility.mainWindow.MainFrame.Navigate(login);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var response = MessageBox.Show("Do you really want to exit?", "Exiting...",
                                               MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (response == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Temp");
                    foreach (string file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        public void RefreshMenu()
        {


            //RemoveMenu();
            RibbonTab tab = new RibbonTab();
            tab.Header = "Menu";
            tab.Name = "tabMaster"; tab.KeyTip = "Z";

            RibbonGroup group = new RibbonGroup();
            group.Header = "Master";

            RibbonButton button = new RibbonButton();


            //button.Label = "decimal";
            //button.Name = "btnresave";
            //button.KeyTip = "r";
            //button.Click += buttonresave;
            //button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/item.png"));
            //group.Items.Add(button);
            //button = new RibbonButton();

        
            if (Utility.LoginUser.userallowed == 1)
            {
                //button = new RibbonButton();
                button.Label = "User";
                button.Name = "btnUser";
                button.KeyTip = "D";
                button.Click += User_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/user.png"));
                group.Items.Add(button);
            }
            if (Utility.LoginUser.Business == 1)
            {
                button = new RibbonButton();
                button.Label = "Business";
                button.Name = "btnBusiness";
                button.KeyTip = "D";
                button.Click += Business_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/business.png"));
                group.Items.Add(button);
            }
            //button = new RibbonButton();
            //button.Label = "Role";
            //button.Name = "btnRole";
            //button.KeyTip = "D";
            //button.Click += Role_Click;
            //button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/Approve.png"));
            //group.Items.Add(button);


            if (Utility.LoginUser.Quality == 1)
            {
                button = new RibbonButton();
                button.Label = "Quality";
                button.Name = "btnItem";
                button.KeyTip = "D";
                button.Click += Item_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/item.png"));
                group.Items.Add(button);
            }

            if(Utility.LoginUser.Location==1)
            {
                button = new RibbonButton();
                button.Label = "Godown Location";
                button.Name = "btnGodownlocation";
                button.KeyTip = "G";
                button.Click += Btn_GodownLocation; ;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/ledger.png"));
                group.Items.Add(button);


                button = new RibbonButton();
                button.Label = "Godown Voucher";
                button.Name = "btnGodownVoucher";
                button.KeyTip = "G";
                button.Click += Btn_GodownVoucher;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/ledger.png"));
                group.Items.Add(button);




                button = new RibbonButton();
                button.Label = "Cutter Voucher";
                button.Name = "btnCutterVoucher";
                button.KeyTip = "G";
                button.Click += Btn_CutterVoucher;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/ledger.png"));
                group.Items.Add(button);
            }
           


            if (Utility.LoginUser.Godown == 1)
            {
                button = new RibbonButton();
                button.Label = "Godown";
                button.Name = "btnGodown";
                button.KeyTip = "D";
                button.Click += Godown_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/ledger.png"));
                group.Items.Add(button);
            }
            if (Utility.LoginUser.GSM == 1)
            {

                button = new RibbonButton();
                button.Label = "GSM";
                button.Name = "btnGSM";
                button.KeyTip = "D";
                button.Click += GSM_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stockJournal.png"));
                group.Items.Add(button);
            }
            if (Utility.LoginUser.Size == 1)
            {
                button = new RibbonButton();
                button.Label = "Size";
                button.Name = "btnSize";
                button.KeyTip = "D";
                button.Click += Size_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stockJournal.png"));
                group.Items.Add(button);
                
                button = new RibbonButton();
                button.Label = "BundleSize";
                button.Name = "btnBundleSize";
                button.KeyTip = "B";
                button.Click += BundleSize_Click; ;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stockJournal.png"));
                group.Items.Add(button);
            }
            if (Utility.LoginUser.BF == 1)
            {
                button = new RibbonButton();
                button.Label = "BF";
                button.Name = "btnBf";
                button.KeyTip = "D";
                button.Click += BF_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stockJournal.png"));
                group.Items.Add(button);
            }
            if (Utility.LoginUser.ReelDia == 1)
            {
                button = new RibbonButton();
                button.Label = "ReelDia";
                button.Name = "btnReelDia";
                button.KeyTip = "D";
                button.Click += ReelDia_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stockJournal.png"));
                group.Items.Add(button);
            }
            tab.Items.Add(group);
            //MyMenu.Items.Add(tab);

            group = new RibbonGroup();
            group.Header = "Voucher";
            if (Utility.LoginUser.Slip == 1)
            {
                button = new RibbonButton();
                button.Label = "Slip";
                button.Name = "btnSlip";
                button.KeyTip = "S";
                button.Click += Slip_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/slip.png"));
                group.Items.Add(button); 
                
                button = new RibbonButton();
                button.Label = "Bundle";
                button.Name = "btnBundle";
                button.KeyTip = "B";
                button.Click += Bundle_Click; ;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/slip.png"));
                group.Items.Add(button);
            }
            if (Utility.LoginUser.Dispatch == 1)
            {
                button = new RibbonButton();
                button.Label = "Dispatch";
                button.Name = "btnDispatch";
                button.KeyTip = "S";
                button.Click += Dispatch_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stock.png"));
                group.Items.Add(button);



                button = new RibbonButton();
                button.Label = "Bundle Dispatch";
                button.Name = "btnBundleDispatch";
                button.KeyTip = "B";
                button.Click += BtnBundleDispatch_Click; ;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stock.png"));
                group.Items.Add(button);


            }

            //if (Utility.LoginUser.Dispatch == 1)
            //{
            //    button = new RibbonButton();
            //    button.Label = "DispatchMeta";
            //    button.Name = "btnDispatchMeta";
            //    button.KeyTip = "S";
            //    button.Click += Dispatch1_Click; ;
            //    button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stock.png"));
            //    group.Items.Add(button);


            //}

            if (Utility.LoginUser.Dispatch == 1)
            {
                button = new RibbonButton();
                button.Label = "Dispatch Return";
                button.Name = "btnDispatchReturn";
                button.KeyTip = "S";
                button.Click += DispatchReturn_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/stock1.png"));
                group.Items.Add(button);
            }
            //if (Utility.LoginUser.GodownTransfer == 1)
            //{


            //    button = new RibbonButton();
            //    button.Label = "Godown Transfer";
            //    button.Name = "btnGodownTransfer";
            //    button.KeyTip = "S";
            //    button.Click += GodownTransfer_Click;
            //    button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
            //    group.Items.Add(button);

               
            //}
            if (Utility.LoginUser.LocationTransfer == 1)
            {


                button = new RibbonButton();
                button.Label = " Godown Location Transfer";
                button.Name = "btnLocationTransfer";
                button.KeyTip = "S";
                button.Click += Button_LocationTransferClick;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
                group.Items.Add(button);


            }
            tab.Items.Add(group);
            if (Utility.LoginUser.Report == 1)
            {
                group = new RibbonGroup();
                group.Header = "Report";

                button = new RibbonButton();
                button.Label = "Stock"; 
                button.Name = "btnReport";
                button.KeyTip = "S";
                button.Click += Report_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
                group.Items.Add(button);
                
                button = new RibbonButton();
                button.Label = "BundleStock"; 
                button.Name = "btnReport";
                button.KeyTip = "S";
                button.Click += BundleStock_Click; ;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
                group.Items.Add(button);
                if (Utility.text.Contains("BHAGESHWARIDISPATCH") || Utility.text.Contains("MARWARIZAINAB")) 
                {
                    button = new RibbonButton();
                    button.Label = "QualityReport";
                    button.Name = "btnReport";
                    button.KeyTip = "S";
                    button.Click += ReportClick;
                    button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
                    group.Items.Add(button);

                    button = new RibbonButton();
                    button.Label = "ProductionReport";
                    button.Name = "btnReport";
                    button.KeyTip = "S";
                    button.Click += Report_Click1;
                    button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
                    group.Items.Add(button);


                    button = new RibbonButton();
                    button.Label = "RepackingReport";
                    button.Name = "btnReport";
                    button.KeyTip = "S";
                    button.Click += Report_Click2; ;
                    button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
                    group.Items.Add(button);


                   
                }

                button = new RibbonButton();
                button.Label = "StockSummary";
                button.Name = "btnStockSummary";
                button.KeyTip = "S";
                button.Click += BtnStockSummary_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
                group.Items.Add(button);

                tab.Items.Add(group);
            }

            if (Utility.LoginUser.Location == 1)
            {
                button = new RibbonButton();
                button.Label = "LocationReport";
                button.Name = "btnReport";
                button.KeyTip = "S";
                button.Click += BtnLocationReport_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/report.png"));
                group.Items.Add(button);
            }
            group = new RibbonGroup();
                group.Header = "Utility";

                button = new RibbonButton();
                button.Label = "Backup";
                button.Name = "btnBackup";
                button.KeyTip = "B";
                button.Click += Backup_Click;
                button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/backup.png"));
                group.Items.Add(button);

                tab.Items.Add(group);

            MyMenu.Items.Add(tab);
            Utility.MainMenu = MyMenu;
            //Utility.MainMenu.Items.Add(tab);
            //Utility.MainMenu.SelectedIndex = Utility.MainMenu.Items.Count - 1;
        }

        private void BundleStock_Click(object sender, RoutedEventArgs e)
        {
            Report master = new Report("BundleStock");
            MarwariNavigator.Navigate(master);
        }

        private void BtnBundleDispatch_Click(object sender, RoutedEventArgs e)
        {
            VVoucherList master = new VVoucherList("BundleDispatch");
            MarwariNavigator.Navigate(master);
        }

        private void BundleSize_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("BundleSize");
            MarwariNavigator.Navigate(master);
        }

        private void Bundle_Click(object sender, RoutedEventArgs e)
        {
            VVoucherList voucher = new VVoucherList("Bundle");
            MarwariNavigator.Navigate(voucher);
        }

        
        private void Btn_CutterVoucher(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("CutterVoucher");
            MarwariNavigator.Navigate(master);
        }

        private void BtnStockSummary_Click(object sender, RoutedEventArgs e)
        {
            Report master = new Report("StockSummary");
            MarwariNavigator.Navigate(master);
        }

        private void Button_LocationTransferClick(object sender, RoutedEventArgs e)
        {
            VVoucherList master = new VVoucherList("Location Transfer");
            MarwariNavigator.Navigate(master);
        }

        private void BtnLocationReport_Click(object sender, RoutedEventArgs e)
        {
            Report master = new Report("LocationReport");
            MarwariNavigator.Navigate(master);
        }

        private void Btn_GodownVoucher(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("GodownVoucher");
            MarwariNavigator.Navigate(master);
        }

        private void Btn_GodownLocation(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("GodownLocation");
            MarwariNavigator.Navigate(master);
        }

        private void Report_Click2(object sender, RoutedEventArgs e)
        {
            Report master = new Report("RepackingReport");
            MarwariNavigator.Navigate(master);
        }

        private void Dispatch1_Click(object sender, RoutedEventArgs e)
        {
            Report master = new Report("DispatchMeta");
            MarwariNavigator.Navigate(master);
        }
         
        private void Report_Click1(object sender, RoutedEventArgs e)
        {
            Report master = new Report("ProductionReport");
            MarwariNavigator.Navigate(master);
        }

        private void buttonresave(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal v = 245.45m;
                MessageBox.Show(v.Round(0).ToString());

             

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void ReportClick(object sender, RoutedEventArgs e)
        {
            Report master = new Report("QualityReport");
            MarwariNavigator.Navigate(master);
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker backup = new BackgroundWorker();
            backup.DoWork += Backup_DoWork;
            backup.RunWorkerCompleted += Backup_RunWorkerCompleted;
            backup.RunWorkerAsync();
           
        }
        private void Backup_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    SaveFileDialog savefile = new SaveFileDialog();
                    savefile.FileName = "Backup " + Utility.LoginDate.ToDate().ToString("dd MMM yyyy HHmmss") + ".zip";
                    // set filters - this can be done in properties as well
                    savefile.Filter = "Zip files (*.zip)|*.*";

                    if (savefile.ShowDialog().Value)
                    {
                        string fileString = Backup.LoadBackup().FirstOrDefault();
                        //File.WriteAllText("filestring.txt",fileString);
                        File.WriteAllBytes(savefile.FileName, Convert.FromBase64String(fileString));

                        MessageBox.Show("Backup done.");
                    }
                }
                catch (Exception ex)
                {
                    
                    MessageBox.Show(ex.Message);
                }

            }, System.Windows.Threading.DispatcherPriority.Background);
        }
      
        private void Backup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
        private void Dispatch_Click(object sender, RoutedEventArgs e)
        {
            VVoucherList master = new VVoucherList("Dispatch");
            MarwariNavigator.Navigate(master);
        }
        private void GodownTransfer_Click(object sender, RoutedEventArgs e)
        {
            VVoucherList master = new VVoucherList("Godown Transfer");
            MarwariNavigator.Navigate(master);
        }
        private void DispatchReturn_Click(object sender, RoutedEventArgs e)
        {
            VVoucherList master = new VVoucherList("DispatchReturn");
            MarwariNavigator.Navigate(master);
        }
        private void Report_Click(object sender, RoutedEventArgs e)
        {
            Report master = new Report("Stock");
            MarwariNavigator.Navigate(master);
        }
        private void Item_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("Quality");
            MarwariNavigator.Navigate(master);
        }
        private void ReelDia_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("ReelDia");
            MarwariNavigator.Navigate(master);
        }
        private void User_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("User");
            MarwariNavigator.Navigate(master);
        }

        private void Business_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("Business");
            MarwariNavigator.Navigate(master);
        }

        //private void Role_Click(object sender, RoutedEventArgs e)
        //{
        //    VMasterList master = new VMasterList("Role");
        //    MarwariNavigator.Navigate(master);
        //}

        private void Godown_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("Godown");
            MarwariNavigator.Navigate(master);
        }
        private void GSM_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("GSM");
            MarwariNavigator.Navigate(master);
        }
        private void Size_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("Size");
            MarwariNavigator.Navigate(master);
        }
        private void BF_Click(object sender, RoutedEventArgs e)
        {
            VMasterList master = new VMasterList("BF");
            MarwariNavigator.Navigate(master);
        }
        private void Slip_Click(object sender, RoutedEventArgs e)
        {
            VVoucherList voucher = new VVoucherList("Slip");
            MarwariNavigator.Navigate(voucher);
        }
        public void RemoveMenu()
        {
            foreach (var obj in Utility.MainMenu.Items)
            {
                string sads = ((System.Windows.Controls.HeaderedItemsControl)obj).Header.ToString();
                if (sads == "Action")
                {
                    Utility.MainMenu.Items.RemoveAt(Utility.MainMenu.Items.Count - 1);
                    break;
                }
            }
        }
        private void lblLicenceUptoDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
