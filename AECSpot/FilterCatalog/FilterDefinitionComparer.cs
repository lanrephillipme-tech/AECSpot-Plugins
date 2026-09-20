using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Produces conservative, deterministic signatures for supported Revit 2024 filter definitions.
    /// A signature is emitted only when every relevant node can be described without guessing.
    /// </summary>
    public class FilterDefinitionComparer
    {
        public FilterComparisonOutcome Compare(FilterCatalogItem left, FilterCatalogItem right)
        {
            var leftResult = Normalize(left);
            var rightResult = Normalize(right);
            if (leftResult.Outcome == FilterComparisonOutcome.Indeterminate
                || rightResult.Outcome == FilterComparisonOutcome.Indeterminate)
            {
                return FilterComparisonOutcome.Indeterminate;
            }

            return string.Equals(leftResult.Signature, rightResult.Signature, StringComparison.Ordinal)
                ? FilterComparisonOutcome.Equivalent
                : FilterComparisonOutcome.NotEquivalent;
        }

        public DuplicateFilterAnalysis Analyze(IEnumerable<FilterCatalogItem> catalog)
        {
            var results = (catalog ?? Enumerable.Empty<FilterCatalogItem>())
                .Select(Normalize)
                .ToList();

            var analysis = new DuplicateFilterAnalysis
            {
                IndeterminateFilters = results
                    .Where(result => result.Outcome == FilterComparisonOutcome.Indeterminate)
                    .OrderBy(result => result.Filter.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList()
            };

            analysis.DuplicateGroups = results
                .Where(result => result.Outcome == FilterComparisonOutcome.Equivalent)
                .GroupBy(result => result.Signature, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => new DuplicateFilterGroup
                {
                    Filters = group.Select(result => result.Filter)
                        .OrderBy(filter => filter.Name, StringComparer.OrdinalIgnoreCase)
                        .ToList(),
                    DefinitionSummary = group.First().Filter.RuleSummary,
                    CategoriesSummary = group.First().Filter.CategoriesSummary
                })
                .OrderBy(group => group.Filters.First().Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return analysis;
        }

        private static FilterNormalizationResult Normalize(FilterCatalogItem filter)
        {
            if (filter == null || filter.Definition == null)
            {
                return Indeterminate(filter, "The filter definition could not be read.");
            }

            string definitionSignature;
            string reason;
            if (!TryNormalizeElementFilter(filter.Definition, out definitionSignature, out reason))
            {
                return Indeterminate(filter, reason);
            }

            var categorySignature = string.Join(",", filter.CategoryIdValues.OrderBy(value => value));
            return new FilterNormalizationResult
            {
                Filter = filter,
                Outcome = FilterComparisonOutcome.Equivalent,
                Signature = "C[" + categorySignature + "]|" + definitionSignature,
                Detail = "Comparable definition"
            };
        }

        private static FilterNormalizationResult Indeterminate(FilterCatalogItem filter, string reason)
        {
            return new FilterNormalizationResult
            {
                Filter = filter,
                Outcome = FilterComparisonOutcome.Indeterminate,
                Detail = reason
            };
        }

        private static bool TryNormalizeElementFilter(ElementFilter filter, out string signature, out string reason)
        {
            signature = null;
            reason = null;

            var parameterFilter = filter as ElementParameterFilter;
            if (parameterFilter != null)
            {
                var ruleSignatures = new List<string>();
                foreach (var rule in parameterFilter.GetRules())
                {
                    string ruleSignature;
                    if (!TryNormalizeRule(rule, out ruleSignature, out reason))
                    {
                        return false;
                    }

                    ruleSignatures.Add(ruleSignature);
                }

                if (!ruleSignatures.Any())
                {
                    reason = "The parameter filter contains no readable rules.";
                    return false;
                }

                // Rules in an ElementParameterFilter are an AND set. Sort to ignore non-semantic API order.
                signature = "PARAM[I=" + parameterFilter.Inverted + "|" + string.Join(",", ruleSignatures.OrderBy(value => value, StringComparer.Ordinal)) + "]";
                return true;
            }

            var logicalFilter = filter as ElementLogicalFilter;
            if (logicalFilter != null)
            {
                var kind = filter is LogicalAndFilter ? "AND" : filter is LogicalOrFilter ? "OR" : null;
                if (kind == null)
                {
                    reason = "This logical filter type is not supported for safe comparison.";
                    return false;
                }

                var childSignatures = new List<string>();
                foreach (var child in logicalFilter.GetFilters())
                {
                    string childSignature;
                    if (!TryNormalizeElementFilter(child, out childSignature, out reason))
                    {
                        return false;
                    }

                    childSignatures.Add(childSignature);
                }

                if (!childSignatures.Any())
                {
                    reason = "The logical filter contains no readable child filters.";
                    return false;
                }

                // AND and OR are commutative; normalize child order while retaining the logical structure.
                signature = kind + "[I=" + logicalFilter.Inverted + "|" + string.Join(",", childSignatures.OrderBy(value => value, StringComparer.Ordinal)) + "]";
                return true;
            }

            reason = "This ElementFilter type is not supported for safe comparison.";
            return false;
        }

        private static bool TryNormalizeRule(FilterRule rule, out string signature, out string reason)
        {
            signature = null;
            reason = null;
            if (rule == null)
            {
                reason = "The filter contains a null rule.";
                return false;
            }

            var inverse = rule as FilterInverseRule;
            if (inverse != null)
            {
                string innerSignature;
                if (!TryNormalizeRule(inverse.GetInnerRule(), out innerSignature, out reason))
                {
                    return false;
                }

                signature = "NOT[" + innerSignature + "]";
                return true;
            }

            var categoryRule = rule as FilterCategoryRule;
            if (categoryRule != null)
            {
                var categoryIds = categoryRule.GetCategories().Select(id => id.Value).OrderBy(value => value);
                signature = "CATEGORY[" + string.Join(",", categoryIds) + "]";
                return true;
            }

            var sharedParameterRule = rule as SharedParameterApplicableRule;
            if (sharedParameterRule != null)
            {
                signature = "SHARED_PARAMETER[" + Escape(sharedParameterRule.ParameterName) + "]";
                return true;
            }

            var parameterId = rule.GetRuleParameter();
            if (parameterId == null || parameterId == ElementId.InvalidElementId)
            {
                reason = "This rule has no stable parameter identifier.";
                return false;
            }

            var type = rule.GetType();

            // HasValueFilterRule and HasNoValueFilterRule are parameter-only predicates.
            if (rule is HasValueFilterRule || rule is HasNoValueFilterRule)
            {
                signature = "PRESENCE[" + type.FullName + "|P=" + parameterId.Value + "]";
                return true;
            }

            var evaluator = GetEvaluatorName(rule);
            if (evaluator == null)
            {
                reason = "This rule evaluator could not be read safely.";
                return false;
            }

            if (rule is FilterStringRule)
            {
                var value = ((FilterStringRule)rule).RuleString;
                signature = "STRING[" + type.FullName + "|P=" + parameterId.Value + "|E=" + evaluator + "|V=" + Escape(value) + "]";
                return true;
            }

            if (rule is FilterDoubleRule)
            {
                var doubleRule = (FilterDoubleRule)rule;
                signature = "DOUBLE[" + type.FullName + "|P=" + parameterId.Value + "|E=" + evaluator + "|V=" + doubleRule.RuleValue.ToString("R", CultureInfo.InvariantCulture) + "|EPS=" + doubleRule.Epsilon.ToString("R", CultureInfo.InvariantCulture) + "]";
                return true;
            }

            if (rule is FilterIntegerRule)
            {
                signature = "INTEGER[" + type.FullName + "|P=" + parameterId.Value + "|E=" + evaluator + "|V=" + ((FilterIntegerRule)rule).RuleValue.ToString(CultureInfo.InvariantCulture) + "]";
                return true;
            }

            if (rule is FilterElementIdRule)
            {
                var value = ((FilterElementIdRule)rule).RuleValue;
                if (value == null)
                {
                    reason = "This ElementId rule has no readable comparison value.";
                    return false;
                }

                signature = "ELEMENT_ID[" + type.FullName + "|P=" + parameterId.Value + "|E=" + evaluator + "|V=" + value.Value + "]";
                return true;
            }

            reason = "This FilterRule type is not supported for safe comparison.";
            return false;
        }

        private static string GetEvaluatorName(FilterRule rule)
        {
            try
            {
                var method = rule.GetType().GetMethod("GetEvaluator", BindingFlags.Public | BindingFlags.Instance);
                var evaluator = method == null ? null : method.Invoke(rule, null);
                return evaluator == null ? null : evaluator.GetType().FullName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("|", "\\|").Replace("]", "\\]");
        }
    }
}
