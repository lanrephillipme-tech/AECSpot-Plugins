using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace AECSpot.QuickFilter
{
    public class QuickFilterSelectionService
    {
        public int Select(UIDocument uiDocument, QuickFilterPlan plan, bool activeViewOnly)
        {
            var ids = FindMatches(uiDocument.Document, uiDocument.ActiveView, plan, activeViewOnly);
            uiDocument.Selection.SetElementIds(ids);
            return ids.Count;
        }

        public IList<ElementId> FindMatches(Document document, View view, QuickFilterPlan plan, bool activeViewOnly)
        {
            var collector = activeViewOnly ? new FilteredElementCollector(document, view.Id) : new FilteredElementCollector(document);
            return collector.WhereElementIsNotElementType().WherePasses(new ElementMulticategoryFilter(plan.CategoryIds))
                .WherePasses(plan.ElementFilter).ToElementIds().ToList();
        }
    }
}
