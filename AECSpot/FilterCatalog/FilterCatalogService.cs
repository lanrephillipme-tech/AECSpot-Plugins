using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Builds a read-only, command-lifetime index of rule-based Revit filters and their view usage.
    /// </summary>
    public class FilterCatalogService
    {
        public List<FilterCatalogItem> Build(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var catalogById = new Dictionary<ElementId, FilterCatalogItem>();
            var filters = new FilteredElementCollector(document)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .ToList();

            foreach (var filter in filters)
            {
                if (filter == null || filter.Id == null || filter.Id == ElementId.InvalidElementId)
                {
                    continue;
                }

                catalogById[filter.Id] = new FilterCatalogItem
                {
                    FilterId = filter.Id,
                    UniqueId = filter.UniqueId,
                    Name = filter.Name,
                    CategoryIdValues = filter.GetCategories().Select(categoryId => categoryId.Value).OrderBy(value => value).ToList(),
                    Definition = filter.GetElementFilter(),
                    CategoryNames = GetCategoryNames(document, filter),
                    RuleSummary = GetRuleSummary(filter)
                };
            }

            // Views are collected once. A failed optional property read on one view must not
            // prevent the rest of the catalog from being available.
            var views = new FilteredElementCollector(document)
                .OfClass(typeof(View))
                .Cast<View>()
                .ToList();

            foreach (var view in views)
            {
                IndexViewUsages(view, catalogById);
            }

            return catalogById.Values
                .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static List<string> GetCategoryNames(Document document, ParameterFilterElement filter)
        {
            var names = new List<string>();
            try
            {
                foreach (var categoryId in filter.GetCategories())
                {
                    var category = Category.GetCategory(document, categoryId);
                    if (category != null && !string.IsNullOrWhiteSpace(category.Name))
                    {
                        names.Add(category.Name);
                    }
                }
            }
            catch (Exception)
            {
                // Category display is optional; retain the filter when a legacy/invalid category cannot be read.
            }

            return names.Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string GetRuleSummary(ParameterFilterElement filter)
        {
            try
            {
                var elementFilter = filter.GetElementFilter();
                if (elementFilter == null)
                {
                    return "No rule details available";
                }

                if (elementFilter is ElementParameterFilter)
                {
                    return "Parameter rule";
                }

                return "Complex filter rule";
            }
            catch (Exception)
            {
                return "Complex filter rule";
            }
        }

        private static void IndexViewUsages(View view, IDictionary<ElementId, FilterCatalogItem> catalogById)
        {
            if (view == null || view.Id == null || view.Id == ElementId.InvalidElementId)
            {
                return;
            }

            ICollection<ElementId> appliedFilterIds;
            try
            {
                appliedFilterIds = view.GetFilters();
            }
            catch (Exception)
            {
                // Some Revit view types do not support view filters. They simply have no catalog entries.
                return;
            }

            foreach (var filterId in appliedFilterIds)
            {
                FilterCatalogItem item;
                if (filterId == null || !catalogById.TryGetValue(filterId, out item))
                {
                    continue;
                }

                var isTemplate = TryGetIsTemplate(view);
                var usage = new FilterUsageInfo
                {
                    ViewId = view.Id,
                    ViewName = TryGetViewName(view),
                    ViewTypeName = TryGetViewTypeName(view),
                    IsTemplate = isTemplate,
                    IsVisible = TryGetVisibility(view, filterId),
                    IsEnabled = TryGetEnabledState(view, filterId),
                    HasGraphicOverrides = TryGetGraphicOverrideState(view, filterId)
                };

                if (usage.IsTemplate)
                {
                    item.TemplateUsages.Add(usage);
                }
                else
                {
                    item.ViewUsages.Add(usage);
                }
            }
        }

        private static bool? TryGetVisibility(View view, ElementId filterId)
        {
            try
            {
                return view.GetFilterVisibility(filterId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool TryGetIsTemplate(View view)
        {
            try
            {
                return view.IsTemplate;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string TryGetViewName(View view)
        {
            try
            {
                return view.Name;
            }
            catch (Exception)
            {
                return "Unnamed view";
            }
        }

        private static string TryGetViewTypeName(View view)
        {
            try
            {
                return view.ViewType.ToString();
            }
            catch (Exception)
            {
                return "Unknown view type";
            }
        }

        private static bool? TryGetEnabledState(View view, ElementId filterId)
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

        private static bool? TryGetGraphicOverrideState(View view, ElementId filterId)
        {
            try
            {
                var overrides = view.GetFilterOverrides(filterId);
                if (overrides == null)
                {
                    return null;
                }

                var isEmpty = typeof(OverrideGraphicSettings).GetMethod("IsEmpty", Type.EmptyTypes);
                if (isEmpty != null)
                {
                    return !(bool)isEmpty.Invoke(overrides, null);
                }

                var isEmptyProperty = typeof(OverrideGraphicSettings).GetProperty("IsEmpty");
                return isEmptyProperty == null ? (bool?)null : !(bool)isEmptyProperty.GetValue(overrides, null);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
