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
    public partial class Win_Quality : Window
    {
        public string selectedLedgerId;
        public string selectedLedgerName;
        DateTime date = DateTime.Now;
        List<Item> listLedger = new List<Item>();
        public Win_Quality(string Quality)
        {
            InitializeComponent();
            cmbLedger.DisplayMemberPath = "Name";
            listLedger = ItemApi.GetItem();
            List<Item> combinedList = new List<Item> { new Item { Name = "All" } };
            combinedList.AddRange(listLedger);

            cmbLedger.ItemsSource = combinedList;
            //cmbLedger.ItemsSource = listLedger;
            cmbLedger.Temp();
            cmbLedger.txtSearch.Focus();

            selectedLedgerId = Quality;

            if (!string.IsNullOrWhiteSpace(Quality))
            {
                cmbLedger.SelectedValue = Quality;
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
                selectedLedgerName = ((Item)cmbLedger.SelectedItem).Name;
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
