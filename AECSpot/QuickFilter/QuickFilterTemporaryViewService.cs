using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace AECSpot.QuickFilter
{
    /// <summary>Maintains restoration data only for this Revit process. No document data is persisted.</summary>
    public class QuickFilterTemporaryViewService
    {
        private static readonly Dictionary<string, Dictionary<long, OverrideGraphicSettings>> OverrideSnapshots = new Dictionary<string, Dictionary<long, OverrideGraphicSettings>>();

        public void ApplyOverrides(Document document, View view, IList<ElementId> ids, OverrideGraphicSettings settings)
        {
            if (view == null || view.IsTemplate) throw new InvalidOperationException("Temporary overrides require a non-template active view.");
            var key = GetKey(document, view);
            Dictionary<long, OverrideGraphicSettings> snapshot;
            if (!OverrideSnapshots.TryGetValue(key, out snapshot))
            {
                snapshot = new Dictionary<long, OverrideGraphicSettings>();
                OverrideSnapshots[key] = snapshot;
            }
            foreach (var id in ids)
                if (!snapshot.ContainsKey(id.Value)) snapshot[id.Value] = view.GetElementOverrides(id);
            using (var transaction = new Transaction(document, "Quick Filter Temporary Overrides"))
            {
                transaction.Start();
                foreach (var id in ids) view.SetElementOverrides(id, settings);
                transaction.Commit();
            }
        }

        public bool ResetOverrides(Document document, View view)
        {
            Dictionary<long, OverrideGraphicSettings> snapshot;
            if (!OverrideSnapshots.TryGetValue(GetKey(document, view), out snapshot)) return false;
            using (var transaction = new Transaction(document, "Reset Quick Filter Temporary Overrides"))
            {
                transaction.Start();
                foreach (var pair in snapshot) view.SetElementOverrides(new ElementId(pair.Key), pair.Value);
                transaction.Commit();
            }
            OverrideSnapshots.Remove(GetKey(document, view));
            return true;
        }

        public void Isolate(Document document, View view, IList<ElementId> ids)
        {
            if (view == null || view.IsTemplate || ids.Count == 0) throw new InvalidOperationException("Temporary isolation requires matching elements in a non-template active view.");
            using (var transaction = new Transaction(document, "Quick Filter Temporary Isolate"))
            {
                transaction.Start();
                view.IsolateElementsTemporary(ids);
                transaction.Commit();
            }
        }

        private static string GetKey(Document document, View view) { return document.GetHashCode() + ":" + view.Id.Value; }
    }
}
