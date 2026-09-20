using Autodesk.Revit.DB;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Read-only snapshot of a single filter assignment on a Revit view.
    /// </summary>
    public class FilterUsageInfo
    {
        public ElementId ViewId { get; set; }
        public string ViewName { get; set; }
        public string ViewTypeName { get; set; }
        public bool IsTemplate { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? HasGraphicOverrides { get; set; }
    }
}
