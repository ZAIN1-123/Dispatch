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
    /// Interaction logic for Win_Status.xaml
    /// </summary>
    public partial class Win_Status : Window
    {
        public string Status;
        public string selectedLedgerName;

        public Win_Status(string Status)
        {
            InitializeComponent();
            List<KeyValuePair<int, string>> dicType = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(2, "All"),
            new KeyValuePair<int, string>(0, "Slip"),
            new KeyValuePair<int, string>(1, "Repacking")
        };


            cmbLedger.DisplayMemberPath = "Value";
            cmbLedger.SelectedValuePath = "Key";
            cmbLedger.ItemsSource = dicType;

            cmbLedger.txtSearch.Focus();
        }

        private void Ledger_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (cmbLedger.SelectedValue == null)
                {
                    cmbLedger.SelectedValue = Status;
                }

                Status = cmbLedger.SelectedValue.ToString();
                cmbLedger.Temp();
                Close();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private void cmbLedger_SelectionChanged(object sender, EventArgs e)
        {
            if (cmbLedger.SelectedValue != null)
            {
                Status = cmbLedger.SelectedValue.ToString();
                cmbLedger.Temp();
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
    }
}
