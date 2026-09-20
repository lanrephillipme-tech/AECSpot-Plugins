using System.Collections.Generic;

namespace AECSpot.FilterCatalog
{
    public class FilterRenameRequest
    {
        public string FindText { get; set; }
        public string ReplaceText { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
    }

    public class FilterRenamePlanItem
    {
        public FilterCatalogItem Filter { get; set; }
        public string ProposedName { get; set; }
        public string ValidationMessage { get; set; }
        public bool IsValid => string.IsNullOrEmpty(ValidationMessage);
        public bool IsChange => !string.Equals(Filter.Name, ProposedName, System.StringComparison.Ordinal);
    }

    public class FilterRenamePlan
    {
        public List<FilterRenamePlanItem> Items { get; set; } = new List<FilterRenamePlanItem>();
        public List<string> Errors { get; set; } = new List<string>();
        public bool IsValid => Errors.Count == 0 && Items.TrueForAll(item => item.IsValid) && Items.Exists(item => item.IsChange);
    }
}
