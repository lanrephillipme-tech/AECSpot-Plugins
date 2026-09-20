using AECSpot;
using AECSpot.FilterCatalog;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace FilterManager
{
    [Transaction(TransactionMode.Manual)]
    public class ReplaceFiltersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null || commandData.Application == null || commandData.Application.ActiveUIDocument == null)
            {
                TaskDialog.Show("Replace Filters", "Open a project document before using Replace Filters.");
                return Result.Cancelled;
            }

            var document = commandData.Application.ActiveUIDocument.Document;
            if (document == null || document.IsFamilyDocument || document.IsReadOnly)
            {
                TaskDialog.Show("Replace Filters", "Replace Filters requires a writable project document.");
                return Result.Cancelled;
            }

            try
            {
                var catalog = new FilterCatalogService().Build(document);
                var window = new ReplaceFiltersWindow(catalog);
                if (window.ShowDialog() != true)
                {
                    return Result.Cancelled;
                }

                new FilterReplacementExecutor().Execute(document, window.ApprovedPlan);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                message = "Replace Filters could not apply the requested changes. No partial replacement should be retained.";
                return Result.Failed;
            }
        }
    }
}
