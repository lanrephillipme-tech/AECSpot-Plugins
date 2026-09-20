using AECSpot;
using AECSpot.QuickFilter;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;

namespace FilterManager
{
    [Transaction(TransactionMode.Manual)]
    public class QuickFilterCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData == null || commandData.Application == null || commandData.Application.ActiveUIDocument == null)
            {
                TaskDialog.Show("Quick Filter", "Open a project document before using Quick Filter.");
                return Result.Cancelled;
            }

            var uiDocument = commandData.Application.ActiveUIDocument;
            var document = uiDocument.Document;
            if (document == null || document.IsFamilyDocument)
            {
                TaskDialog.Show("Quick Filter", "Quick Filter is available only in project documents.");
                return Result.Cancelled;
            }

            try
            {
                var window = new QuickFilterWindow(document);
                if (window.ShowDialog() != true) return Result.Cancelled;
                var temporaryViews = new QuickFilterTemporaryViewService();
                if (window.ApprovedAction == QuickFilterAction.ResetTemporaryOverrides)
                {
                    TaskDialog.Show("Quick Filter", temporaryViews.ResetOverrides(document, uiDocument.ActiveView)
                        ? "Quick Filter's saved element overrides were restored for this view."
                        : "There are no Quick Filter temporary overrides to reset for this view in this Revit session.");
                    return Result.Succeeded;
                }
                if (window.ApprovedAction == QuickFilterAction.SelectCurrentView || window.ApprovedAction == QuickFilterAction.SelectEntireProject)
                {
                    var count = new QuickFilterSelectionService().Select(uiDocument, window.ApprovedPlan, window.ApprovedAction == QuickFilterAction.SelectCurrentView);
                    TaskDialog.Show("Quick Filter", count == 0 ? "No matching elements were found." : count + " matching element" + (count == 1 ? " was" : "s were") + " selected.");
                    return Result.Succeeded;
                }

                if (window.ApprovedAction == QuickFilterAction.TemporaryIsolate)
                {
                    var ids = new QuickFilterSelectionService().FindMatches(document, uiDocument.ActiveView, window.ApprovedPlan, true);
                    temporaryViews.Isolate(document, uiDocument.ActiveView, ids);
                    return Result.Succeeded;
                }

                if (window.ApprovedAction == QuickFilterAction.TemporaryOverride)
                {
                    var overrideDialog = new AplayFilterVisibilitySettings(document, new OverrideGraphicSettings());
                    if (overrideDialog.ShowDialog() != true) return Result.Cancelled;
                    var ids = new QuickFilterSelectionService().FindMatches(document, uiDocument.ActiveView, window.ApprovedPlan, true);
                    if (ids.Count == 0)
                    {
                        TaskDialog.Show("Quick Filter", "No matching elements were found in the active view.");
                        return Result.Succeeded;
                    }
                    temporaryViews.ApplyOverrides(document, uiDocument.ActiveView, ids, overrideDialog.CreateFilterOverride());
                    TaskDialog.Show("Quick Filter", "Temporary element overrides were applied. Reopen Quick Filter and choose Reset Overrides before closing Revit if you want to restore the saved state.");
                    return Result.Succeeded;
                }

                if (window.ApprovedAction == QuickFilterAction.SaveToProject)
                {
                    if (document.IsReadOnly)
                    {
                        TaskDialog.Show("Quick Filter", "This document is read-only, so the filter cannot be saved.");
                        return Result.Cancelled;
                    }
                    var name = window.SaveName.Trim();
                    if (new FilteredElementCollector(document).OfClass(typeof(ParameterFilterElement)).Cast<ParameterFilterElement>().Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
                    {
                        TaskDialog.Show("Quick Filter", "A parameter filter with that name already exists.");
                        return Result.Cancelled;
                    }
                    using (var transaction = new Transaction(document, "Save Quick Filter"))
                    {
                        transaction.Start();
                        ParameterFilterElement.Create(document, name, window.ApprovedPlan.CategoryIds, window.ApprovedPlan.ElementFilter);
                        transaction.Commit();
                    }
                    TaskDialog.Show("Quick Filter", "The filter was saved to the project. It has not been applied to any view.");
                    return Result.Succeeded;
                }
                return Result.Cancelled;
            }
            catch (Exception)
            {
                message = "Quick Filter could not complete the requested operation. The document was not changed unless a save transaction was committed.";
                return Result.Failed;
            }
        }
    }
}
