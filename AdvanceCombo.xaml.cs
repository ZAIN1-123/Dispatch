using Dynamitey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinishGoodStock
{
    /// <summary>
    /// Interaction logic for AdvanceCombo.xaml
    /// </summary>
    public partial class AdvanceCombo : UserControl
    {
        public event EventHandler SelectionChanged;
        public event EventHandler CreateClick;
        public event EventHandler AlterClick;
        public event EventHandler DeletePress;

        public IEnumerable ItemsSource { get; set; }

        public string DisplayMemberPath { get; set; }
        public object SelectedItem { get; set; }
        public string SelectedValuePath { get; set; }

        List<ComboDisplay> ComboDisplayMembers = new List<ComboDisplay>();

        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(AdvanceCombo), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool DisplayCreate { get; set; }

        public bool DisplayAlter { get; set; }

        private void SetSearchBoxWidth()
        {
            try
            {
                MySearch.Width = 700;
                if (DisplayCreate)
                {
                    btnCreate.Visibility = Visibility.Visible;
                }
                else
                {
                    btnCreate.Visibility = Visibility.Collapsed;
                    MySearch.Width += 100;
                }
                if (DisplayAlter)
                {
                    btnAlter.Visibility = Visibility.Visible;
                }
                else
                {
                    btnAlter.Visibility = Visibility.Collapsed;
                    MySearch.Width += 100;
                }

                MySearch.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public AdvanceCombo()
        {
            try
            {
                InitializeComponent();
                MyDataGrid.ItemsSource = this.ItemsSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Bind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Bind();
        }

        private void Bind()
        {
            try
            {
                if (MyPopup.IsOpen == false)
                {
                    SetSearchBoxWidth();
                    MyPopup.IsOpen = true;

                }

                SetDisplayMember();

                MySearch.Focus();

                MyDataGrid.Columns.Clear();


                string where = "";
                foreach (ComboDisplay displayMember in ComboDisplayMembers)
                {
                    if (displayMember.DisplayOnGrid == 1)
                    {
                        var col = new DataGridTextColumn();
                        Style MyCellStyle;

                        if (displayMember.Alignment == "R")
                        {
                            col.HeaderStyle = new Style(typeof(DataGridColumnHeader));
                            col.HeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Right));
                            MyCellStyle = new Style(typeof(DataGridCell)) { Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right) } };

                        }
                        else if (displayMember.Alignment == "C")
                        {
                            col.HeaderStyle = new Style(typeof(DataGridColumnHeader));
                            col.HeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
                            MyCellStyle = new Style(typeof(DataGridCell)) { Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) } };

                        }
                        else
                        {
                            col.HeaderStyle = new Style(typeof(DataGridColumnHeader));
                            col.HeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Left));
                            MyCellStyle = new Style(typeof(DataGridCell)) { Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left) } };
                        }

                        col.CellStyle = MyCellStyle;

                        col.Width = new DataGridLength(displayMember.Width, DataGridLengthUnitType.Star);

                        col.Header = displayMember.HeaderText;
                        col.Binding = new Binding(displayMember.FieldName);
                        MyDataGrid.Columns.Add(col);
                    }
                    if (where != "")
                    {
                        where += " or ";
                    }

                    where += displayMember.FieldName + ".ToUpper().Replace(\".\", \"\").Replace(\" \", \"\").Contains(@0)";
                }

                //MyPopup.IsOpen = false;
                string ToSearch = MySearch.Text.ToUpper().Replace(".", "").Replace(" ", "");

                string OrderBy = ComboDisplayMembers.FirstOrDefault().FieldName + ".ToUpper().IndexOf(\"" + ToSearch + "\")";
                OrderBy = OrderBy + "<0?99999:" + OrderBy;

                MyDataGrid.ItemsSource = this.ItemsSource.AsQueryable()
                    .Where(where, ToSearch)
                   .OrderBy(OrderBy);
                MyDataGrid.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SetDisplayMember() 
        {
            try
            {
                List<string> DisplayMembers = this.DisplayMemberPath.Split(',').ToList();
                ComboDisplayMembers = new List<ComboDisplay>();
                int counter = 1;
                foreach (string Member in DisplayMembers)
                {
                    ComboDisplay comboDisplay = new ComboDisplay();
                    comboDisplay.Id = counter++;
                    comboDisplay.Value = Member;
                    if (Member.Contains("="))
                    {
                        comboDisplay.FieldName = Member.Replace("!", "").Replace("%", "").Replace("^", "").Replace(">", "").Split(new string[] { "=" }, StringSplitOptions.None)[1];
                        comboDisplay.HeaderText = Member.Replace("!", "").Replace("%", "").Replace("^", "").Replace(">", "").Split(new string[] { "=" }, StringSplitOptions.None)[0];
                    }
                    else
                    {
                        comboDisplay.FieldName = Member.Replace("!", "").Replace("%", "").Replace("^", "").Replace(">", "");
                        comboDisplay.HeaderText = Member.Replace("!", "").Replace("%", "").Replace("^", "").Replace(">", "");
                    }

                    if (comboDisplay.FieldName.Contains(":"))
                    {
                        comboDisplay.Width = float.Parse(comboDisplay.FieldName.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                        comboDisplay.FieldName = comboDisplay.FieldName.Split(new string[] { ":" }, StringSplitOptions.None)[0];
                        comboDisplay.HeaderText = comboDisplay.FieldName.Split(new string[] { ":" }, StringSplitOptions.None)[0];
                    }
                    else
                    {
                        comboDisplay.Width = 1;
                    }


                    if (Member.Contains("!") || Member.Contains("%"))
                    {
                        comboDisplay.DisplayOnGrid = 0;
                    }
                    else
                    {
                        comboDisplay.DisplayOnGrid = 1;
                    }

                    if (Member.Contains("^"))
                    {
                        comboDisplay.Alignment = "C";
                    }
                    else if (Member.Contains(">"))
                    {
                        comboDisplay.Alignment = "R";
                    }
                    else
                    {
                        comboDisplay.Alignment = "L";
                    }

                    ComboDisplayMembers.Add(comboDisplay);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MyPopup_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    MySearch.Text = "";
                    MyPopup.IsOpen = false;
                    if (MyDataGrid.Items.Count > 0)
                    {
                        var border = VisualTreeHelper.GetChild(MyDataGrid, 0) as Decorator;
                        if (border != null)
                        {
                            var scroll = border.Child as ScrollViewer;
                            if (scroll != null) scroll.ScrollToTop();
                        }
                    }
                    txtSearch.Focus();
                }
                if (e.Key == Key.Back)
                {
                    if (string.IsNullOrWhiteSpace(MySearch.Text))
                        e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MySearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Down)
                {
                    e.Handled = true;
                    MySearch.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    
                }
                else if (e.Key == Key.Enter)
                {
                    RecordSelected();
                    txtSearch.Background = new SolidColorBrush(Colors.White);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Bind();
        }

        private void MySearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Bind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!(e.Key == Key.Enter || e.Key == Key.Tab || e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Escape || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.F11 || e.Key == Key.F7))
                {
                    Bind();
                }
                else if (e.Key == Key.Enter)
                {
                    TextBox T = sender as TextBox;

                    if (T != null)
                    {
                        T.Background = new SolidColorBrush(Colors.White);
                        //return;
                    }

                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RecordSelected()
        {
            try
            {
                MySearch.Text = "";
                IEnumerable enumerable = MyDataGrid.SelectedItems;

                SelectedItem = enumerable.Cast<object>().ToList()[0];


                txtSearch.Text = this.SelectedItem.GetType().GetProperty(ComboDisplayMembers.FirstOrDefault().FieldName).GetValue(this.SelectedItem).ToString();

                SelectedValue = this.SelectedItem.GetType().GetProperty(this.SelectedValuePath).GetValue(this.SelectedItem).ToString();

                MyPopup.IsOpen = false;
                if (MyDataGrid.Items.Count > 0)
                {
                    var border = VisualTreeHelper.GetChild(MyDataGrid, 0) as Decorator;
                    if (border != null)
                    {
                        var scroll = border.Child as ScrollViewer;
                        if (scroll != null) scroll.ScrollToTop();
                    }
                }

                if (SelectionChanged != null)
                {
                    SelectionChanged(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MyDataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                RecordSelected();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MyDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    RecordSelected();
                    txtSearch.Background = new SolidColorBrush(Colors.White);
                    e.Handled = true;
                }
                else if (!(e.Key == Key.Down || e.Key == Key.Up))
                {
                    MyDataGrid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    Bind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Temp()
        {
            try
            {
                IEnumerable enumerable = this.ItemsSource.AsQueryable().Where(this.SelectedValuePath + "==@0", this.SelectedValue);

                SelectedItem = enumerable.Cast<object>().ToList()[0];


                SetDisplayMember();

                txtSearch.Text = this.SelectedItem.GetType().GetProperty(ComboDisplayMembers.FirstOrDefault().FieldName).GetValue(this.SelectedItem).ToString();

                SelectedValue = this.SelectedItem.GetType().GetProperty(this.SelectedValuePath).GetValue(this.SelectedItem).ToString();

            }
            catch
            {
                txtSearch.Text = "";
                SelectedItem = null;

                SelectedValue = null;

            }

        }

        private void MyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                IEnumerable enumerable = MyDataGrid.SelectedItems;
                if (enumerable.Cast<object>().ToList().Count > 0)
                {
                    object Myobj = enumerable.Cast<object>().ToList()[0];

                    if (ComboDisplayMembers.Count >= 1)
                    {
                        lblItem1.Text = (ComboDisplayMembers.Where(o => o.Id == 1).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 1).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 2)
                    {
                        lblItem2.Text = (ComboDisplayMembers.Where(o => o.Id == 2).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 2).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 3)
                    {
                        lblItem3.Text = (ComboDisplayMembers.Where(o => o.Id == 3).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 3).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 4)
                    {
                        lblItem4.Text = (ComboDisplayMembers.Where(o => o.Id == 4).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 4).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 5)
                    {
                        lblItem5.Text = (ComboDisplayMembers.Where(o => o.Id == 5).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 5).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 6)
                    {
                        lblItem6.Text = (ComboDisplayMembers.Where(o => o.Id == 6).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 6).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 7)
                    {
                        lblItem7.Text = (ComboDisplayMembers.Where(o => o.Id == 7).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 7).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 8)
                    {
                        lblItem8.Text = (ComboDisplayMembers.Where(o => o.Id == 8).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 8).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 9)
                    {
                        lblItem9.Text = (ComboDisplayMembers.Where(o => o.Id == 9).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 9).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 10)
                    {
                        lblItem10.Text = (ComboDisplayMembers.Where(o => o.Id == 10).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 10).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 11)
                    {
                        lblItem11.Text = (ComboDisplayMembers.Where(o => o.Id == 11).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 11).FirstOrDefault().FieldName));
                    }
                    if (ComboDisplayMembers.Count >= 12)
                    {
                        lblItem12.Text = (ComboDisplayMembers.Where(o => o.Id == 12).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 12).FirstOrDefault().FieldName));
                    }
                    //if (ComboDisplayMembers.Count >= 10)
                    //{
                    //    lblItem10.Text = (ComboDisplayMembers.Where(o => o.Id == 10).FirstOrDefault().HeaderText + ": " + Dynamic.InvokeGetChain(Myobj, ComboDisplayMembers.Where(o => o.Id == 10).FirstOrDefault().FieldName));
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete)
                {
                    Delete_Press(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                e.Handled = true;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (CreateClick != null)
            {
                CreateClick(sender, e);
            }
        }

        private void Alter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RecordSelected();
                if (AlterClick != null)
                {
                    AlterClick(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Delete_Press(object sender, RoutedEventArgs e)
        {
            if (DeletePress != null)
            {
                DeletePress(sender, e);
            }
        }

        private void Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            Utility.GotFocus(sender,e);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Utility.LostFocus(sender, e);
        }
    }
    public class ComboDisplay
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string FieldName { get; set; }
        public int DisplayOnGrid { get; set; } = 1;
        public string HeaderText { get; set; }
        public string Alignment { get; set; }
        public double Width { get; set; }

    }
}