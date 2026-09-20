using AECSpot.FilterCatalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AECSpot
{
    public class ReplaceFiltersViewModel : ViewModelBase
    {
        private readonly FilterReplacementPlanner _planner = new FilterReplacementPlanner();
        private FilterCatalogItem _sourceFilter;
        private FilterCatalogItem _replacementFilter;
        private FilterReplacementPlan _approvedPlan;
        private string _status = "Select a source filter and a replacement filter, then preview the change.";

        public ReplaceFiltersViewModel(IEnumerable<FilterCatalogItem> catalog)
        {
            Filters = new ObservableCollection<FilterCatalogItem>((catalog ?? Enumerable.Empty<FilterCatalogItem>()).OrderBy(filter => filter.Name, StringComparer.OrdinalIgnoreCase));
        }

        public ObservableCollection<FilterCatalogItem> Filters { get; }
        public bool HasFilters => Filters.Count > 0;
        public FilterReplacementPlan ApprovedPlan => _approvedPlan;
        public bool CanApply => _approvedPlan != null && _approvedPlan.IsValid;

        public FilterCatalogItem SourceFilter { get => _sourceFilter; set { _sourceFilter = value; InvalidatePreview(); OnPropertyChanged(nameof(SourceFilter)); } }
        public FilterCatalogItem ReplacementFilter { get => _replacementFilter; set { _replacementFilter = value; InvalidatePreview(); OnPropertyChanged(nameof(ReplacementFilter)); } }
        public string Status { get => _status; private set { _status = value; OnPropertyChanged(nameof(Status)); } }
        public int PlannedAssignmentCount => _approvedPlan == null ? 0 : _approvedPlan.Usages.Count;
        public int ExistingReplacementCount => _approvedPlan == null ? 0 : _approvedPlan.ExistingReplacementAssignments;

        public void BuildPreview()
        {
            _approvedPlan = _planner.CreatePlan(SourceFilter, ReplacementFilter);
            if (SourceFilter == null || ReplacementFilter == null)
            {
                Status = "Select both a source filter and a replacement filter.";
            }
            else if (SourceFilter.FilterId.Value == ReplacementFilter.FilterId.Value)
            {
                Status = "The replacement filter must be different from the source filter.";
            }
            else if (_approvedPlan.Usages.Count == 0)
            {
                Status = "The source filter is not applied to any project view or view template.";
            }
            else
            {
                Status = string.Format("{0} assignment{1} will be replaced. {2} already use the replacement; source state will replace their existing state.", _approvedPlan.Usages.Count, _approvedPlan.Usages.Count == 1 ? string.Empty : "s", _approvedPlan.ExistingReplacementAssignments);
            }

            OnPropertyChanged(nameof(ApprovedPlan));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(PlannedAssignmentCount));
            OnPropertyChanged(nameof(ExistingReplacementCount));
        }

        public bool ConfirmApply()
        {
            if (!CanApply)
            {
                Status = "Create a valid replacement preview before applying changes.";
                return false;
            }

            return true;
        }

        private void InvalidatePreview()
        {
            _approvedPlan = null;
            Status = "Selection changed. Create a new preview before applying changes.";
            OnPropertyChanged(nameof(ApprovedPlan));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(PlannedAssignmentCount));
            OnPropertyChanged(nameof(ExistingReplacementCount));
        }
    }
}
