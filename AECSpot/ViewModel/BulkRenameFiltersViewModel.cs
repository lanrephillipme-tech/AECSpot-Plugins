using AECSpot.FilterCatalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AECSpot
{
    public class BulkRenameFiltersViewModel : ViewModelBase
    {
        private readonly List<FilterCatalogItem> _catalog;
        private readonly FilterRenamePlanner _planner = new FilterRenamePlanner();
        private string _findText = string.Empty;
        private string _replaceText = string.Empty;
        private string _prefix = string.Empty;
        private string _suffix = string.Empty;
        private string _status = "Select filters and create a preview before applying changes.";

        public BulkRenameFiltersViewModel(IEnumerable<FilterCatalogItem> catalog)
        {
            _catalog = (catalog ?? Enumerable.Empty<FilterCatalogItem>()).ToList();
            Filters = new ObservableCollection<BulkRenameFilterItem>(_catalog.Select(filter => new BulkRenameFilterItem(filter)));
            PreviewItems = new ObservableCollection<FilterRenamePlanItem>(); foreach (var item in Filters) { item.PropertyChanged += (sender, args) => { if (args.PropertyName == nameof(BulkRenameFilterItem.IsSelected)) InvalidatePreview(); }; }
        }

        public ObservableCollection<BulkRenameFilterItem> Filters { get; }
        public ObservableCollection<FilterRenamePlanItem> PreviewItems { get; }
        public FilterRenamePlan ApprovedPlan { get; private set; }
        public bool HasFilters => Filters.Count > 0;

        public string FindText { get => _findText; set { _findText = value ?? string.Empty; OnPropertyChanged(nameof(FindText)); InvalidatePreview(); } }
        public string ReplaceText { get => _replaceText; set { _replaceText = value ?? string.Empty; OnPropertyChanged(nameof(ReplaceText)); InvalidatePreview(); } }
        public string Prefix { get => _prefix; set { _prefix = value ?? string.Empty; OnPropertyChanged(nameof(Prefix)); InvalidatePreview(); } }
        public string Suffix { get => _suffix; set { _suffix = value ?? string.Empty; OnPropertyChanged(nameof(Suffix)); InvalidatePreview(); } }
        public string Status { get => _status; private set { _status = value; OnPropertyChanged(nameof(Status)); } }
        public bool CanApply => ApprovedPlan != null && ApprovedPlan.IsValid;

        private void InvalidatePreview()
        {
            ApprovedPlan = null;
            PreviewItems.Clear();
            Status = "Inputs changed. Create a new preview before applying changes.";
            OnPropertyChanged(nameof(CanApply));
        }

        public void BuildPreview()
        {
            ApprovedPlan = null;
            var request = new FilterRenameRequest { FindText = FindText, ReplaceText = ReplaceText, Prefix = Prefix, Suffix = Suffix };
            var plan = _planner.CreatePlan(_catalog, Filters.Where(item => item.IsSelected).Select(item => item.Filter), request);
            PreviewItems.Clear();
            foreach (var item in plan.Items)
            {
                PreviewItems.Add(item);
            }

            Status = plan.IsValid ? string.Format("{0} filter name{1} ready to apply.", plan.Items.Count(item => item.IsChange), plan.Items.Count(item => item.IsChange) == 1 ? string.Empty : "s") : GetPlanError(plan);
            if (plan.IsValid)
            {
                ApprovedPlan = plan;
            }

            OnPropertyChanged(nameof(CanApply));
        }

        public bool ConfirmApply()
        {
            if (!CanApply)
            {
                Status = "Create a valid preview before applying changes.";
                return false;
            }

            return true;
        }

        private static string GetPlanError(FilterRenamePlan plan)
        {
            var error = plan.Errors.FirstOrDefault() ?? plan.Items.Select(item => item.ValidationMessage).FirstOrDefault(message => !string.IsNullOrEmpty(message));
            return error ?? "The rename preview is not valid.";
        }
    }

    public class BulkRenameFilterItem : ViewModelBase
    {
        private bool _isSelected;

        public BulkRenameFilterItem(FilterCatalogItem filter)
        {
            Filter = filter;
        }

        public FilterCatalogItem Filter { get; }
        public string Name => Filter.Name;
        public string UsageSummary => Filter.UsageSummary;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }
    }
}

