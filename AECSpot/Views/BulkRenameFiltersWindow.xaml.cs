using AECSpot.FilterCatalog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AECSpot
{
    public partial class BulkRenameFiltersWindow : Window
    {
        public BulkRenameFiltersWindow(IEnumerable<FilterCatalogItem> catalog)
        {
            InitializeComponent();
            DataContext = new BulkRenameFiltersViewModel(catalog);
        }

        public FilterRenamePlan ApprovedPlan => ((BulkRenameFiltersViewModel)DataContext).ApprovedPlan;

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            ((BulkRenameFiltersViewModel)DataContext).BuildPreview();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (((BulkRenameFiltersViewModel)DataContext).ConfirmApply())
            {
                DialogResult = true;
                Close();
            }
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
