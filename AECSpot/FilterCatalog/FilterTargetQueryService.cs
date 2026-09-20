using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Resolves matching elements only after a user selects a catalog filter.
    /// </summary>
    public class FilterTargetQueryService
    {
        private const int DisplayLimit = 250;

        public FilterTargetQueryResult FindMatchingElements(Document document, FilterCatalogItem filter)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (filter == null || filter.Definition == null || !filter.CategoryIdValues.Any())
            {
                return new FilterTargetQueryResult { ErrorMessage = "This filter does not have a readable definition and category set." };
            }

            try
            {
                var categoryIds = filter.CategoryIdValues.Select(value => new ElementId(value)).ToList();
                var collector = new FilteredElementCollector(document)
                    .WhereElementIsNotElementType()
                    .WherePasses(new ElementMulticategoryFilter(categoryIds))
                    .WherePasses(filter.Definition);

                var result = new FilterTargetQueryResult();
                foreach (var element in collector)
                {
                    result.MatchCount++;
                    if (result.DisplayElements.Count < DisplayLimit)
                    {
                        result.DisplayElements.Add(new FilterTargetElementInfo
                        {
                            ElementId = element.Id.Value.ToString(),
                            Name = GetElementName(element),
                            CategoryName = GetCategoryName(element)
                        });
                    }
                }

                result.IsTruncated = result.MatchCount > DisplayLimit;
                return result;
            }
            catch (Exception)
            {
                return new FilterTargetQueryResult { ErrorMessage = "Matching elements could not be read for this filter." };
            }
        }

        private static string GetElementName(Element element)
        {
            try
            {
                return string.IsNullOrWhiteSpace(element.Name) ? "Unnamed element" : element.Name;
            }
            catch (Exception)
            {
                return "Unnamed element";
            }
        }

        private static string GetCategoryName(Element element)
        {
            try
            {
                return element.Category == null ? "No category" : element.Category.Name;
            }
            catch (Exception)
            {
                return "No category";
            }
        }
    }
}
