using AECSpot.FilterCatalog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AECSpot
{
    public partial class PurgeFiltersWindow : Window
    {
        public PurgeFiltersWindow(IEnumerable<FilterCatalogItem> catalog)
        {
            InitializeComponent();
            DataContext = new PurgeFiltersViewModel(catalog);
        }

        public FilterPurgePlan ApprovedPlan => ((PurgeFiltersViewModel)DataContext).ApprovedPlan;

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            ((PurgeFiltersViewModel)DataContext).BuildPreview();
        }

        private void Purge_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (PurgeFiltersViewModel)DataContext;
            if (!viewModel.ConfirmApply())
            {
                return;
            }

            var confirmation = MessageBox.Show("Delete the filters shown in the preview? This action modifies the project.", "Purge Unused Filters", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirmation == MessageBoxResult.Yes)
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
