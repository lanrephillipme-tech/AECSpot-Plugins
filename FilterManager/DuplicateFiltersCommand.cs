using AECSpot;
using AECSpot.FilterCatalog;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace FilterManager
{
    /// <summary>
    /// Read-only analysis of equivalent ParameterFilterElement definitions.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class DuplicateFiltersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null || commandData.Application == null || commandData.Application.ActiveUIDocument == null)
            {
                TaskDialog.Show("Duplicate Filters", "Open a project document before using Duplicate Filters.");
                return Result.Cancelled;
            }

            var document = commandData.Application.ActiveUIDocument.Document;
            if (document == null || document.IsFamilyDocument)
            {
                TaskDialog.Show("Duplicate Filters", "Duplicate Filters is available only in project documents.");
                return Result.Cancelled;
            }

            try
            {
                var catalog = new FilterCatalogService().Build(document);
                var analysis = new FilterDefinitionComparer().Analyze(catalog);
                new DuplicateFiltersWindow(analysis, catalog.Count > 0).ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception)
            {
                message = "Duplicate Filters could not analyze filter definitions for this project.";
                return Result.Failed;
            }
        }
    }
}




