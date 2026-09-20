using AECSpot;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterManager
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class CreateFilterCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Current Application Session

            ///geting the current  ui document
            UIDocument _uiDoc = commandData.Application.ActiveUIDocument;
            ///geting the current document
            Document _currentDoc = commandData.Application.ActiveUIDocument.Document;

            #endregion Current Application Session
            try
            {
                var filter = CreateFilter(_currentDoc, new List<ElementId>() { new ElementId(BuiltInCategory.OST_Entourage) }, new ElementId(BuiltInParameter.ALL_MODEL_MODEL));
                return Result.Succeeded;
            }
            catch (Exception e)
            {

                message = e.Message;

                return Result.Failed;
            }

        }

        public ParameterFilterElement CreateFilter(Document doc, List<ElementId> categories, ElementId ParametertypeNameId, string FilterName = "AECSPOT 3D PEOPLE", string ElementContainValue = "AEC3DP")
        {

            var IsFilterExisit = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement)).FirstOrDefault(pa => pa.Name == FilterName);
            try
            {
                IList<FilterRule> rules = new List<FilterRule>();
                rules.Add(ParameterFilterRuleFactory.CreateContainsRule(ParametertypeNameId, ElementContainValue));
                var view = doc.ActiveView;
                var viewFilters = view.GetFilters();
                if (IsFilterExisit != null)
                {
                    AssignFilter(doc, categories, FilterName, rules, view, false, (ParameterFilterElement)IsFilterExisit);
                    return (ParameterFilterElement)IsFilterExisit;
                }
                else
                {
                    AssignFilter(doc, categories, FilterName, rules, view, true);
                    return null;
                }
            }
            catch (System.Exception)
            {
                return null;
            }

        }

        private static void AssignFilter(Document doc, List<ElementId> categories, string FilterName, IList<FilterRule> rules, Autodesk.Revit.DB.View view, bool CreateFilter, ParameterFilterElement filterElement = null)
        {
            using (Transaction t = new Transaction(doc, "Create and Apply Filter"))
            {

                t.Start();
                if (CreateFilter)
                {
#if REVIT2018
                    var filter = ParameterFilterElement.Create(doc, FilterName, categories, rules);

#else
                    var filter = ParameterFilterElement.Create(doc, FilterName, categories, new ElementParameterFilter(rules));
#endif
                    view.AddFilter(filter.Id);
                    view.SetFilterVisibility(filter.Id, true);
                    AssignFilterOverrides(doc, view, filter, new OverrideGraphicSettings());
                    // MessageBox.Show("Filter Created Successfully.");
                }
                else
                {
                    if (filterElement == null) return;
                    view.SetFilterVisibility(filterElement.Id, true);
                    AssignFilterOverrides(doc, view, filterElement, view.GetFilterOverrides(filterElement.Id));
                    // MessageBox.Show("Filter Added Successfully.");
                }
                t.Commit();
            }
        }

        private static void AssignFilterOverrides(Document doc, Autodesk.Revit.DB.View view, ParameterFilterElement filter, OverrideGraphicSettings OldSettings)
        {
            var dialog = new AplayFilterVisibilitySettings(doc, OldSettings);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                view.SetFilterOverrides(filter.Id, dialog.CreateFilterOverride());
            }
        }
    }
}
