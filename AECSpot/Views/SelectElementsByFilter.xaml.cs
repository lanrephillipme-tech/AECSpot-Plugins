using AECSpot.Model;
using Autodesk.Revit.DB;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace AECSpot
{
    /// <summary>
    /// Interaction logic for CopyFilter.xaml
    /// </summary>
    public partial class SelectElementsByFilter : Window
    {
        public string searchWord = string.Empty;
      
        public List<ParameterFilterElement> DocumentsFilter { get; set; }
        public ParameterFilterElement SelectedFilter { get; set; }
        public List<ParameterFilterElement> SelectedFilters { get; set; } = new List<ParameterFilterElement>();

        public SelectElementsByFilter(List<ParameterFilterElement> documentsFilter)
        {
            InitializeComponent();
            filtersSelectedTextBox.Text = "(0 items selected)";

            this.DataContext = this;
            DocumentsFilter = documentsFilter;
        }

        private void SelectByFilterButtonClicked(object sender, RoutedEventArgs e)
        {
            foreach (var item in DocumentsFilter)
            {
                var x = FiltersList.SelectedItems;
                foreach (var item1 in x)
                {
                    if (item1 is ParameterFilterElement element)
                    {
                        if (element.Name == item.Name)
                        {
                            SelectedFilters.Add(item);
                        }
                    }
                }
            }
            DialogResult = true;
            this.Close();
        }

        private void IsAllViews_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsCurrentView == null) return;
            IsCurrentView.IsChecked = true;
        }

        private void IsAllViews_Checked(object sender, RoutedEventArgs e)
        {
            if (IsCurrentView == null) return;
            IsCurrentView.IsChecked = false;
        }

        internal void IsCurrentView_Checked(object sender, RoutedEventArgs e)
        {
            if (IsAllViews == null) return;
            IsAllViews.IsChecked = false;
        }

        private void IsCurrentView_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsAllViews == null) return;
            IsAllViews.IsChecked = true;
        }
        private void textValue_Checked(object sender, RoutedEventArgs e)
        {
            var x = ((CheckBox)e.OriginalSource).Parent;
            ////var y = (ListBoxItem)sender;
            if (FiltersList.SelectedItems.Count == FiltersList.Items.Count)
            {
                this.CheckBoxAllFilter.IsChecked = true;

            }
            else
            {
                this.CheckBoxAllFilter.IsChecked = null;

            }

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
                this.WindowState = System.Windows.WindowState.Maximized;
            else
                this.WindowState = System.Windows.WindowState.Normal;
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void ViewFiltersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListBox).Items.Refresh();
            filtersSelectedTextBox.Text = $"({(sender as ListBox).SelectedItems.Count} items selected)";

        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var x = e.AddedItems;
            if (x != null)
            {
                var selectedValue = ((ComboBoxItem)x[0]).Content;
                if (selectedValue != null)
                {
                    switch (selectedValue)
                    {
                        case "A-Z":
                            //ViewFiltersList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(string.Empty, System.ComponentModel.ListSortDirection.Ascending));
                            FiltersList.ItemsSource = DocumentsFilter?.OrderBy(s => s.Name).Where(s => s.Name.Contains(searchWord, StringComparison.OrdinalIgnoreCase)).ToList();

                            break;

                        case "Default":
                            FiltersList.ItemsSource = DocumentsFilter?.Where(s => s.Name.Contains(searchWord, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                    }
                }
            }
            //x[0].
        }
        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchWord = ((WatermarkTextBox)sender).Text;

            FiltersList.ItemsSource = DocumentsFilter?.Where(s => s.Name.Contains(searchWord, StringComparison.OrdinalIgnoreCase));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.FiltersList.SelectAll();
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.FiltersList.UnselectAll();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.aecspot.com");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.aecspot.com/models");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.aecspot.com/tools");
        }
    }
}
