using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Command-lifetime presentation model for one ParameterFilterElement.
    /// </summary>
    public class FilterCatalogItem
    {
        public ElementId FilterId { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public List<long> CategoryIdValues { get; set; } = new List<long>();
        public ElementFilter Definition { get; set; }
        public List<string> CategoryNames { get; set; } = new List<string>();
        public string RuleSummary { get; set; }
        public List<FilterUsageInfo> ViewUsages { get; set; } = new List<FilterUsageInfo>();
        public List<FilterUsageInfo> TemplateUsages { get; set; } = new List<FilterUsageInfo>();

        public int ViewUsageCount => ViewUsages.Count;
        public int TemplateUsageCount => TemplateUsages.Count;
        public bool IsUnused => ViewUsageCount == 0 && TemplateUsageCount == 0;
        public string UsageSummary => IsUnused
            ? "Unused"
            : string.Format("{0} view{1}, {2} template{3}",
                ViewUsageCount,
                ViewUsageCount == 1 ? string.Empty : "s",
                TemplateUsageCount,
                TemplateUsageCount == 1 ? string.Empty : "s");
        public string CategoriesSummary => CategoryNames.Any()
            ? string.Join(", ", CategoryNames)
            : "No categories could be resolved";
    }
}
