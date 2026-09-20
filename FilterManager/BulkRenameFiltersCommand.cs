using AECSpot;
using AECSpot.FilterCatalog;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace FilterManager
{
    [Transaction(TransactionMode.Manual)]
    public class BulkRenameFiltersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null || commandData.Application == null || commandData.Application.ActiveUIDocument == null)
            {
                TaskDialog.Show("Bulk Rename Filters", "Open a project document before using Bulk Rename Filters.");
                return Result.Cancelled;
            }

            var document = commandData.Application.ActiveUIDocument.Document;
            if (document == null || document.IsFamilyDocument || document.IsReadOnly)
            {
                TaskDialog.Show("Bulk Rename Filters", "Bulk Rename Filters requires a writable project document.");
                return Result.Cancelled;
            }

            try
            {
                var catalog = new FilterCatalogService().Build(document);
                var window = new BulkRenameFiltersWindow(catalog);
                if (window.ShowDialog() != true)
                {
                    return Result.Cancelled;
                }

                new FilterRenameExecutor().Execute(document, window.ApprovedPlan);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                message = "Bulk Rename Filters could not apply the requested changes. No partial rename should be retained.";
                return Result.Failed;
            }
        }
    }
}
