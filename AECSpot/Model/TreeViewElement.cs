using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AECSpot
{
    public class TreeViewElement : ViewModelBase
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void OnStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }


        public static string _counter = "0";

        public static string Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                OnStaticPropertyChanged(nameof(Counter));
            }
        }
        #region Properties
        public string Name { get; set; }
        public ElementId Id { get; set; }
        public string CategoryName { get; set; }
        public ElementId CategoryId { get; set; }
        #endregion

        #region Roots
        public List<TreeViewElement> Children { get; set; }
        public TreeViewElement Parent { get; set; }
        private bool? _IsChecked { get; set; } = false;
        public bool? IsChecked
        {
            get => _IsChecked;
            set
            {
                SetIsChecked(value, true, true);
            }
        }

        public bool HasChildren
        {
            get
            {
                if (Children != null && Children?.Count > 0) return true;
                else return false;
            }
        }
        private System.Windows.Visibility _visibility = System.Windows.Visibility.Visible;
        public System.Windows.Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }
        public bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        } 



        #region Methods
        public void AddChild(TreeViewElement child)
        {
            if (Children == null) Children = new List<TreeViewElement>();
            Children.Add(child);
        }
        public List<ElementId> GetSelected()
        {
            var selectedList = new List<ElementId>();

            if (!HasChildren && IsChecked != true) return selectedList;

            else if (IsChecked == true && Id != null)
            {
                selectedList.Add(Id);
                return selectedList;
            }
            foreach (var child in Children)
            {
                if (child.HasChildren)
                {
                    selectedList.AddRange(child.GetSelected());
                }
                else if (child.IsChecked == true && child.Id != null)
                {
                    selectedList.Add((ElementId)child.Id);
                }
            }
            return selectedList;
        }
        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            //if(value == true && !IsChecked)
            //{

            //    int count = int.Parse(Counter);
            //    count += 1;
            //    Counter = count.ToString();
            //}
            //else if (value is false && IsChecked)
            //{

            //    int count = int.Parse(Counter);
            //    count -= 1;
            //    Counter = count.ToString();
            //}
            // Leaf nodes should never be in indeterminate state
            if (!HasChildren && value == null)
                value = false;

            // Parent going from true -> skip null, go straight to false
            if (updateChildren && HasChildren && value == null && _IsChecked == true)
                value = false;

            if (value == _IsChecked)
            {
                //    int count2 = int.Parse(Counter);
                //    count2 += GetSelected().Count;

                return;
            }
            _IsChecked = value;
            Counter = "0";
            if (updateChildren && _IsChecked.HasValue && this.Children != null)
                this.Children.ForEach(c => (c).SetIsChecked(_IsChecked, true, false));
            if (updateParent && this.Parent != null)
                (Parent).VerifyCheckState();
            this.OnPropertyChanged(nameof(IsChecked));
        }
        public void VerifyCheckState()
        {
            bool? state = null;
            if (Children != null)
            {
                for (int i = 0; i < this.Children.Count; ++i)
                {
                    bool? current = ((Children)[i]).IsChecked;
                    if (i == 0)
                    {
                        state = current;
                    }
                    else if (state != current)
                    {
                        state = null;
                        break;
                    }
                }
            }
            else
            {
                state = _IsChecked;
            }
            this.SetIsChecked(state, false, true);
        }
        #endregion
    }
}
#endregion