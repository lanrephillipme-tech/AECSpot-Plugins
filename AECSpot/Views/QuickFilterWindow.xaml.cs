using AECSpot.QuickFilter;
using Autodesk.Revit.DB;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AECSpot
{
    public partial class QuickFilterWindow : Window
    {
        public QuickFilterWindow(Document document) { InitializeComponent(); DataContext = new QuickFilterViewModel(document); }
        public QuickFilterPlan ApprovedPlan => ((QuickFilterViewModel)DataContext).ApprovedPlan;
        public QuickFilterAction ApprovedAction => ((QuickFilterViewModel)DataContext).ApprovedAction;
        public string SaveName => ((QuickFilterViewModel)DataContext).SaveName;
        private void LoadParameters_Click(object sender, RoutedEventArgs e) { ((QuickFilterViewModel)DataContext).LoadParameters(); }
        private void AddRule_Click(object sender, RoutedEventArgs e) { ((QuickFilterViewModel)DataContext).AddRule(); }
        private void RemoveRule_Click(object sender, RoutedEventArgs e) { ((QuickFilterViewModel)DataContext).RemoveRule((sender as Button)?.Tag as QuickFilterRuleDraft); }
        private void SelectCurrentView_Click(object sender, RoutedEventArgs e) { Approve(QuickFilterAction.SelectCurrentView); }
        private void SelectEntireProject_Click(object sender, RoutedEventArgs e) { Approve(QuickFilterAction.SelectEntireProject); }
        private void TemporaryOverride_Click(object sender, RoutedEventArgs e) { Approve(QuickFilterAction.TemporaryOverride); }
        private void TemporaryIsolate_Click(object sender, RoutedEventArgs e) { Approve(QuickFilterAction.TemporaryIsolate); }
        private void ResetOverrides_Click(object sender, RoutedEventArgs e) { ((QuickFilterViewModel)DataContext).ApproveReset(); DialogResult = true; Close(); }
        private void SaveToProject_Click(object sender, RoutedEventArgs e) { Approve(QuickFilterAction.SaveToProject); }
        private void Approve(QuickFilterAction action) { if (((QuickFilterViewModel)DataContext).Approve(action)) { DialogResult = true; Close(); } }
        private void CloseButton_Click(object sender, RoutedEventArgs e) { Close(); }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { if (e.ChangedButton == MouseButton.Left) DragMove(); }
    }
}
