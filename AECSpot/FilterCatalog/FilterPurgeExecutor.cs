using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Revalidates direct filter usage and deletes a planned set atomically.
    /// </summary>
    public class FilterPurgeExecutor
    {
        public void Execute(Document document, FilterPurgePlan plan)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (plan == null || !plan.IsValid)
            {
                throw new InvalidOperationException("The purge plan is not valid.");
            }

            var plannedIds = new HashSet<long>(plan.Filters.Select(filter => filter.FilterId.Value));
            ValidateUnused(document, plannedIds);

            using (var transaction = new Transaction(document, "Purge Unused Filters"))
            {
                transaction.Start();
                foreach (var filter in plan.Filters)
                {
                    if (!(document.GetElement(filter.FilterId) is ParameterFilterElement))
                    {
                        throw new InvalidOperationException("A planned filter is no longer available in this project.");
                    }

                    document.Delete(filter.FilterId);
                }

                transaction.Commit();
            }
        }

        private static void ValidateUnused(Document document, ISet<long> plannedIds)
        {
            var views = new FilteredElementCollector(document).OfClass(typeof(View)).Cast<View>().ToList();
            foreach (var view in views)
            {
                ICollection<ElementId> appliedIds;
                try
                {
                    appliedIds = view.GetFilters();
                }
                catch (Exception)
                {
                    // Unsupported view types do not expose filters and cannot prove a direct assignment.
                    continue;
                }

                if (appliedIds.Any(id => plannedIds.Contains(id.Value)))
                {
                    throw new InvalidOperationException("A planned filter is currently applied to a project view or view template.");
                }
            }
        }
    }
}
