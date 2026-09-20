using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AECSpot.ViewModel
{
    public class ApplayFilterOverrideViewModel
    {
        public string Lines_SelectedLinePattern { get; set; }
        public string Lines_SelectedLineWeight { get; set; }
        public Autodesk.Revit.DB.Color Lines_SelectedLineColor { get; set; }
        public Autodesk.Revit.DB.Color Pattern_Foreground_SelectedLineColor { get; set; }
        public Autodesk.Revit.DB.Color Pattern_Background_SelectedLineColor { get; set; }
        public double TransparencyValue { get; set; }
        public bool Visibility { get; set; }
        public bool EnableFilter { get; set; }
        public bool HalfTone { get; set; }
    }
}
