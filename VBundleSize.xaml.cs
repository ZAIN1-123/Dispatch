using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FinishGoodStock.Models;

namespace FinishGoodStock.Views
{
    /// <summary>
    /// Interaction logic for VSize.xaml
    /// </summary>
    public partial class VBundleSize : Page
    {
        BundleSizeMaster size;
        int Id;
        public VBundleSize(int Id)
        {
            this.Id = Id;
            InitializeComponent();
            LoadMenu();
            LoadData();
        }


        private void LoadData()
        {
            txtunit.ItemsSource = new Dictionary<string, string>() { { "INCH", "INCH" }, { "CM", "CM" } };
            if (Id == 0 || Id < 0)
            {
                size = new BundleSizeMaster();
                txtunit.SelectedValue = "INCH";
                txtunit.Temp();
            }
            else
            {
                size = BundleSizeApi.GetSize(Id);
            }
            if (size.Id > 0)
            {
                size = BundleSizeApi.GetSize(size.Id);
                txtName.Text = size.Name;
                txtunit.SelectedValue = size.Unit;
                txtunit.Temp();
                LoadMenu();
            }
            this.DataContext = size;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    if (Validate())
                    {
                        if (Id < 0 || Id == 0)
                        {
                            size.Name = txtName.Text;

                            string response = BundleSizeApi.PostSize(size);
                            if (response != "Created")
                            {
                                MessageBox.Show(response);
                            }
                            LoadMenu();
                            LoadData();
                            MarwariNavigator.GoBack();
                        }
                        else
                        {
                            size.Name = txtName.Text;

                            string response = BundleSizeApi.PutSize(size, Id);
                            if (response != "Updated")
                            {
                                MessageBox.Show(response);
                            }
                            LoadMenu();
                            LoadData();
                            MarwariNavigator.GoBack();

                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                    LoadMenu();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                LoadMenu();
            }
        }

        private bool Validate()
        {
            BundleSizeMasterValidator validator = new BundleSizeMasterValidator();
            FluentValidation.Results.ValidationResult result = validator.Validate(size);
            if (!result.IsValid)
            {
                string error = "";

                foreach (var err in result.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        error += Environment.NewLine;
                    }

                    error += err.ErrorMessage;
                }
                MessageBox.Show(error);
                return false;
            }

            return true;
        }

        public void LoadMenu()
        {
            RemoveMenu();
            RibbonTab tab = new RibbonTab();
            tab.Header = "Action";
            tab.Name = "tabAction"; tab.KeyTip = "Z";

            RibbonGroup group = new RibbonGroup();
            group.Header = "Menu";

            RibbonButton button = new RibbonButton();

            //if (Id < 0 || Id == 0  || Mylbl.Text == "Create Size")
            //{
            //    button = new RibbonButton();
            //    button.Label = "Add";
            //    button.Name = "btnAdd";
            //    button.Click += Create_Click;
            //    button.KeyTip = "A";
            //    button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/AddMaster.png"));
            //    group.Sizes.Add(button);
            //}

            button = new RibbonButton();
            button.Label = "Save";
            button.Name = "btnSave";
            button.Click += Save_Click;
            button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/save.png"));
            group.Items.Add(button);

            button = new RibbonButton();
            button.Label = "Close";
            button.Name = "btnClose";
            button.KeyTip = "C";
            button.Click += Close_Click;
            button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/close.png"));
            group.Items.Add(button);


            //button = new RibbonButton();
            //button.Label = "Refresh";
            //button.Name = "btnRefresh";
            //button.KeyTip = "R";
            //button.Click += Refresh_Click; ;
            //button.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Icons/refreshReport.png"));
            //group.Sizes.Add(button);


            tab.Items.Add(group);
            Utility.MainMenu.Items.Add(tab);
            Utility.MainMenu.SelectedIndex = Utility.MainMenu.Items.Count - 1;
        }


        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveMenu();

                VSize size = new VSize(Id);
                MarwariNavigator.Navigate(size);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void RemoveMenu()
        {
            foreach (var obj in Utility.MainMenu.Items)
            {
                string sads = ((System.Windows.Controls.HeaderedItemsControl)obj).Header.ToString();
                if (sads == "Action")
                {
                    Utility.MainMenu.Items.RemoveAt(Utility.MainMenu.Items.Count - 1);
                    break;
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (Utility.Show("Quit ?") == Utility.MessageResult.Button1)
                //{
                RemoveMenu();
                MarwariNavigator.GoBack();
                //}
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


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadMenu();
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                if (size.Id == 0 || size.Id < 0)
                {
                    Mylbl.Text = "Create Bundle Size";
                }
                if (size.Id > 0)
                {
                    Mylbl.Text = "Edit Bundle Size";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbunit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    txtunit.txtSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbunit_SelectionChanged(object sender, EventArgs e)
        {
            size.Unit = txtunit.SelectedValue.ToString();
            txtunit.Temp();
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.Key == Key.Escape)
                {
                    Close_Click(sender, new RoutedEventArgs(null, null));
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) && (e.Key == Key.S) || Keyboard.IsKeyDown(Key.RightCtrl) && (e.Key == Key.S))
                {

                    Save_Click(sender, new RoutedEventArgs(null, null));
                    e.Handled = true;
                }
                TextBox tb = e.Source as TextBox;
                if (tb != null)
                {
                    tb.Background = Brushes.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
