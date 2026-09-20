using AECSpot.FilterCatalog;
using System.Windows;
using System.Windows.Input;

namespace AECSpot
{
    public partial class DuplicateFiltersWindow : Window
    {
        public DuplicateFiltersWindow(DuplicateFilterAnalysis analysis, bool hasFilters)
        {
            InitializeComponent();
            DataContext = new DuplicateFiltersViewModel(analysis, hasFilters);
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
