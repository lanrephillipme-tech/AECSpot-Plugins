using AECSpot.QuickFilter;
using Autodesk.Revit.DB;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AECSpot
{
    public class QuickFilterViewModel : ViewModelBase
    {
        private readonly Document _document;
        private readonly QuickFilterBuilderService _builder;
        private string _status = "Select categories, load common parameters, then add one or more AND rules.";

        public QuickFilterViewModel(Document document)
        {
            _document = document;
            _builder = new QuickFilterBuilderService();
            Categories = new ObservableCollection<QuickFilterCategoryItem>(_builder.GetCategories(document));
            Parameters = new ObservableCollection<QuickFilterParameterItem>();
            Rules = new ObservableCollection<QuickFilterRuleDraft>();
            Operators = new ObservableCollection<string>(new[] { "Equals", "Not equals", "Contains", "Does not contain", "Begins with", "Ends with", "Greater than", "Greater than or equal", "Less than", "Less than or equal" });
        }

        public ObservableCollection<QuickFilterCategoryItem> Categories { get; }
        public ObservableCollection<QuickFilterParameterItem> Parameters { get; }
        public ObservableCollection<QuickFilterRuleDraft> Rules { get; }
        public ObservableCollection<string> Operators { get; }
        public QuickFilterAction ApprovedAction { get; private set; }
        public QuickFilterPlan ApprovedPlan { get; private set; }
        public string SaveName { get; set; }
        public string Status { get => _status; private set { _status = value; OnPropertyChanged(nameof(Status)); } }

        public void LoadParameters()
        {
            Parameters.Clear();
            var selected = Categories.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            if (selected.Count == 0) { Status = "Select at least one category before loading common parameters."; return; }
            foreach (var parameter in _builder.GetCommonParameters(_document, selected)) Parameters.Add(parameter);
            Status = Parameters.Count == 0 ? "No supported common string, integer, or numeric parameters were found." : "Choose a parameter and add an AND rule.";
        }

        public void AddRule()
        {
            if (Parameters.Count == 0) { Status = "Load common parameters before adding a rule."; return; }
            Rules.Add(new QuickFilterRuleDraft { Parameter = Parameters.First(), Operator = "Equals", Value = string.Empty });
        }

        public void RemoveRule(QuickFilterRuleDraft rule)
        {
            if (rule != null) Rules.Remove(rule);
        }

        public bool Approve(QuickFilterAction action)
        {
            var plan = _builder.BuildPlan(_document, Categories, Rules);
            if (!plan.IsValid) { Status = plan.ErrorMessage; return false; }
            if (action == QuickFilterAction.SaveToProject && string.IsNullOrWhiteSpace(SaveName)) { Status = "Enter a name before saving this filter to the project."; return false; }
            ApprovedPlan = plan;
            ApprovedAction = action;
            return true;
        }

        public void ApproveReset() { ApprovedAction = QuickFilterAction.ResetTemporaryOverrides; }
    }
}
