using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AECSpot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Color = Autodesk.Revit.DB.Color;
using Document = Autodesk.Revit.DB.Document;
using Line = Autodesk.Revit.DB.Line;

namespace ElectricalSystemCircuits
{
    /// <summary>
    /// Interaction logic for ApplyLinesToElementsView.xaml
    /// </summary>
    public partial class ApplyLinesToElementsView : Window
    {

        public ApplyLinesToElementsView(ApplyLinesToElementsViewModel ApplyLinesToElementsViewModel)
        {
            InitializeComponent();
            this.DataContext = ApplyLinesToElementsViewModel;
        }
    }
}