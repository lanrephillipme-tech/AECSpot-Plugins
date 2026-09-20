using System.Collections.Generic;
using System.Linq;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Creates a read-only replacement preview from the catalog already built for the command.
    /// </summary>
    public class FilterReplacementPlanner
    {
        public FilterReplacementPlan CreatePlan(FilterCatalogItem source, FilterCatalogItem replacement)
        {
            var usages = source == null
                ? new List<FilterUsageInfo>()
                : source.ViewUsages.Concat(source.TemplateUsages).ToList();
            var replacementUsageIds = replacement == null
                ? new HashSet<long>()
                : new HashSet<long>(replacement.ViewUsages.Concat(replacement.TemplateUsages).Select(usage => usage.ViewId.Value));

            return new FilterReplacementPlan
            {
                SourceFilter = source,
                ReplacementFilter = replacement,
                Usages = usages,
                ExistingReplacementAssignments = usages.Count(usage => replacementUsageIds.Contains(usage.ViewId.Value))
            };
        }
    }
}
