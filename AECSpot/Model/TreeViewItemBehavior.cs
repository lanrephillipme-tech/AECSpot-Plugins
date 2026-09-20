using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace AECSpot
{
    public static class TreeViewItemBehavior
    {
        public static bool GetIsSelectedOnCheck(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedOnCheckProperty);
        }

        public static void SetIsSelectedOnCheck(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedOnCheckProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSelectedOnCheck.  
        public static readonly DependencyProperty IsSelectedOnCheckProperty =
            DependencyProperty.RegisterAttached("IsSelectedOnCheck", typeof(bool), typeof(TreeViewItemBehavior), new PropertyMetadata(false, OnIsSelectedOnCheckChanged));

        private static void OnIsSelectedOnCheckChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CheckBox checkBox && checkBox.DataContext is TreeViewItem treeViewItem)
            {
                // Subscribe to CheckBox's checked event
                checkBox.Checked += (sender, args) =>
                {
                    treeViewItem.IsSelected = true;
                };

                // Subscribe to CheckBox's unchecked event if you want to unselect the item when unchecked
                checkBox.Unchecked += (sender, args) =>
                {
                    treeViewItem.IsSelected = false;
                };
            }
        }
    }
}
