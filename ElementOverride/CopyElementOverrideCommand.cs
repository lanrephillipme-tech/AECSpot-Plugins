using AECSpot;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ElementOverride
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class CopyElementOverrideCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Current Application Session
            ///geting the current  ui document
            UIDocument _uiDoc = commandData.Application.ActiveUIDocument;
            ///geting the current document
            Document _currentDoc = commandData.Application.ActiveUIDocument.Document;
            var selectedElements = _uiDoc.Selection.GetElementIds();
            if (selectedElements.Count == 0)
            {
                selectedElements = new List<ElementId>();
                try
                {
                    var selectedElementRefrence = _uiDoc.Selection.PickObjects(ObjectType.Element, "Select Objects To Apply Override To");
                    if (selectedElementRefrence == null || selectedElementRefrence?.Count == 0) return Result.Succeeded;
                    foreach (var reference in selectedElementRefrence)
                    {
                        selectedElements.Add(reference.ElementId);
                    }
                    
                }
                catch
                {

                    message = "Operation Aborted.";
                    return Result.Succeeded;
                }
            }
            try
            {


#if REVIT2025
 //if (MessageBox.Show("Select an element to copy Graphic Overrides from.", "Selection", MessageBoxButton.OK) == MessageBoxResult.OK)
#else

                //if (System.Windows.Forms.MessageBox.Show("Select an element to copy Graphic Overrides from.", "Selection", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
#endif
                {
                    try
                    {
                        var obj = _uiDoc.Selection.PickObject(ObjectType.Element, "Select an element to copy Graphic Overrides from.");
                        if (obj == null)
                        {
                            message = "Operation Aborted.";
                            return Result.Succeeded;
                        }
                        var elementOverrides = _uiDoc.Document.ActiveView.GetElementOverrides(obj.ElementId);
                        using (Transaction transaction = new Transaction(_uiDoc.Document, "Copy Element Overrides"))
                        {
                            transaction.Start();
                            foreach (var item in selectedElements)
                            {
                                _uiDoc.Document.ActiveView.SetElementOverrides(item, elementOverrides);
                            }
                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        message = "Operation Aborted.";
                        return Result.Succeeded;
                    }
                }
            }
            catch (Exception ee)
            {
            }
            //var elementOverrides = new OverrideGraphicSettings();

            #endregion Current Application Session

            return Result.Succeeded;
        }

      



    }
}