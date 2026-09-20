using System.Collections.Generic;
using System.Linq;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Pure planner for filters with no direct project-view or template assignments.
    /// </summary>
    public class FilterPurgePlanner
    {
        public List<FilterCatalogItem> GetUnusedFilters(IEnumerable<FilterCatalogItem> catalog)
        {
            return (catalog ?? Enumerable.Empty<FilterCatalogItem>())
                .Where(filter => filter != null && filter.IsUnused)
                .OrderBy(filter => filter.Name, System.StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public FilterPurgePlan CreatePlan(IEnumerable<FilterCatalogItem> selectedFilters)
        {
            return new FilterPurgePlan
            {
                Filters = (selectedFilters ?? Enumerable.Empty<FilterCatalogItem>())
                    .Where(filter => filter != null && filter.IsUnused)
                    .Distinct()
                    .ToList()
            };
        }
    }
}
