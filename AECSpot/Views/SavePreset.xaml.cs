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
    /// Interaction logic for SavePreset.xaml
    /// </summary>
    public partial class SavePreset : Window
    {
        public string PresetName { get; private set; } = string.Empty;
        public bool IsConfirmed { get; private set; } = false;

        private List<string> presets = new List<string>();

        public SavePreset(List<string> setsNames)
        {
            InitializeComponent();
            PresetNameTextBox.Focus();
            presets.AddRange(setsNames);

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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PresetNameTextBox.Text))
            {
                MessageBox.Show("Please enter a preset name.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if ( presets.Any(s=>s.ToLower() ==  PresetNameTextBox.Text.ToLower()))
            {
                MessageBox.Show("Name already exists , Please enter different preset name.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PresetName = PresetNameTextBox.Text.Trim();
            IsConfirmed = true;
            this.Close();
        }
    }
}

