using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace AECSpot
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class CopyFilterCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Current Application Session

            //  Assembly.Load("Xceed.Wpf.Toolkit, PublicKeyToken=3e4669d2f30244f4");

            ///geting the current  ui document
            UIDocument _uiDoc = commandData.Application.ActiveUIDocument;
            ///geting the current document
            Document _currentDoc = commandData.Application.ActiveUIDocument.Document;
            var filters = GetViewParameterFilters(_currentDoc);
            if (filters == null)
            {
                message = "Internal Error Try again later, please.";
                return Result.Failed;
            }
            new CopyFilter(_currentDoc, filters).ShowDialog();
            #endregion Current Application Session
            /*CreateFilter(_currentDoc, );*/
            return Result.Succeeded;
        }

        private List<ParameterFilterElement> GetViewParameterFilters(Document _currentDoc)
        {
            try
            {
                var filterList = new List<ParameterFilterElement>();
                var view = _currentDoc.ActiveView;
                if (view == null) return filterList;
                var filters = _currentDoc.ActiveView.GetFilters();
                foreach (var filter in filters)
                {
                    if (_currentDoc.ActiveView.IsFilterApplied(filter))
                    {
                        var filterElement = _currentDoc.GetElement(filter);
                        if (filterElement.GetType() == typeof(ParameterFilterElement))
                        {
                            filterList.Add((ParameterFilterElement)filterElement);
                        }
    ;
                    }
                }
                return filterList;
            }
            catch (System.Exception)
            {

                return null;
            }

        }





    }
}
