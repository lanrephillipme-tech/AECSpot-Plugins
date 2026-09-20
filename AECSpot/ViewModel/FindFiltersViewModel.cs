using AECSpot.FilterCatalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AECSpot
{
    public class FindFiltersViewModel : ViewModelBase
    {
        private readonly List<FilterCatalogItem> _catalog;
        private string _searchText = string.Empty;
        private FilterCatalogItem _selectedFilter;

        public FindFiltersViewModel(IEnumerable<FilterCatalogItem> catalog)
        {
            _catalog = (catalog ?? Enumerable.Empty<FilterCatalogItem>()).ToList();
            FilterItems = new ObservableCollection<FilterCatalogItem>();
            RefreshFilterItems();
            SelectedFilter = FilterItems.FirstOrDefault();
        }

        public ObservableCollection<FilterCatalogItem> FilterItems { get; }

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
                OnPropertyChanged(nameof(SelectedFilter));
            }
        }

        public bool HasFilters => _catalog.Count > 0;
        public bool HasSearchResults => FilterItems.Count > 0;

        private void RefreshFilterItems()
        {
            var previousSelection = SelectedFilter;
            var search = SearchText.Trim();
            var results = _catalog
                .Where(item => string.IsNullOrWhiteSpace(search)
                    || item.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                    || item.CategoryNames.Any(category => category.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0))
                .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            FilterItems.Clear();
            foreach (var item in results)
            {
                FilterItems.Add(item);
            }

            OnPropertyChanged(nameof(HasFilters));
            OnPropertyChanged(nameof(HasSearchResults));
            SelectedFilter = FilterItems.Contains(previousSelection) ? previousSelection : FilterItems.FirstOrDefault();
        }
    }
}
