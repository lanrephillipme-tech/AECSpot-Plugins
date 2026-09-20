using Autodesk.Revit.DB;
using System;
using System.Linq;

namespace AECSpot.FilterCatalog
{
    /// <summary>
    /// Applies a previously validated plan in one named Revit transaction.
    /// </summary>
    public class FilterRenameExecutor
    {
        public void Execute(Document document, FilterRenamePlan plan)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (plan == null || !plan.IsValid)
            {
                throw new InvalidOperationException("The rename plan is not valid.");
            }

            var changes = plan.Items.Where(item => item.IsChange).ToList();
            using (var transaction = new Transaction(document, "Bulk Rename Filters"))
            {
                transaction.Start();
                foreach (var change in changes)
                {
                    var filter = document.GetElement(change.Filter.FilterId) as ParameterFilterElement;
                    if (filter == null)
                    {
                        throw new InvalidOperationException("A selected filter is no longer available in this project.");
                    }

                    filter.Name = change.ProposedName;
                }

                transaction.Commit();
            }
        }
    }
}
