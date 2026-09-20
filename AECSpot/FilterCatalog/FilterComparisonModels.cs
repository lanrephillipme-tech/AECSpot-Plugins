using System.Collections.Generic;

namespace AECSpot.FilterCatalog
{
    public enum FilterComparisonOutcome
    {
        Equivalent,
        NotEquivalent,
        Indeterminate
    }

    public class FilterNormalizationResult
    {
        public FilterCatalogItem Filter { get; set; }
        public FilterComparisonOutcome Outcome { get; set; }
        public string Signature { get; set; }
        public string Detail { get; set; }
    }

    public class DuplicateFilterGroup
    {
        public List<FilterCatalogItem> Filters { get; set; } = new List<FilterCatalogItem>();
        public string DefinitionSummary { get; set; }
        public string CategoriesSummary { get; set; }
        public int Count => Filters.Count;
        public string GroupName => string.Format("Duplicate group ({0} filters)", Count);
    }

    public class DuplicateFilterAnalysis
    {
        public List<DuplicateFilterGroup> DuplicateGroups { get; set; } = new List<DuplicateFilterGroup>();
        public List<FilterNormalizationResult> IndeterminateFilters { get; set; } = new List<FilterNormalizationResult>();
    }
}
