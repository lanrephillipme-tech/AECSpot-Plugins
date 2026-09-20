using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Replaces direct filter assignments and carries source state to the replacement in one transaction.
    /// The source ParameterFilterElement itself is intentionally retained.
    /// </summary>
    public class FilterReplacementExecutor
    {
        public void Execute(Document document, FilterReplacementPlan plan)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (plan == null || !plan.IsValid)
            {
                throw new InvalidOperationException("The filter replacement plan is not valid.");
            }

            using (var transaction = new Transaction(document, "Replace View Filters"))
            {
                transaction.Start();
                foreach (var usage in plan.Usages)
                {
                    var view = document.GetElement(usage.ViewId) as View;
                    if (view == null)
                    {
                        throw new InvalidOperationException("A view or view template in the replacement plan is no longer available.");
                    }

                    ReplaceOnView(view, plan.SourceFilter.FilterId, plan.ReplacementFilter.FilterId);
                }

                transaction.Commit();
            }
        }

        private static void ReplaceOnView(View view, ElementId sourceId, ElementId replacementId)
        {
            var currentIds = view.GetFilters().ToList();
            if (!currentIds.Any(id => id.Value == sourceId.Value))
            {
                throw new InvalidOperationException("A planned source filter assignment is no longer present.");
            }

            var sourceState = ReadState(view, sourceId);
            var replacementPresent = currentIds.Any(id => id.Value == replacementId.Value);
            if (!replacementPresent)
            {
                view.AddFilter(replacementId);
            }

            // Source state is authoritative for this replace operation, including when the replacement was already assigned.
            view.SetFilterVisibility(replacementId, sourceState.IsVisible);
            view.SetFilterOverrides(replacementId, sourceState.Overrides);
            SetEnabledStateIfSupported(view, replacementId, sourceState.IsEnabled);

            var orderedIds = TryGetOrderedFilters(view) ?? currentIds;
            var sourceIndex = orderedIds.FindIndex(id => id.Value == sourceId.Value);
            view.RemoveFilter(sourceId);
            TrySetReplacementOrder(view, orderedIds, sourceId, replacementId, sourceIndex);
        }

        private static ViewFilterState ReadState(View view, ElementId filterId)
        {
            return new ViewFilterState
            {
                IsVisible = view.GetFilterVisibility(filterId),
                Overrides = view.GetFilterOverrides(filterId),
                IsEnabled = GetEnabledStateIfSupported(view, filterId)
            };
        }

        private static bool? GetEnabledStateIfSupported(View view, ElementId filterId)
        {
            try
            {
                var method = typeof(View).GetMethod("GetIsFilterEnabled", new[] { typeof(ElementId) });
                return method == null ? (bool?)null : (bool)method.Invoke(view, new object[] { filterId });
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void SetEnabledStateIfSupported(View view, ElementId filterId, bool? enabled)
        {
            if (!enabled.HasValue)
            {
                return;
            }

            var method = typeof(View).GetMethod("SetIsFilterEnabled", new[] { typeof(ElementId), typeof(bool) });
            if (method != null)
            {
                method.Invoke(view, new object[] { filterId, enabled.Value });
            }
        }

        private static List<ElementId> TryGetOrderedFilters(View view)
        {
            try
            {
                var method = typeof(View).GetMethod("GetOrderedFilters", Type.EmptyTypes);
                var values = method == null ? null : method.Invoke(view, null) as IEnumerable;
                return values == null ? null : values.Cast<ElementId>().ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void TrySetReplacementOrder(View view, List<ElementId> originalOrder, ElementId sourceId, ElementId replacementId, int sourceIndex)
        {
            try
            {
                var method = typeof(View).GetMethod("SetOrderedFilters", new[] { typeof(IList<ElementId>) });
                if (method == null || sourceIndex < 0)
                {
                    return;
                }

                var reordered = originalOrder.Where(id => id.Value != sourceId.Value && id.Value != replacementId.Value).ToList();
                reordered.Insert(Math.Min(sourceIndex, reordered.Count), replacementId);
                method.Invoke(view, new object[] { reordered });
            }
            catch (Exception)
            {
                // Ordering is optional; assignments, visibility, enabled state, and overrides remain valid.
            }
        }

        private class ViewFilterState
        {
            public bool IsVisible { get; set; }
            public bool? IsEnabled { get; set; }
            public OverrideGraphicSettings Overrides { get; set; }
        }
    }
}
