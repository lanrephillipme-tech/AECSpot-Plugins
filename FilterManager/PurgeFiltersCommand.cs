using AECSpot;
using AECSpot.FilterCatalog;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace FilterManager
{
    [Transaction(TransactionMode.Manual)]
    public class PurgeFiltersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null || commandData.Application == null || commandData.Application.ActiveUIDocument == null)
            {
                TaskDialog.Show("Purge Unused Filters", "Open a project document before using Purge Unused Filters.");
                return Result.Cancelled;
            }

            var document = commandData.Application.ActiveUIDocument.Document;
            if (document == null || document.IsFamilyDocument || document.IsReadOnly)
            {
                TaskDialog.Show("Purge Unused Filters", "Purge Unused Filters requires a writable project document.");
                return Result.Cancelled;
            }

            try
            {
                var catalog = new FilterCatalogService().Build(document);
                var window = new PurgeFiltersWindow(catalog);
                if (window.ShowDialog() != true)
                {
                    return Result.Cancelled;
                }

                new FilterPurgeExecutor().Execute(document, window.ApprovedPlan);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                message = "Purge Unused Filters could not complete. No partial deletion should be retained.";
                return Result.Failed;
            }
        }
    }
}
