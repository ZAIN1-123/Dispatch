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
    /// Interaction logic for Win_Godown.xaml
    /// </summary>
    public partial class Win_Godown : Window
    {
        public string selectedLedgerId;
        public string selectedLedgerName;
        List<Godown> listLedger = new List<Godown>();
        public Win_Godown(string LedgerId, string LegderGroupId = "", string Alias = "")
        {
            InitializeComponent();

            //cmbLedger.DisplayMemberPath = "Name";

            //listLedger = GodownApi.GetGodown();

            //cmbLedger.ItemsSource = listLedger;

            cmbLedger.DisplayMemberPath = "Name";
            List<Godown> listLedger = GodownApi.GetGodown();
            List<Godown> combinedList = new List<Godown> { new Godown { Name = "All" } };
            combinedList.AddRange(listLedger);

            cmbLedger.ItemsSource = combinedList;

         

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
                Utility.Godown = selectedLedgerId;
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
                selectedLedgerName = ((Godown)cmbLedger.SelectedItem).Name ;
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
