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
using System.Windows.Shapes;

namespace AECSpot.Views
{
    /// <summary>
    /// Interaction logic for DeletePreset.xaml
    /// </summary>
    public partial class DeletePreset : Window
    {
        public bool IsConfirmed { get; private set; } = false;

        public DeletePreset()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }
    }
}
