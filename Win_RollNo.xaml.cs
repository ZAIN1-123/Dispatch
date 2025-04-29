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
using FinishGoodStock.Models;

namespace FinishGoodStock
{
    /// <summary>
    /// Interaction logic for Win_RollNo.xaml
    /// </summary>
    
    public partial class Win_RollNo : Window
    {
        public string selectedLedgerId;
        public string selectedLedgerName;
        DateTime date = DateTime.Now;
        List<Slip> listLedger = new List<Slip>();
        public Win_RollNo(string SetNo)
        {
            InitializeComponent();
            cmbLedger.DisplayMemberPath = "SetNo";
            listLedger = SlipApi.RollNo();
            List<Slip> combinedList = new List<Slip> { new Slip { SetNo = "" } };            
            combinedList.AddRange(listLedger);

            cmbLedger.ItemsSource = combinedList;
            // cmbLedger.ItemsSource = listLedger;
            cmbLedger.Temp();
            cmbLedger.txtSearch.Focus();

            selectedLedgerId = SetNo;

            if (!string.IsNullOrWhiteSpace(SetNo))
            {
                cmbLedger.SelectedValue = SetNo;
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
                selectedLedgerName = ((Slip)cmbLedger.SelectedItem).SetNo;
                cmbLedger.Temp();
                //cmbLedger.Temp();
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
