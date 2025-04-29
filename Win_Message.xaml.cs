
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
    /// Interaction logic for Win_Message.xaml
    /// </summary>
    public partial class Win_Message : Window
    {
        public FinishGoodStock.Utility.MessageResult MyResult { get; set; }

        public Win_Message(string Message, string Button1Text = "Yes", string Button2Text = "No", string Button3Text = "", int Width = 200)
        {
            InitializeComponent();
            MyMessage.Text = Message;
            Button1.Text = Button1Text;
            Button2.Text = Button2Text;
            Button3.Text = Button3Text;
            this.Width = Width;
            if (Button3Text != "")
            {
                Button3.Visibility = Visibility.Visible;
                M2.Visibility = Visibility.Visible;
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
                else if (e.Key == Key.Enter || e.Key == Key.Y || e.Key == Key.V)
                {
                    MyResult = Utility.MessageResult.Button1;
                    this.Close();
                }
                else if (e.Key == Key.N || e.Key == Key.L)
                {
                    MyResult = Utility.MessageResult.Button2;
                    this.Close();
                }
                else if (e.Key == Key.I)
                {
                    MyResult = Utility.MessageResult.Button3;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MyResult = Utility.MessageResult.Button1;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MyResult = Utility.MessageResult.Button2;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button3_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MyResult = Utility.MessageResult.Button3;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
