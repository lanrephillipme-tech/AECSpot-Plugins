using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AECSpot.QuickFilter
{
    /// <summary>Builds an in-memory, AND-only filter definition. It never changes the document.</summary>
    public class QuickFilterBuilderService
    {
        public IList<QuickFilterCategoryItem> GetCategories(Document document)
        {
            return ParameterFilterUtilities.GetAllFilterableCategories()
                .Select(id => new { Id = id, Category = Category.GetCategory(document, id) })
                .Where(x => x.Category != null)
                .OrderBy(x => x.Category.Name, StringComparer.OrdinalIgnoreCase)
                .Select(x => new QuickFilterCategoryItem { Id = x.Id, Name = x.Category.Name })
                .ToList();
        }

        public IList<QuickFilterParameterItem> GetCommonParameters(Document document, ICollection<ElementId> categories)
        {
            if (categories == null || categories.Count == 0) return new List<QuickFilterParameterItem>();
            var result = new List<QuickFilterParameterItem>();
            foreach (var id in ParameterFilterUtilities.GetFilterableParametersInCommon(document, categories))
            {
                var name = GetParameterName(document, id);
                var metadata = FindParameterMetadata(document, categories, id);
                if (metadata != null && metadata.StorageType != StorageType.ElementId)
                    result.Add(new QuickFilterParameterItem { Id = id, Name = name, StorageType = metadata.StorageType, DataTypeId = metadata.DataTypeId });
            }
            return result.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public QuickFilterPlan BuildPlan(Document document, IEnumerable<QuickFilterCategoryItem> categories, IEnumerable<QuickFilterRuleDraft> drafts)
        {
            var categoryIds = (categories ?? Enumerable.Empty<QuickFilterCategoryItem>()).Where(x => x.IsSelected).Select(x => x.Id).ToList();
            if (categoryIds.Count == 0) return new QuickFilterPlan { ErrorMessage = "Select at least one filterable category." };
            var rules = new List<FilterRule>();
            foreach (var draft in drafts ?? Enumerable.Empty<QuickFilterRuleDraft>())
            {
                if (draft.Parameter == null || string.IsNullOrWhiteSpace(draft.Operator) || string.IsNullOrWhiteSpace(draft.Value))
                    return new QuickFilterPlan { ErrorMessage = "Each rule needs a parameter, operator, and value." };
                try { rules.Add(CreateRule(document, draft)); }
                catch (FormatException) { return new QuickFilterPlan { ErrorMessage = "One or more rule values are not valid for the selected parameter type." }; }
                catch (NotSupportedException) { return new QuickFilterPlan { ErrorMessage = "One or more rule operators are not supported for the selected parameter type." }; }
            }
            if (rules.Count == 0) return new QuickFilterPlan { ErrorMessage = "Add at least one rule." };
            var filter = new ElementParameterFilter(rules);
            if (!ParameterFilterElement.ElementFilterIsAcceptableForParameterFilterElement(document, new HashSet<ElementId>(categoryIds), filter))
                return new QuickFilterPlan { ErrorMessage = "This category and rule combination is not accepted by Revit." };
            return new QuickFilterPlan { CategoryIds = categoryIds, ElementFilter = filter };
        }

        private static FilterRule CreateRule(Document document, QuickFilterRuleDraft draft)
        {
            var id = draft.Parameter.Id;
            var value = draft.Value.Trim();
            if (draft.Parameter.StorageType == StorageType.String)
            {
                switch (draft.Operator) {
                    case "Equals": return ParameterFilterRuleFactory.CreateEqualsRule(id, value);
                    case "Not equals": return ParameterFilterRuleFactory.CreateNotEqualsRule(id, value);
                    case "Contains": return ParameterFilterRuleFactory.CreateContainsRule(id, value);
                    case "Does not contain": return ParameterFilterRuleFactory.CreateNotContainsRule(id, value);
                    case "Begins with": return ParameterFilterRuleFactory.CreateBeginsWithRule(id, value);
                    case "Ends with": return ParameterFilterRuleFactory.CreateEndsWithRule(id, value);
                    default: throw new NotSupportedException();
                }
            }
            if (draft.Parameter.StorageType == StorageType.Integer)
            {
                var integer = ParseInteger(value);
                switch (draft.Operator) {
                    case "Equals": return ParameterFilterRuleFactory.CreateEqualsRule(id, integer);
                    case "Not equals": return ParameterFilterRuleFactory.CreateNotEqualsRule(id, integer);
                    case "Greater than": return ParameterFilterRuleFactory.CreateGreaterRule(id, integer);
                    case "Greater than or equal": return ParameterFilterRuleFactory.CreateGreaterOrEqualRule(id, integer);
                    case "Less than": return ParameterFilterRuleFactory.CreateLessRule(id, integer);
                    case "Less than or equal": return ParameterFilterRuleFactory.CreateLessOrEqualRule(id, integer);
                    default: throw new NotSupportedException();
                }
            }
            if (draft.Parameter.StorageType == StorageType.Double)
            {
                double number;
                if (draft.Parameter.DataTypeId != null && UnitUtils.IsMeasurableSpec(draft.Parameter.DataTypeId))
                {
                    if (!UnitFormatUtils.TryParse(document.GetUnits(), draft.Parameter.DataTypeId, value, out number))
                        throw new FormatException();
                }
                else if (!double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out number))
                    throw new FormatException();
                const double epsilon = 1e-9;
                switch (draft.Operator) {
                    case "Equals": return ParameterFilterRuleFactory.CreateEqualsRule(id, number, epsilon);
                    case "Not equals": return ParameterFilterRuleFactory.CreateNotEqualsRule(id, number, epsilon);
                    case "Greater than": return ParameterFilterRuleFactory.CreateGreaterRule(id, number, epsilon);
                    case "Greater than or equal": return ParameterFilterRuleFactory.CreateGreaterOrEqualRule(id, number, epsilon);
                    case "Less than": return ParameterFilterRuleFactory.CreateLessRule(id, number, epsilon);
                    case "Less than or equal": return ParameterFilterRuleFactory.CreateLessOrEqualRule(id, number, epsilon);
                    default: throw new NotSupportedException();
                }
            }
            throw new NotSupportedException();
        }

        private static int ParseInteger(string value)
        {
            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase)) return 1;
            if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "no", StringComparison.OrdinalIgnoreCase)) return 0;
            return int.Parse(value, CultureInfo.CurrentCulture);
        }

        private static string GetParameterName(Document document, ElementId id)
        {
            var element = document.GetElement(id) as ParameterElement;
            if (element != null) return element.GetDefinition().Name;
            try { return LabelUtils.GetLabelFor((BuiltInParameter)id.Value); }
            catch { return "Parameter " + id.Value; }
        }

        private static ParameterMetadata FindParameterMetadata(Document document, ICollection<ElementId> categories, ElementId parameterId)
        {
            try
            {
                var collector = new FilteredElementCollector(document).WhereElementIsNotElementType()
                    .WherePasses(new ElementMulticategoryFilter(categories));
                foreach (var element in collector)
                {
                    var parameter = element.Parameters.Cast<Parameter>().FirstOrDefault(x => x.Id == parameterId);
                    if (parameter != null) return new ParameterMetadata { StorageType = parameter.StorageType, DataTypeId = parameter.Definition.GetDataType() };
                }
            }
            catch (Autodesk.Revit.Exceptions.ApplicationException) { }
            return null;
        }

        private class ParameterMetadata
        {
            public StorageType StorageType { get; set; }
            public ForgeTypeId DataTypeId { get; set; }
        }
    }
}
