using AECSpot.Model;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace AECSpot
{
    /// <summary>
    /// 
    /// Interaction logic for CopyFilter.xaml
    /// </summary>
    public partial class CopyFilter : Window
    {
        public TransferFilterViewModel transferFilterViewModel;
        public string searchWord = string.Empty;
        public string searchWordForProjectViews = string.Empty;
        public string filtersSelected = "(0 items selected)";
        public CopyFilter(Document document, List<ParameterFilterElement> filters)
        {
            InitializeComponent();
            filtersSelectedTextBox.Text = "(0 items selected)";


            TreeViewElement.StaticPropertyChanged += OnStaticPropertyChanged;
            CategoriessSelectedTextBox.Text = "0";


            transferFilterViewModel = new TransferFilterViewModel();
            transferFilterViewModel.HostDocument = document;
            transferFilterViewModel.ViewFilters = filters;
            this.DataContext = transferFilterViewModel;
            //CategoriessSelectedTextBox.Text = $"({transferFilterViewModel.Counter} items selected)";
        }

        private void OnStaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CategoriessSelectedTextBox.Text = transferFilterViewModel?.Root?.GetSelected().Count.ToString();

            //CategoriessSelectedTextBox.Text = "changed";
        }

        private void CollapseAllButton_Click(object sender, RoutedEventArgs e)
        {
            tree.SetExpansion(isExpanded: false);
        }
        private void ExpandAllButton_Click(object sender, RoutedEventArgs e)
        {
            tree.SetExpansion(isExpanded: true);
        }
        private void CopyDataClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var document = transferFilterViewModel.HostDocument;
                var views = new List<View>();
                var view = transferFilterViewModel.HostDocument.ActiveView;
                List<ParameterFilterElement> selectedFilters = new List<ParameterFilterElement>();
                foreach (var item in transferFilterViewModel.ViewFilters)
                {
                    var x = ViewFiltersList.SelectedItems;
                    foreach (var item1 in x)
                    {
                        if (item1 is ParameterFilterElement element)
                        {
                            if (element.Name == item.Name)
                            {
                                selectedFilters.Add(item);
                            }
                        }
                    }
                }
                //.ViewFilters.W;
                var selectedViews = transferFilterViewModel?.Root?.GetSelected();
                if (view == null || selectedFilters.Count == 0 || transferFilterViewModel.ViewFilters == null || selectedViews == null) return;

                using (Transaction t = new Transaction(document, "Override Filter Graphics"))
                {
                    t.Start();
                    foreach (var selectedFilter in selectedFilters)
                    {

                        var overrideSettings = view.GetFilterOverrides(selectedFilter.Id);
                        selectedViews.ForEach(v => views.Add((View)document.GetElement(v)));
                        AssignFilter(view, views, selectedFilter, overrideSettings);
                    }
                    t.Commit();
                }
                this.Close();
                // System.Windows.MessageBox.Show("Filter Copied Successfully.");

            }
            catch { }
        }
        private static void AssignFilter(View activeView, List<View> views, ParameterFilterElement filter, OverrideGraphicSettings overrideGraphic)
        {

            if (filter == null) return;
            foreach (var view in views)
            {
                try
                {
                  bool visi= activeView.GetFilterVisibility(filter.Id);
                    view.SetFilterVisibility(filter.Id, visi);
                }
                catch (Exception)
                { 
                
                }
                view.SetFilterOverrides(filter.Id, overrideGraphic);
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.ViewFiltersList.SelectAll();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ViewFiltersList.UnselectAll();

        }

        private void ViewFiltersList_Selected(object sender, RoutedEventArgs e)
        {
            this.CheckBoxAllFilter.IsChecked = null;

        }

        private void textValue_Checked(object sender, RoutedEventArgs e)
        {
            var x = ((CheckBox)e.OriginalSource).Parent;
            ////var y = (ListBoxItem)sender;
            if (ViewFiltersList.SelectedItems.Count == ViewFiltersList.Items.Count)
            {
                this.CheckBoxAllFilter.IsChecked = true;

            }
            else
            {
                this.CheckBoxAllFilter.IsChecked = null;

            }

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
                            ViewFiltersList.ItemsSource = transferFilterViewModel.ViewFilters.OrderBy(s => s.Name).Where(s => s.Name.Contains(searchWord, StringComparison.OrdinalIgnoreCase)).ToList();

                            break;

                        case "Default":
                            ViewFiltersList.ItemsSource = transferFilterViewModel.ViewFilters.Where(s => s.Name.Contains(searchWord, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                    }
                }
            }
            //x[0].
        }

        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchWord = ((WatermarkTextBox)sender).Text;

            ViewFiltersList.ItemsSource = transferFilterViewModel.ViewFilters.Where(s => s.Name.Contains(searchWord, StringComparison.OrdinalIgnoreCase));
        }

        private void WatermarkTextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            searchWordForProjectViews = ((WatermarkTextBox)sender).Text;

            //var x =transferFilterViewModel.Root.Children.Select(s=>s.Children).ToList().Where(s=>s.All(n=>n.Name.Contains(searchWordForProjectViews,StringComparison.OrdinalIgnoreCase)));

            var selectedViews = transferFilterViewModel?.Root?.GetSelected();

            //var treeChildernItems = GetFamiliesData(transferFilterViewModel.HostDocument, searchWordForProjectViews).Children;
            //transferFilterViewModel.Root = GetFamiliesData(transferFilterViewModel.HostDocument, searchWordForProjectViews);
            //transferFilterViewModel.Root.Children.Select(s=>s.Children.Where(n=>n.Name.Contains(searchWordForProjectViews,StringComparison.OrdinalIgnoreCase))).ToList();
            foreach (var item in transferFilterViewModel.Root.Children)
            {
                foreach (var item1 in item.Children)
                {
                    item1.IsVisible = true;
                    if (selectedViews.Any(s => s.Value == item1.Id.Value))
                    {
                        item1.IsChecked = true;
                    }
                }


            }
          var x=  transferFilterViewModel.Root.Children.Select(s => s.Children.Where(n => !n.Name.Contains(searchWordForProjectViews, StringComparison.OrdinalIgnoreCase)));
            foreach (var item in x)
            {
                foreach (var item1 in item)
                {
                    //item1.Visibility = System.Windows.Visibility.Collapsed;
                    item1.IsVisible = false;
                }
            }
          

            tree.SetExpansion(true);

            if (searchWordForProjectViews == string.Empty) tree.ItemsSource = transferFilterViewModel.Root.Children;
        }
        private TreeViewElement GetFamiliesData(Document doc, string searchedWord)
        {
            var root = new TreeViewElement() { Children = new List<TreeViewElement>(), Name = "Views" };

            var categories = new List<Category>() { Category.GetCategory(doc, BuiltInCategory.OST_Views) };
            var views = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).ToList();

            var rootSections = new Dictionary<string, TreeViewElement>();
            var viewsTypes = views.Select(v => ((View)v).ViewType.ToString()).Distinct();
            foreach (var category in categories)
            {
                foreach (var view in viewsTypes)
                {
                    TreeViewElement categorySection = null;
                    if (view == "ThreeD")
                    {
                        TransferFilterViewModel.CreateRootSectionIfNotExist(root, rootSections, "3D");
                        categorySection = rootSections["3D"];
                    }
                    else
                    {
                        TransferFilterViewModel.CreateRootSectionIfNotExist(root, rootSections, view);
                        categorySection = rootSections[view];
                    }
                    List<TreeViewElement> CategoryElementsdata = TransferFilterViewModel.ConvertElemntsToTreeElements(views.Where(s => s.Name.Contains(searchedWord, StringComparison.OrdinalIgnoreCase)).Where(v => ((View)v).ViewType.ToString() == view).ToList(), categorySection);
                    categorySection.Children.AddRange(CategoryElementsdata);
                }
            }
            foreach (var item in rootSections) root.Children.Add(item.Value);
            return root;
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

        private void watermarkTextBox_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void watermarkTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

            //(sender as WatermarkTextBox).BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#BC7B18"));
        }

        private void CheckBoxAllCategories_Checked(object sender, RoutedEventArgs e)
        {
            if (tree.HasItems)
            {
                var treeItems = tree.ItemsSource;
                foreach (TreeViewElement item in treeItems)
                {
                    item.IsChecked = true;
                }

            }
        }

        private void CheckBoxAllCategories_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tree.HasItems)
            {
                var treeItems = tree.ItemsSource;
                foreach (TreeViewElement item in treeItems)
                {
                    item.IsChecked = false;
                }

            }
        }

        //private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    int count = 0;
        //    var items =(sender as TreeView).ItemsSource;
        //    foreach (TreeViewElement item in items) 
        //    {
        //        if (item.IsChecked is true)
        //        {
        //            count++;
        //        }
        //    }
        //  //  CategoriessSelectedTextBox.Text = $"({count} items selected)";

        //}

        //private void tree_GotTouchCapture(object sender, TouchEventArgs e)
        //{

        //}
        //private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{

        //}

        //private void tree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{

        //}
        private void treeviewBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the clicked border
            Border border = sender as Border;

            // Find the CheckBox inside the Border
            if (border != null)
            {
                CheckBox checkBox = border.Child as CheckBox;

                if (checkBox != null)
                {
                    // Toggle the CheckBox's IsChecked property
                    checkBox.IsChecked = !(checkBox.IsChecked ?? false);
                }
            }
        }

        //private void tree_Selected(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
