using System.Collections.Generic;

namespace AECSpot.FilterCatalog
{
    public class FilterPurgePlan
    {
        public List<FilterCatalogItem> Filters { get; set; } = new List<FilterCatalogItem>();
        public bool IsValid => Filters.Count > 0;
    }
}
