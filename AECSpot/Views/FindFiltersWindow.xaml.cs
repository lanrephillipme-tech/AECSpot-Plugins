using AECSpot.FilterCatalog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AECSpot
{
    public partial class FindFiltersWindow : Window
    {
        public FindFiltersWindow(IEnumerable<FilterCatalogItem> catalog)
        {
            InitializeComponent();
            DataContext = new FindFiltersViewModel(catalog);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
