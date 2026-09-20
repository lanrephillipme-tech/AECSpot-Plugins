using System.Collections.Generic;

namespace AECSpot.FilterCatalog
{
    public class FilterReplacementPlan
    {
        public FilterCatalogItem SourceFilter { get; set; }
        public FilterCatalogItem ReplacementFilter { get; set; }
        public List<FilterUsageInfo> Usages { get; set; } = new List<FilterUsageInfo>();
        public int ExistingReplacementAssignments { get; set; }
        public bool IsValid => SourceFilter != null && ReplacementFilter != null && SourceFilter.FilterId.Value != ReplacementFilter.FilterId.Value && Usages.Count > 0;
    }
}
