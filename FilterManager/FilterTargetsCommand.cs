using AECSpot;
using AECSpot.FilterCatalog;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace FilterManager
{
    [Transaction(TransactionMode.Manual)]
    public class FilterTargetsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null || commandData.Application == null || commandData.Application.ActiveUIDocument == null)
            {
                TaskDialog.Show("Filter Targets", "Open a project document before using Filter Targets.");
                return Result.Cancelled;
            }

            var document = commandData.Application.ActiveUIDocument.Document;
            if (document == null || document.IsFamilyDocument)
            {
                TaskDialog.Show("Filter Targets", "Filter Targets is available only in project documents.");
                return Result.Cancelled;
            }

            try
            {
                var catalog = new FilterCatalogService().Build(document);
                new FilterTargetsWindow(document, catalog).ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception)
            {
                message = "Filter Targets could not read the filter catalog for this project.";
                return Result.Failed;
            }
        }
    }
}
