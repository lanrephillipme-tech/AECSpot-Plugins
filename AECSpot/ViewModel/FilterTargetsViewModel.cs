using AECSpot.FilterCatalog;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AECSpot
{
    public class FilterTargetsViewModel : ViewModelBase
    {
        private readonly Document _document;
        private readonly FilterTargetQueryService _queryService;
        private readonly List<FilterCatalogItem> _catalog;
        private FilterCatalogItem _selectedFilter;
        private string _searchText = string.Empty;
        private string _queryStatus = "Select a filter, then resolve matching elements on demand.";

        public FilterTargetsViewModel(Document document, IEnumerable<FilterCatalogItem> catalog, FilterTargetQueryService queryService)
        {
            _document = document;
            _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
            _catalog = (catalog ?? Enumerable.Empty<FilterCatalogItem>()).ToList();
            FilterItems = new ObservableCollection<FilterCatalogItem>();
            MatchingElements = new ObservableCollection<FilterTargetElementInfo>();
            RefreshFilterItems();
            SelectedFilter = FilterItems.FirstOrDefault();
        }

        public ObservableCollection<FilterCatalogItem> FilterItems { get; }
        public ObservableCollection<FilterTargetElementInfo> MatchingElements { get; }
        public bool HasFilters => _catalog.Count > 0;
        public bool HasSearchResults => FilterItems.Count > 0;
        public bool HasMatchingElements => MatchingElements.Count > 0;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (string.Equals(_searchText, value, StringComparison.Ordinal))
                {
                    return;
                }

                _searchText = value ?? string.Empty;
                OnPropertyChanged(nameof(SearchText));
                RefreshFilterItems();
            }
        }

        public FilterCatalogItem SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter == value)
                {
                    return;
                }

                _selectedFilter = value;
                MatchingElements.Clear();
                QueryStatus = value == null
                    ? "Select a filter to inspect its targets."
                    : "Select Find Matching Elements to query this filter on demand.";
                OnPropertyChanged(nameof(SelectedFilter));
                OnPropertyChanged(nameof(HasMatchingElements));
            }
        }

        public string QueryStatus
        {
            get => _queryStatus;
            private set
            {
                _queryStatus = value;
                OnPropertyChanged(nameof(QueryStatus));
            }
        }

        public void LoadMatchingElements()
        {
            if (SelectedFilter == null)
            {
                QueryStatus = "Select a filter before finding matching elements.";
                return;
            }

            var result = _queryService.FindMatchingElements(_document, SelectedFilter);
            MatchingElements.Clear();
            foreach (var element in result.DisplayElements)
            {
                MatchingElements.Add(element);
            }

            QueryStatus = result.ErrorMessage
                ?? (result.MatchCount == 0
                    ? "No matching elements were found."
                    : string.Format("{0} matching element{1}{2}.", result.MatchCount, result.MatchCount == 1 ? string.Empty : "s", result.IsTruncated ? " The first 250 are shown" : string.Empty));
            OnPropertyChanged(nameof(HasMatchingElements));
        }

        private void RefreshFilterItems()
        {
            var previous = SelectedFilter;
            var search = SearchText.Trim();
            var results = _catalog.Where(item => string.IsNullOrWhiteSpace(search)
                || item.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                || item.CategoryNames.Any(category => category.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0))
                .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase).ToList();

            FilterItems.Clear();
            foreach (var item in results)
            {
                FilterItems.Add(item);
            }

            OnPropertyChanged(nameof(HasFilters));
            OnPropertyChanged(nameof(HasSearchResults));
            SelectedFilter = FilterItems.Contains(previous) ? previous : FilterItems.FirstOrDefault();
        }
    }
}
