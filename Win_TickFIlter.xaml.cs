using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using FinishGoodStock.Models;

namespace FinishGoodStock
{
    /// <summary>
    /// Interaction logic for Win_TickFIlter.xaml
    /// </summary>
    public partial class Win_TickFIlter : Window
    {
        public string SelectedGroup;
        public string SelectedGroupName;

        public ObservableCollection<GodownLocation> VoucherGroups { get; set; }
        private Dictionary<string, bool> isSelectedDict;

        public Win_TickFIlter()
        {
            InitializeComponent();

            // Get the list of VoucherGroups from the API
            var lst = GodownLocationApi.GetGodownLocation();  // No need for dictionary if method doesn't require it
            VoucherGroups = new ObservableCollection<GodownLocation>(lst);

            // Initialize dictionary to track the selection status of each VoucherGroup
            isSelectedDict = new Dictionary<string, bool>();
            foreach (var voucherGroup in VoucherGroups)
            {
                isSelectedDict[voucherGroup.Id.ToString()] = false; // Initialize with unchecked status
            }

            // Bind the ListBox to the collection
            ItemsListBox.ItemsSource = VoucherGroups;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Get the clicked CheckBox and its associated VoucherGroup
            var checkBox = sender as CheckBox;
            var godownLocation = checkBox.DataContext as GodownLocation;  // Assuming you want to use GodownLocation

            // Update the dictionary with the selection status
            if (godownLocation != null)
            {
                isSelectedDict[godownLocation.Id.ToString()] = checkBox.IsChecked ?? false; // Convert to string if needed
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Filter selected items based on the dictionary values
            var selectedIds = isSelectedDict
                .Where(kv => kv.Value) // Select only checked items
                .Select(kv => kv.Key)  // Get the ID of the selected items
                .ToList();

            // Display or return selected IDs
            SelectedGroup = string.Join(", ", selectedIds.Select(id =>id));

            // Close the window
            this.DialogResult = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }


}
