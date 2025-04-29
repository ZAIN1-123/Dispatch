using FinishGoodStock.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinishGoodStock
{
    /// <summary>
    /// Interaction logic for Win_Vehicle.xaml
    /// </summary>
    public partial class Win_Vehicle : Window
    {
        public string selectedLedgerId;
        public string selectedLedgerName;
        List<Dispatch> listLedger = new List<Dispatch>();
        List<BundleDispatch> BundlelistLedger = new List<BundleDispatch>();
        string Alias = "";
        public Win_Vehicle(string LedgerId, string LegderGroupId = "", string Alias = "")
        {
            InitializeComponent();
            this.Alias = Alias;
            cmbLedger.DisplayMemberPath = "VehicleNo";
            List<Dispatch> combinedList = new List<Dispatch>();
            List<BundleDispatch> combinedList1 = new List<BundleDispatch>();
            if (Alias=="Dispatch")
            {
                listLedger = DispatchApi.Getvehicle();
                combinedList = new List<Dispatch> { new Dispatch { VehicleNo = "All" } };
                combinedList.AddRange(listLedger);
            }
            else if(Alias=="BundleDispatch")
            {
                BundlelistLedger = BundleDispatchApi.Getvehicle();
                combinedList1 = new List<BundleDispatch> { new BundleDispatch { VehicleNo = "All" } };
                combinedList1.AddRange(BundlelistLedger);
            }


            if (Alias == "Dispatch")
            {
                cmbLedger.ItemsSource = combinedList;
                cmbLedger.ItemsSource = listLedger;
            }
            else if(Alias =="BundleDispatch")
            {
                cmbLedger.ItemsSource = combinedList1;
                cmbLedger.ItemsSource = BundlelistLedger;
            }

            cmbLedger.Temp();
            cmbLedger.txtSearch.Focus();

            selectedLedgerId = LedgerId;


            if (!string.IsNullOrWhiteSpace(LedgerId))
            {
                cmbLedger.SelectedValue = LedgerId;
                cmbLedger.Temp();

            }
        }

        private void cmbLedger_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                selectedLedgerId = cmbLedger.SelectedValue.ToString();
                cmbLedger.Temp();
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Ledger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbLedger.SelectedValue == null)
                {
                    cmbLedger.SelectedValue = selectedLedgerId;
                }

                selectedLedgerId = cmbLedger.SelectedValue.ToString();
                if(Alias=="Dispatch")
                {
                    selectedLedgerName = ((Dispatch)cmbLedger.SelectedItem).VehicleNo;

                }
                else if (Alias=="BundleDispatch")
                {
                    selectedLedgerName = ((BundleDispatch)cmbLedger.SelectedItem).VehicleNo;

                }
                cmbLedger.Temp();
                Close();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private void cmbLedger_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                    btnLedger.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}

