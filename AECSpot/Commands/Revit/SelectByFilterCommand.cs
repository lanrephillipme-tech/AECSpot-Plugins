using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AECSpot
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class SelectByFilterCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Current Application Session

            ///geting the current  ui document
            UIDocument _uiDoc = commandData.Application.ActiveUIDocument;
            ///geting the current document
            Document _currentDoc = commandData.Application.ActiveUIDocument.Document;
            var dataList = new FilteredElementCollector(_currentDoc).OfClass(typeof(ParameterFilterElement)).Cast<ParameterFilterElement>().ToList();
            SelectElements(_uiDoc, dataList);
            #endregion Current Application Session
            /*CreateFilter(_currentDoc, );*/
            return Result.Succeeded;
        }
        private bool SelectElements(UIDocument _uiDoc, List<ParameterFilterElement> dataList)
        {
            var view = new SelectElementsByFilter(dataList);
            if (view.ShowDialog() != true) return false;
            if (view.SelectedFilter == null)
            {
                MessageBox.Show("No Filter Selected");
                return false;
            }
            var selectionList = new List<ElementId>();
            //var categries2 = view.SelectedFilter.GetElementFilterParameters();
            List<ElementId> categories = new List<ElementId>();
            view.SelectedFilters.ForEach(s => categories.AddRange(s.GetCategories().ToList()));
            if (view.IsAllViews.IsChecked == true)
            {
                foreach (var category in categories)
                {
                    foreach (var item in view.SelectedFilters)
                    {
#if REVIT2018
                        ElementMulticategoryFilter catfilter = new ElementMulticategoryFilter(item.GetCategories());
                        IEnumerable<Element> ElemsByFilter = new FilteredElementCollector(_uiDoc.Document)
                            .WhereElementIsNotElementType()
                            .WherePasses(catfilter);
                        foreach (FilterRule rule in item.GetRules())
                        {
                            selectionList.AddRange(ElemsByFilter.Where(e => rule.ElementPasses(e)).Select(s => s.Id).ToList());
                        }
#else
                        selectionList.AddRange(CreateFilter(_uiDoc.Document, category, item.GetElementFilter())?.Select(E => E.Id)?.ToList());
#endif
                    }
                }
            }
            else if (view.IsCurrentView.IsChecked == true)
            {
                foreach (var category in categories)
                {
                    foreach (var item in view.SelectedFilters)
                    {
#if REVIT2018

                        ElementMulticategoryFilter catfilter = new ElementMulticategoryFilter(item.GetCategories());
                        IEnumerable<Element> ElemsByFilter = new FilteredElementCollector(_uiDoc.Document, _uiDoc.Document.ActiveView.Id)
                            .WhereElementIsNotElementType()
                            .WherePasses(catfilter);
                        foreach (FilterRule rule in item.GetRules())
                        {
                            selectionList.AddRange(  ElemsByFilter.Where(e => rule.ElementPasses(e)).Select(s=>s.Id).ToList());
                        }

                       

#else
                        selectionList.AddRange(CreateActiveViewFilter(_uiDoc.Document, category, item.GetElementFilter())?.Select(E => E.Id)?.ToList());
#endif
                    }
                }
            }
            if (selectionList != null && selectionList?.Count != 0)
            {
                _uiDoc.Selection.SetElementIds(selectionList);
                return true;
            }
            return false;
        }
        public List<Element> CreateFilter(Document doc, ElementId category, ElementFilter filter)
        {
            if (filter != null)
            {

                return new FilteredElementCollector(doc).OfCategoryId(category).WhereElementIsNotElementType().WherePasses(filter).ToList();
            }
            else
            {
                return new List<Element>();
            }
        }
        public List<Element> CreateActiveViewFilter(Document doc, ElementId category, ElementFilter filter)
        {
            var activeview = doc.ActiveView;
            if (activeview == null)
            {
                MessageBox.Show("Please Select Valid View.");
                return null;
            }
            if (filter != null)
            {

                return new FilteredElementCollector(doc, activeview.Id).OfCategoryId(category).WhereElementIsNotElementType().WherePasses(filter).ToList();
            }
            else
            {
                return new List<Element>();
            }
        }
    }
}