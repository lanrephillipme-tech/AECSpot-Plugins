using System;
using System.Collections.Generic;
using System.Linq;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Pure rename preview and collision validation. It does not access or modify Revit.
    /// </summary>
    public class FilterRenamePlanner
    {
        private static readonly char[] ProhibitedCharacters = { '{', '}', '[', ']', '|', ';', '<', '>', '?', '`', '~' };

        public FilterRenamePlan CreatePlan(IEnumerable<FilterCatalogItem> allFilters, IEnumerable<FilterCatalogItem> selectedFilters, FilterRenameRequest request)
        {
            var all = (allFilters ?? Enumerable.Empty<FilterCatalogItem>()).Where(filter => filter != null).ToList();
            var selected = (selectedFilters ?? Enumerable.Empty<FilterCatalogItem>()).Where(filter => filter != null).Distinct().ToList();
            request = request ?? new FilterRenameRequest();
            var plan = new FilterRenamePlan();

            if (!selected.Any())
            {
                plan.Errors.Add("Select at least one filter to rename.");
                return plan;
            }

            if (string.IsNullOrEmpty(request.FindText) && string.IsNullOrEmpty(request.Prefix) && string.IsNullOrEmpty(request.Suffix))
            {
                plan.Errors.Add("Enter a find value, prefix, or suffix before previewing changes.");
                return plan;
            }

            foreach (var filter in selected)
            {
                var renamed = string.IsNullOrEmpty(request.FindText)
                    ? filter.Name
                    : filter.Name.Replace(request.FindText, request.ReplaceText ?? string.Empty);
                renamed = (request.Prefix ?? string.Empty) + renamed + (request.Suffix ?? string.Empty);
                plan.Items.Add(new FilterRenamePlanItem { Filter = filter, ProposedName = renamed });
            }

            var selectedIds = new HashSet<long>(selected.Select(filter => filter.FilterId.Value));
            var unselectedNames = new HashSet<string>(all.Where(filter => !selectedIds.Contains(filter.FilterId.Value)).Select(filter => filter.Name), StringComparer.OrdinalIgnoreCase);
            var selectedOriginalNames = selected.ToDictionary(filter => filter.FilterId.Value, filter => filter.Name, EqualityComparer<long>.Default);

            foreach (var item in plan.Items)
            {
                item.ValidationMessage = ValidateName(item.ProposedName);
                if (item.IsValid && unselectedNames.Contains(item.ProposedName))
                {
                    item.ValidationMessage = "This name is already used by an unselected filter.";
                }

                if (item.IsValid)
                {
                    var otherSelectedOwnsName = selectedOriginalNames.Any(pair => pair.Key != item.Filter.FilterId.Value && string.Equals(pair.Value, item.ProposedName, StringComparison.OrdinalIgnoreCase));
                    if (otherSelectedOwnsName)
                    {
                        item.ValidationMessage = "This name is currently used by another selected filter; rename cycles are not applied automatically.";
                    }
                }
            }

            foreach (var group in plan.Items.Where(item => item.IsValid).GroupBy(item => item.ProposedName, StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1))
            {
                foreach (var item in group)
                {
                    item.ValidationMessage = "More than one selected filter would receive this name.";
                }
            }

            if (!plan.Items.Any(item => item.IsChange))
            {
                plan.Errors.Add("The requested operation does not change any selected filter names.");
            }

            return plan;
        }

        private static string ValidateName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Filter names cannot be empty.";
            }

            return value.IndexOfAny(ProhibitedCharacters) >= 0
                ? "The proposed name contains a character that Revit does not permit."
                : null;
        }
    }
}
