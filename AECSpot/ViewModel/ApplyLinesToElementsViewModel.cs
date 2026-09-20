using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ElectricalSystemCircuits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AECSpot
{
    public class ApplyLinesToElementsViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// the line apply view
        /// </summary>
        public Window MainView { get; set; }

        /// <summary>
        /// name of the line style that is created to act like invisible line
        /// </summary>
        public const string InvisbleLineStyleName = "Background Colour";

        public const string DefaultCategoryStyleName = "<By Category>";

        /// <summary>
        /// the current revit session background color used in <see cref="InvisbleLineStyleName" />
        /// as color
        /// </summary>
        public Color GetUserRevitSessionColor { get; set; }

        /// <summary>
        /// the active revit application
        /// </summary>
        public UIApplication Application { get; set; }

        /// <summary>
        /// the active document
        /// </summary>
        public Document _document { get; set; }

        /// <summary>
        /// names of all lines style in the active document
        /// </summary>
        public List<string> DocumentLinesStylesNames = new List<string>();

        /// <summary>
        /// List of lines style in the active document
        /// </summary>
        public List<Category> DocumentLinesStyles { get; set; }

        /// <summary>
        /// id of the user select elements to apply the lines style to then
        /// </summary>

        public List<ElementId> _selectedElementsId = new List<ElementId>();

        /*       /// <summary>
               /// References of the user select elements to apply the lines style to then </summary>
               public IList<Reference> References = new List<Reference>();*/

        #region Notify PropertyChange props

        private string _UserSelectedLineStyle;

        /// <summary>
        /// the selected line style name that the user select from dropdwon menu
        /// </summary>
        public string UserSelectedLineStyle
        {
            get { return _UserSelectedLineStyle; }
            set
            {
                _UserSelectedLineStyle = value;
                OnPropertyChanged(nameof(UserSelectedLineStyle));
            }
        }

        private int _DropDownSlectedIndex;

        /// <summary>
        /// index of selected element in the drop dwon menu
        /// </summary>
        public int DropDownSlectedIndex
        {
            get { return 0; }
            set
            {
                _DropDownSlectedIndex = value;
                OnPropertyChanged(nameof(DropDownSlectedIndex));
            }
        }

        public ObservableCollection<string> LinesStyleNames => GetLinesStyle();

        #endregion Notify PropertyChange props

        #endregion Properties

        #region Commands

        public ICommand ApplyLineStyleCommand { get; set; }

        #endregion Commands

        public ApplyLinesToElementsViewModel(UIApplication uIApplication)
        {
            if (InjectRefrences(uIApplication)) ShowWindow();
        }

        /// <summary>
        /// show lines style window
        /// </summary>
        private void ShowWindow()
        {
            if (_selectedElementsId.Count > 0)
            {
                MainView = new ApplyLinesToElementsView(this);
                MainView.ShowDialog();
            }
        }

        /// <summary>
        /// Dependency injection
        /// </summary>
        /// <param name="uIApplication"> </param>
        private bool InjectRefrences(UIApplication uIApplication)
        {
            Application = uIApplication;
            _document = Application.ActiveUIDocument.Document;
            var ellmentsRefrences = GetSelected(uIApplication);
            if (false == ellmentsRefrences) return false;
            GetUserRevitSessionColor = uIApplication.Application.BackgroundColor;
            ApplyLineStyleCommand = new ApplyLineStyleCommand(this);
            return true;
        }

        /// <summary>
        /// Select Revit elements to Run the operation Throw
        /// </summary>
        /// <param name="Application"> </param>
        public bool GetSelected(UIApplication Application)
        {
            try
            {
                var selectedelements = Application.ActiveUIDocument.Selection;
                var elements = selectedelements.GetElementIds();

             /*   foreach (var elementID in elements)
                {
                    selectedelements.SetElementIds(new List<ElementId>() { elementID });
                }*/
                if (elements.Count > 0)
                {
                    _selectedElementsId = elements.ToList();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// get elements id from list of regrences
        /// </summary>
        /// <param name="selectedElements"> </param>
        /// <returns> </returns>
        public List<ElementId> GetSelectedElementsId(IList<Reference> selectedElements)
        {
            if (selectedElements.Count > 0)
            {
                var elementIds = new List<ElementId>();
                foreach (var item in selectedElements)
                {
                    elementIds.Add(item.ElementId);
                }
                return elementIds;
            }
            else
            {
                return new List<ElementId>();
            }
        }

        /// <summary>
        /// get all lines style names in the active document
        /// </summary>
        /// <returns> </returns>
        public ObservableCollection<string> GetLinesStyle()
        {
            DocumentLinesStyles = GetListOfLinestyles(_document);
            DocumentLinesStyles.ForEach(el => DocumentLinesStylesNames.Add(el.Name));
            List<string> UserLinesValue = DocumentLinesStylesNames;
            UserLinesValue.Insert(0, InvisbleLineStyleName);
            UserLinesValue.Insert(0, DefaultCategoryStyleName);
            UserLinesValue.Sort();
            ObservableCollection<string> LinesStyle = new ObservableCollection<string>();
            UserLinesValue.ForEach(l => LinesStyle.Add(l));
            this.DropDownSlectedIndex = 0;
            OnPropertyChanged(nameof(DropDownSlectedIndex));
            ((ApplyLinesToElementsView)MainView).SelectedLineStyleDropDown.SelectedIndex = this.DropDownSlectedIndex;
            return LinesStyle;
        }

        /// <summary>
        /// Apply the line style to the selected object
        /// </summary>
        public void ConfirmApllyStyle()
        {
            using (Transaction trans = new Transaction(_document))
            {
                trans.Start("Change Lines Style");

                if (UserSelectedLineStyle == DefaultCategoryStyleName)
                {
                    _selectedElementsId.ForEach(ele => ResetOverridOfElement(_document, ele));
                }
                else if (UserSelectedLineStyle == InvisbleLineStyleName)
                {
                    _selectedElementsId.ForEach(ele => AsseginInvisibleStyl(_document, ele));
                }
                else
                {
                    try
                    {
                        var linestyle = UserSelectedLineStyle;
                        var selectedLineStyle = DocumentLinesStyles.Where(el => el.Name == linestyle).FirstOrDefault();
                        var getGraphicStyle = selectedLineStyle?.GetGraphicsStyle(GraphicsStyleType.Projection);
                        if (null != getGraphicStyle)
                        {
                            _selectedElementsId.ForEach(ele => AssginOverrideGraphicSettings(_document, ele, getGraphicStyle));
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }

                trans.Commit();
                MainView.Close();
            }
        }

        /// <summary>
        /// ovveride element lines
        /// </summary>
        /// <param name="document">          </param>
        /// <param name="elementToOverride"> </param>
        /// <param name="graphicsStyle">     </param>
        public void AssginOverrideGraphicSettings(Document document, ElementId elementToOverride, GraphicsStyle graphicsStyle)
        {
            if (null != elementToOverride)
            {
                #region OverrideSettings

                OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
                if (null == graphicsStyle)
                {
                    overrideGraphicSettings.SetProjectionLinePatternId(CreateLinePatternElement(InvisbleLineStyleName).Id);
                    overrideGraphicSettings.SetProjectionLineColor(GetUserRevitSessionColor);
                    overrideGraphicSettings.SetProjectionLineWeight(OverrideGraphicSettings.InvalidPenNumber);
                }
                else
                {
                    var style = graphicsStyle.GraphicsStyleCategory;
                    overrideGraphicSettings.SetProjectionLinePatternId(style.GetLinePatternId(GraphicsStyleType.Projection));
                    overrideGraphicSettings.SetProjectionLineColor(style.LineColor);
                    overrideGraphicSettings.SetProjectionLineWeight(style.GetLineWeight(GraphicsStyleType.Projection).GetValueOrDefault());
                }

                #endregion OverrideSettings

                ///assgin the color to the object
                document.ActiveView.SetElementOverrides(elementToOverride, overrideGraphicSettings);
            }
        }

        public void AsseginInvisibleStyl(Document document, ElementId elementToOverride)
        {
            if (null != elementToOverride)
            {
                #region OverrideSettings

                OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
                overrideGraphicSettings.SetProjectionLinePatternId(CreateLinePatternElement(InvisbleLineStyleName).Id);
                overrideGraphicSettings.SetProjectionLineColor(GetUserRevitSessionColor);
                overrideGraphicSettings.SetProjectionLineWeight(OverrideGraphicSettings.InvalidPenNumber);

                #endregion OverrideSettings

                ///assgin the color to the object
                document.ActiveView.SetElementOverrides(elementToOverride, overrideGraphicSettings);
            }
        }

        public void ResetOverridOfElement(Document document, ElementId elementToOverride)
        {
            if (null != elementToOverride)
            {
                #region OverrideSettings

                OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();

                #endregion OverrideSettings

                ///assgin the color to the object
                document.ActiveView.SetElementOverrides(elementToOverride, overrideGraphicSettings);
            }
        }

        /// <summary>
        ///get the invisible line style in the active document
        /// </summary>
        /// <returns></returns>
        public GraphicsStyle GetInvisibleLine()
        {
            FilteredElementCollector ElectricalSysteys = new FilteredElementCollector(_document);
            GraphicsStyle ElectricalSysteyslist = ElectricalSysteys.OfClass(typeof(GraphicsStyle)).Where(ele => ele.Name == "<Invisible lines>").Cast<GraphicsStyle>().FirstOrDefault();
            return ElectricalSysteyslist;
        }

        /// <summary>
        /// get all lines styles in the active document
        /// </summary>
        /// <param name="doc"> </param>
        /// <returns> </returns>
        public List<Category> GetListOfLinestyles(Document doc)
        {
            Category c = doc.Settings.Categories.get_Item(
              BuiltInCategory.OST_Lines);

            CategoryNameMap subcats = c.SubCategories;
            List<Category> LinesStyle = new List<Category>();
            foreach (Category lineStyle in subcats)
            {
                LinesStyle.Add(lineStyle);
            }
            return LinesStyle.OrderBy(l => l.Name).ToList();
        }

        /// <summary>
        /// Creat new pattenr for element
        /// </summary>
        /// <param name="patternName"> </param>
        /// <returns> </returns>
        public LinePatternElement CreateLinePatternElement(string patternName)
        {
            var collector = new FilteredElementCollector(_document);
            LinePatternElement pattern = collector.OfClass(typeof(LinePatternElement)).Where(e => e.Name == InvisbleLineStyleName)?.Cast<LinePatternElement>()?.FirstOrDefault();
            if (pattern == null)
            {
                //Create list of segments which define the line pattern
                List<LinePatternSegment> lstSegments = new List<LinePatternSegment>();
                lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dot, 0.0));
                lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 5));
                LinePattern linePattern = new LinePattern(patternName);
                linePattern.SetSegments(lstSegments);
                SubTransaction trans = new SubTransaction(_document);
                trans.Start();
                LinePatternElement linePatternElement = LinePatternElement.Create(_document, linePattern);
                trans.Commit();
                return linePatternElement;
            }
            return pattern;
        }
    }
}
