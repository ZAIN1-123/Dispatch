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
    /// Interaction logic for Win_Reelno.xaml
    /// </summary>
    public partial class Win_BF : Window
    {
        public string selectedLedgerId;
        public string selectedLedgerName;
        DateTime date = DateTime.Now;
        List<BF> listLedger = new List<BF>();
        public Win_BF(string Bf)
        {
            InitializeComponent();
            cmbLedger.DisplayMemberPath = "Name";
            listLedger = BFApi.GetBF();
            List<BF> combinedList = new List<BF> { new BF { Name = "All" } };
            combinedList.AddRange(listLedger);

            cmbLedger.ItemsSource = combinedList;
            //cmbLedger.ItemsSource = listLedger;
            cmbLedger.Temp();
            cmbLedger.txtSearch.Focus();

            selectedLedgerId = Bf;

            if (!string.IsNullOrWhiteSpace(Bf))
            {
                cmbLedger.SelectedValue = Bf;
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
                selectedLedgerName = ((BF)cmbLedger.SelectedItem).Name;
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
