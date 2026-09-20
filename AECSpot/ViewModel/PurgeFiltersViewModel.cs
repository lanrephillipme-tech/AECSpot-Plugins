using AECSpot.FilterCatalog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AECSpot
{
    public class PurgeFiltersViewModel : ViewModelBase
    {
        private readonly FilterPurgePlanner _planner = new FilterPurgePlanner();
        private FilterPurgePlan _approvedPlan;
        private string _status;
        private bool _confirmed;

        public PurgeFiltersViewModel(IEnumerable<FilterCatalogItem> catalog)
        {
            Candidates = new ObservableCollection<PurgeFilterItem>(_planner.GetUnusedFilters(catalog).Select(filter => new PurgeFilterItem(filter)));
            PreviewItems = new ObservableCollection<FilterCatalogItem>();
            Status = Candidates.Count == 0 ? "No unused parameter filters were found." : "Select unused filters and create a deletion preview.";
        }

        public ObservableCollection<PurgeFilterItem> Candidates { get; }
        public ObservableCollection<FilterCatalogItem> PreviewItems { get; }
        public bool HasCandidates => Candidates.Count > 0;
        public FilterPurgePlan ApprovedPlan => _approvedPlan;
        public string Status { get => _status; private set { _status = value; OnPropertyChanged(nameof(Status)); } }
        public bool Confirmed { get => _confirmed; set { _confirmed = value; OnPropertyChanged(nameof(Confirmed)); } }

        public void BuildPreview()
        {
            _approvedPlan = _planner.CreatePlan(Candidates.Where(item => item.IsSelected).Select(item => item.Filter));
            PreviewItems.Clear();
            foreach (var filter in _approvedPlan.Filters)
            {
                PreviewItems.Add(filter);
            }

            Status = _approvedPlan.IsValid
                ? string.Format("{0} unused filter{1} will be deleted after confirmation.", PreviewItems.Count, PreviewItems.Count == 1 ? string.Empty : "s")
                : "Select at least one unused filter before previewing deletion.";
            Confirmed = false;
        }

        public bool ConfirmApply()
        {
            if (_approvedPlan == null || !_approvedPlan.IsValid)
            {
                Status = "Create a deletion preview before purging filters.";
                return false;
            }

            if (!Confirmed)
            {
                Status = "Confirm that the selected filters will be permanently deleted.";
                return false;
            }

            return true;
        }
    }

    public class PurgeFilterItem : ViewModelBase
    {
        private bool _isSelected;

        public PurgeFilterItem(FilterCatalogItem filter)
        {
            Filter = filter;
        }

        public FilterCatalogItem Filter { get; }
        public string Name => Filter.Name;
        public string CategoriesSummary => Filter.CategoriesSummary;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }
    }
}
