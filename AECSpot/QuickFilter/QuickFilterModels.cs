using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace AECSpot.QuickFilter
{
    public enum QuickFilterAction { None, SelectCurrentView, SelectEntireProject, TemporaryOverride, TemporaryIsolate, ResetTemporaryOverrides, SaveToProject }

    public class QuickFilterCategoryItem
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class QuickFilterParameterItem
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public StorageType StorageType { get; set; }
        public ForgeTypeId DataTypeId { get; set; }
        public string DisplayName => Name + " (" + StorageType + ")";
    }

    public class QuickFilterRuleDraft
    {
        public QuickFilterParameterItem Parameter { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class QuickFilterPlan
    {
        public IList<ElementId> CategoryIds { get; set; }
        public ElementFilter ElementFilter { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsValid => string.IsNullOrWhiteSpace(ErrorMessage) && ElementFilter != null && CategoryIds != null && CategoryIds.Count > 0;
    }
}
