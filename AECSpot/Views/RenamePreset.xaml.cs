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
    /// Interaction logic for RenamePreset.xaml
    /// </summary>
    public partial class RenamePreset : Window
    {
        public string PresetName { get; private set; } = string.Empty;
        private List<string> presets = new List<string>();

        public RenamePreset(string oldName, List<string> setsNames)
        {
            InitializeComponent();
            
            PresetNameTextBox.Text = oldName;
            PresetName = oldName;
            PresetNameTextBox.Focus();

            presets.Add(PresetName);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PresetNameTextBox.Text))
            {
                MessageBox.Show("Please enter a preset name.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (presets.Any(s => s.ToLower() == PresetNameTextBox.Text.ToLower()))
            {
                MessageBox.Show("Name already exists , Please enter different preset name.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PresetName = PresetNameTextBox.Text.Trim();
            DialogResult = true;
            this.Close();
        }
    }
}
