using AECSpot.FilterCatalog;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AECSpot
{
    public partial class FilterTargetsWindow : Window
    {
        public FilterTargetsWindow(Document document, IEnumerable<FilterCatalogItem> catalog)
        {
            InitializeComponent();
            DataContext = new FilterTargetsViewModel(document, catalog, new FilterTargetQueryService());
        }

        private void FindMatchingElements_Click(object sender, RoutedEventArgs e)
        {
            ((FilterTargetsViewModel)DataContext).LoadMatchingElements();
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
