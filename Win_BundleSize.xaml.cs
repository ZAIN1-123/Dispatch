using FinishGoodStock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class Win_BundleSize : Window
    {
        public string selectedLedgerId;
        public string selectedLedgerName;
        DateTime date = DateTime.Now;
        List<BundleSizeMaster> listLedger = new List<BundleSizeMaster>();
        public Win_BundleSize(string Size)
        {
            InitializeComponent();
            cmbLedger.DisplayMemberPath = "SizeName";
            listLedger = BundleSizeApi.GetSize();
            List<BundleSizeMaster> combinedList = new List<BundleSizeMaster> { new BundleSizeMaster { SizeName = "All" } };
            //float f = float.Parse(combinedList.FirstOrDefault().Name);
            //var orderedList = combinedList.OrderBy(item => item.Name);

            // Then parse the float from the first item
            //float f = float.Parse(orderedList.FirstOrDefault().Name);
            combinedList.AddRange(listLedger);
            //combinedList = combinedList.OrderBy(x => x.Name).ToList();
            cmbLedger.ItemsSource = combinedList;
            // cmbLedger.ItemsSource = listLedger;
            cmbLedger.Temp();
            cmbLedger.txtSearch.Focus();

            selectedLedgerId = Size;

            if (!string.IsNullOrWhiteSpace(Size))
            {
                cmbLedger.SelectedValue = Size;
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
                selectedLedgerName = ((BundleSizeMaster)cmbLedger.SelectedItem).SizeName;
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
