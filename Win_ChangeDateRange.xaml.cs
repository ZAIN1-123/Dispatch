using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for Win_ChangeDateRange.xaml
    /// </summary>
    public partial class Win_ChangeDateRange : Window
    {
        public DateTime FromDate;
        public DateTime ToDate;

        public Win_ChangeDateRange(DateTime fromDate, DateTime toDate, string Hide = "")
        {
            try
            {
                InitializeComponent();
                FromDate = fromDate;
                ToDate = toDate;
                txtToDate.Text = ToDate.ToString("dd-MMM-yyyy");
                txtFromDate.Text = FromDate.ToString("dd-MMM-yyyy");
                txtFromDate.Focus();
                txtFromDate.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            //System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("hi-IN");
            try
            {
                if (e.Key == Key.Escape)
                {
                    this.Close();
                }
                if (e.Key == Key.Enter)
                {
                    lblError.Text = "";
                    try
                    {

                        //string abc = txtFromDate.Text.ToDate();

                        //if (DateTime.Parse(txtToDate.Text, cultureinfo) < DateTime.Parse(txtFromDate.Text, cultureinfo))
                        if (txtToDate.Text.ToDate() < txtFromDate.Text.ToDate())
                        {
                            txtFromDate.Text = FromDate.ToString("dd-MMM-yyyy");
                            txtToDate.Text = ToDate.ToString("dd-MMM-yyyy");
                            txtFromDate.Focus();
                            txtFromDate.SelectAll();
                            throw new Exception("From Date must be smaller than the TO Date");
                        }

                        TextBox t = e.Source as TextBox;
                        if (t != null)
                        {
                            t.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        }

                        txtToDate.SelectAll();

                        //FromDate = DateTime.Parse(txtFromDate.Text, cultureinfo);
                        //ToDate = DateTime.Parse(txtToDate.Text, cultureinfo);
                        FromDate = txtFromDate.Text.ToDate();
                        ToDate = txtToDate.Text.ToDate();
                        txtFromDate.Text = FromDate.ToString("dd-MMM-yyyy");
                        txtToDate.Text = ToDate.ToString("dd-MMM-yyyy");

                        if (t.Name == "txtToDate")
                        {
                            this.Close();
                        }
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

        private void Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            Utility.GotFocus(sender, e);
        }
    }
}
