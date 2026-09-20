using AECSpot.FilterCatalog;
using System.Collections.ObjectModel;
using System.Linq;

namespace AECSpot
{
    public class DuplicateFiltersViewModel : ViewModelBase
    {
        private DuplicateFilterGroup _selectedGroup;

        public DuplicateFiltersViewModel(DuplicateFilterAnalysis analysis, bool hasFilters)
        {
            analysis = analysis ?? new DuplicateFilterAnalysis();
            DuplicateGroups = new ObservableCollection<DuplicateFilterGroup>(analysis.DuplicateGroups);
            IndeterminateFilters = new ObservableCollection<FilterNormalizationResult>(analysis.IndeterminateFilters);
            HasFilters = hasFilters;
            SelectedGroup = DuplicateGroups.FirstOrDefault();
        }

        public ObservableCollection<DuplicateFilterGroup> DuplicateGroups { get; }
        public ObservableCollection<FilterNormalizationResult> IndeterminateFilters { get; }
        public bool HasFilters { get; }
        public bool HasDuplicateGroups => DuplicateGroups.Count > 0;
        public bool HasIndeterminateFilters => IndeterminateFilters.Count > 0;

        public DuplicateFilterGroup SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup == value)
                {
                    return;
                }

                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }
    }
}
