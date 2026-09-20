using AECSpot;
using AECSpot.FilterCatalog;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace FilterManager
{
    /// <summary>
    /// Displays a read-only catalog of rule-based project filters and their direct view usage.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class FindFiltersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null || commandData.Application == null || commandData.Application.ActiveUIDocument == null)
            {
                TaskDialog.Show("Find Filters", "Open a project document before using Find Filters.");
                return Result.Cancelled;
            }

            var document = commandData.Application.ActiveUIDocument.Document;
            if (document == null || document.IsFamilyDocument)
            {
                TaskDialog.Show("Find Filters", "Find Filters is available only in project documents.");
                return Result.Cancelled;
            }

            try
            {
                var catalog = new FilterCatalogService().Build(document);
                new FindFiltersWindow(catalog).ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception)
            {
                message = "Find Filters could not read the filter catalog for this project.";
                return Result.Failed;
            }
        }
    }
}




