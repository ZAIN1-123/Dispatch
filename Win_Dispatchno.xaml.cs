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
    /// Interaction logic for Win_Dispatchno.xaml
    /// </summary>
    public partial class Win_Dispatchno : Window
    {
       public string number;


        public Win_Dispatchno(string Bf)
        {
            try
            {
                InitializeComponent();
                this.number = number;
                txtNumber.Focus();
                txtNumber.Text = this.txtNumber.ToString();
                txtNumber.SelectAll();
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
                    this.Close();
                }
                if (e.Key == Key.Enter)
                {
                    try
                    {
                        this.number = txtNumber.Text.ToString();
                        this.Close();
                        e.Handled = true;
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = ex.Message;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.LostFocus(sender, e);
        }

       
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Utility.GotFocus(sender, e);
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Utility.Key_Down(sender, e);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            Utility.Key_Up(sender, e);
        }

        private void Ledger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                number = txtNumber.Text.ToString();
                //cmbLedger.Temp();
                Close();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
    }
}
