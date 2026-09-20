using System.Collections.Generic;

namespace AECSpot.FilterCatalog
{
    public class FilterTargetElementInfo
    {
        public string ElementId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
    }

    public class FilterTargetQueryResult
    {
        public int MatchCount { get; set; }
        public List<FilterTargetElementInfo> DisplayElements { get; set; } = new List<FilterTargetElementInfo>();
        public bool IsTruncated { get; set; }
        public string ErrorMessage { get; set; }
    }
}
