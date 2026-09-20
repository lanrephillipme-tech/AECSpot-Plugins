using AECSpot.FilterCatalog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AECSpot
{
    public partial class ReplaceFiltersWindow : Window
    {
        public ReplaceFiltersWindow(IEnumerable<FilterCatalogItem> catalog)
        {
            InitializeComponent();
            DataContext = new ReplaceFiltersViewModel(catalog);
        }

        public FilterReplacementPlan ApprovedPlan => ((ReplaceFiltersViewModel)DataContext).ApprovedPlan;

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            ((ReplaceFiltersViewModel)DataContext).BuildPreview();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (((ReplaceFiltersViewModel)DataContext).ConfirmApply())
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
