using AECSpot.Model;
using AECSpot.Views;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;


namespace AECSpot
{
    /// <summary>
    /// Interaction logic for AplayFilterVisibilitySettings.xaml
    /// </summary>
    public partial class TransferElementsGraphics : Window
    {
        public const string NoOverrideValueName = "<No Override>";
        public const string SolidName = "Solid";
        public List<LinePatternElement> LinePatternElements { get; set; }
        public List<FillPatternElement> FillPatterns { get; set; }

        public UIDocument _uiDoc { get; set; }
        public string searchWord = string.Empty;
        public ObservableCollection<OverrideElementModel> OverrideElementModels { get; set; }
        //public ObservableCollection<OverrideElementModel> SearchOverrideElementModels { get; set; }
        public OverrideElementModel SelectedOverrideElementModels { get; set; }

        private static readonly string AppDataFolder = System.IO.Path.Combine(
           Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
           "AECSpot",  // Replace with your application name
           "Presets"
       );
        public TransferElementsGraphics(UIDocument _document, ICollection<ElementId> SelectedElements)
        {


            LinePatternElements = GetAllLinePatterns(_document.Document);
            FillPatterns = GetAllFillPatterns(_document.Document);
            var patterns = GetElementsNames(LinePatternElements);
            var fillPAterns = GetElementsNames(FillPatterns);
            this.SelectedElements = SelectedElements;
            InitializeComponent();
            FillItemsSources(patterns, fillPAterns);
            this.DataContext = this;
            _uiDoc = _document;
            _doc = _document.Document;
            GetElementsSelectedOverrids();
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }
            else
            {
                OverrideElementModels = new ObservableCollection<OverrideElementModel>();

                LoadPresets(AppDataFolder);
            }

            DeleteButton.IsEnabled = false;
            OverrideButton.IsEnabled = false;
            DuplicateButton.IsEnabled = false;
            RenameButton.IsEnabled = false;
        }

        private void LoadPresets(string appDataFolder)
        {
            string[] xmlFiles = Directory.GetFiles(appDataFolder, "*.xml");

            foreach (string file in xmlFiles)
            {
                var objec = (OverrideElementModel)SerializerManager.DeSerializationFromXML(new OverrideElementModel(), file);
                OverrideElementModels.Add(objec);

            }

        }

        private void FillItemsSources(List<string> patterns, List<string> fillPAterns)
        {
            Pattern_Background_Pattern.ItemsSource = fillPAterns;
            Pattern_ForeGround_Pattern.ItemsSource = fillPAterns;
            Lines_Pattern.ItemsSource = patterns;
            Lines_LineWieght.ItemsSource = GetAlLineWeight();
            CutPattern_Background_Pattern.ItemsSource = fillPAterns;
            CutPattern_ForeGround_Pattern.ItemsSource = fillPAterns;
            CutLines_Pattern.ItemsSource = patterns;
            CutLines_LineWieght.ItemsSource = GetAlLineWeight();
        }
        private void GetElementsSelectedOverrids()
        {
            OverrideGraphicSettings overrideGraphicSettings = null;
            foreach (var item in SelectedElements)
            {
                var overrideGraphic = _uiDoc.Document.ActiveView.GetElementOverrides(item); ;
                if (overrideGraphic == null) continue;
                if (overrideGraphicSettings == null)
                {
                    overrideGraphicSettings = overrideGraphic;
                    break;
                }
                ;
            }
            OldSettings = overrideGraphicSettings;
            if (OldSettings != null)
            {
#if REVIT2018
#else
                Lines_SelectedLinePattern = GetElementName(OldSettings.ProjectionLinePatternId);
                Lines_SelectedLineWeight = (OldSettings.ProjectionLineWeight).ToString();
                Pattern_Foreground_SelectedLinePattern = GetElementName(OldSettings.SurfaceForegroundPatternId);
                Pattern_Background_SelectedLinePattern = GetElementName(OldSettings.SurfaceBackgroundPatternId);
                BackgroundPatternVisible = (OldSettings.IsSurfaceBackgroundPatternVisible);
                ForegroundPatternVisible = (OldSettings.IsSurfaceForegroundPatternVisible);
                CutLines_SelectedLinePattern = GetElementName(OldSettings.CutLinePatternId);
                CutLines_SelectedLineWeight = (OldSettings.CutLineWeight).ToString();
                CutPattern_Foreground_SelectedLinePattern = GetElementName(OldSettings.CutForegroundPatternId);
                CutPattern_Background_SelectedLinePattern = GetElementName(OldSettings.CutBackgroundPatternId);
                CutBackgroundPatternVisible = (OldSettings.IsCutBackgroundPatternVisible);
                CutForegroundPatternVisible = (OldSettings.IsCutForegroundPatternVisible);
                TransparencyValue = OldSettings.Transparency;
                Halftone = OldSettings.Halftone;
                if (OldSettings.SurfaceBackgroundPatternColor.IsValid) PatterBackgroundColor.SelectedColor = Color.FromRgb(OldSettings.SurfaceBackgroundPatternColor.Red, OldSettings.SurfaceBackgroundPatternColor.Green, OldSettings.SurfaceBackgroundPatternColor.Blue);
                if (OldSettings.SurfaceForegroundPatternColor.IsValid) PatterForegroundColor.SelectedColor = Color.FromRgb(OldSettings.SurfaceForegroundPatternColor.Red, OldSettings.SurfaceForegroundPatternColor.Green, OldSettings.SurfaceForegroundPatternColor.Blue);
                if (OldSettings.ProjectionLineColor.IsValid) linePatternColor.SelectedColor = Color.FromRgb(OldSettings.ProjectionLineColor.Red, OldSettings.ProjectionLineColor.Green, OldSettings.ProjectionLineColor.Blue);
                if (OldSettings.CutBackgroundPatternColor.IsValid) CutPatterBackgroundColor.SelectedColor = Color.FromRgb(OldSettings.CutBackgroundPatternColor.Red, OldSettings.CutBackgroundPatternColor.Green, OldSettings.CutBackgroundPatternColor.Blue);
                if (OldSettings.CutForegroundPatternColor.IsValid) CutPatterForegroundColor.SelectedColor = Color.FromRgb(OldSettings.CutForegroundPatternColor.Red, OldSettings.CutForegroundPatternColor.Green, OldSettings.CutForegroundPatternColor.Blue);
                if (OldSettings.CutLineColor.IsValid) CutlinePatternColor.SelectedColor = Color.FromRgb(OldSettings.CutLineColor.Red, OldSettings.CutLineColor.Green, OldSettings.CutLineColor.Blue);
#endif
            }
            foreach (var item in SelectedElements)
            {
                var overrideGraphic = _uiDoc.Document.ActiveView.GetElementOverrides(item); ;
                if (overrideGraphic == null) continue;
                if (overrideGraphicSettings == null) overrideGraphicSettings = overrideGraphic;
                else
                {
#if REVIT2018
#else
                    var IsEqual = overrideGraphicSettings.Halftone == overrideGraphic.Halftone;
                    if (IsEqual == false)
                    {
                        halftonecheckbox.IsChecked = null;
                        Halftone = null;
                    }
                    var IsEqualIsCutBackgroundPatternVisible = overrideGraphicSettings.IsCutBackgroundPatternVisible == overrideGraphic.IsCutBackgroundPatternVisible;
                    if (IsEqualIsCutBackgroundPatternVisible == false)
                    {
                        CutbackgroundVisible.IsChecked = null;
                        CutBackgroundPatternVisible = null;
                    }
                    var IsEqualIsSurfaceBackgroundPatternVisible = overrideGraphicSettings.IsSurfaceBackgroundPatternVisible == overrideGraphic.IsSurfaceBackgroundPatternVisible;
                    if (IsEqualIsSurfaceBackgroundPatternVisible == false)
                    {
                        backgroundVisible.IsChecked = null;
                        BackgroundPatternVisible = null;
                    }
                    var IsEqualIsSurfaceForegroundPatternVisible = overrideGraphicSettings.IsSurfaceForegroundPatternVisible == overrideGraphic.IsSurfaceForegroundPatternVisible;
                    if (IsEqualIsSurfaceForegroundPatternVisible == false)
                    {
                        ForegroundVisible.IsChecked = null;
                        ForegroundPatternVisible = null;
                    }
                    var IsEqualIsCutForegroundPatternVisible = overrideGraphicSettings.IsCutForegroundPatternVisible == overrideGraphic.IsCutForegroundPatternVisible;
                    if (IsEqualIsCutForegroundPatternVisible == false)
                    {
                        CutForegroundVisible.IsChecked = null;
                        CutForegroundPatternVisible = null;
                    }
                    var IsEqualTransparency = overrideGraphicSettings.Transparency == overrideGraphic.Transparency;
                    if (IsEqualTransparency == false)
                    {
                        transparencyBar.Value = 0;
                        TransparencyValue = null;
                    }
                    var IsEqualCutBackgroundPatternColor = IsColorsEqual(overrideGraphicSettings.CutBackgroundPatternColor, overrideGraphic.CutBackgroundPatternColor);
                    if (IsEqualCutBackgroundPatternColor == false)
                    {
                        CutPatterBackgroundColor.SelectedColor = null;
                        CutPattern_Background_SelectedLineColor = null;
                    }
                    var IsEqualSurfaceBackgroundPatternColor = IsColorsEqual(overrideGraphicSettings.SurfaceBackgroundPatternColor, overrideGraphic.SurfaceBackgroundPatternColor);
                    if (IsEqualSurfaceBackgroundPatternColor == false)
                    {
                        PatterBackgroundColor.SelectedColor = null;
                        Pattern_Background_SelectedLineColor = null;
                    }
                    var IsEqualSurfaceForegroundPatternColor = IsColorsEqual(overrideGraphicSettings.SurfaceForegroundPatternColor, overrideGraphic.SurfaceForegroundPatternColor);
                    if (IsEqualSurfaceForegroundPatternColor == false)
                    {
                        PatterForegroundColor.SelectedColor = null;
                        Pattern_Foreground_SelectedLineColor = null;
                    }
                    var IsEqualCutForegroundPatternColor = IsColorsEqual(overrideGraphicSettings.CutForegroundPatternColor, overrideGraphic.CutForegroundPatternColor);
                    if (IsEqualCutForegroundPatternColor == false)
                    {
                        CutPatterForegroundColor.SelectedColor = null;
                        CutPattern_Foreground_SelectedLineColor = null;
                    }
                    var IsEqualCutLineColor = IsColorsEqual(overrideGraphicSettings.CutLineColor, overrideGraphic.CutLineColor);
                    if (IsEqualCutLineColor == false)
                    {
                        CutlinePatternColor.SelectedColor = null;
                        CutlinePatternColor_SelectedLineColor = null;
                    }
                    var IsEqualProjectionLineColor = IsColorsEqual(overrideGraphicSettings.ProjectionLineColor, overrideGraphic.ProjectionLineColor);
                    if (IsEqualProjectionLineColor == false)
                    {
                        linePatternColor.SelectedColor = null;
                        Lines_SelectedLineColor = null;
                    }
                    var IsEqualSurfaceBackgroundPatternId = overrideGraphicSettings.SurfaceBackgroundPatternId == overrideGraphic.SurfaceBackgroundPatternId;
                    if (IsEqualSurfaceBackgroundPatternId == false)
                    {
                        Pattern_Background_Pattern.SelectedItem = null;
                        Pattern_Background_SelectedLinePattern = null;
                    }
                    var IsEqualCutBackgroundPatternId = overrideGraphicSettings.CutBackgroundPatternId == overrideGraphic.CutBackgroundPatternId;
                    if (IsEqualCutBackgroundPatternId == false)
                    {
                        CutPattern_Background_Pattern.SelectedItem = null;
                        CutPattern_Background_SelectedLinePattern = null;
                    }
                    var IsEqualCutForegroundPatternId = overrideGraphicSettings.CutForegroundPatternId == overrideGraphic.CutForegroundPatternId;
                    if (IsEqualCutForegroundPatternId == false)
                    {
                        CutPattern_ForeGround_Pattern.SelectedItem = null;
                        CutPattern_Foreground_SelectedLinePattern = null;
                    }
                    var IsEqualSurfaceForegroundPatternId = overrideGraphicSettings.SurfaceForegroundPatternId == overrideGraphic.SurfaceForegroundPatternId;
                    if (IsEqualSurfaceForegroundPatternId == false)
                    {
                        Pattern_ForeGround_Pattern.SelectedItem = null;
                        Pattern_Foreground_SelectedLinePattern = null;
                    }
                    var IsEqualCutLineWeight = overrideGraphicSettings.CutLineWeight == overrideGraphic.CutLineWeight;
                    if (IsEqualCutLineWeight == false)
                    {
                        CutLines_LineWieght.SelectedItem = null;
                        CutLines_SelectedLineWeight = null;
                    }
                    var IsEqualProjectionLineWeight = overrideGraphicSettings.ProjectionLineWeight == overrideGraphic.ProjectionLineWeight;
                    if (IsEqualProjectionLineWeight == false)
                    {
                        Lines_LineWieght.SelectedItem = null;
                        Lines_SelectedLineWeight = null;
                    }
#endif
                }

            }
        }

        private static bool IsColorsEqual(Autodesk.Revit.DB.Color color1, Autodesk.Revit.DB.Color color2)
        {
            var colorOne = color1;
            var colorTwo = color2;
            if (colorOne.IsValid == true && colorTwo.IsValid == true)
            {
                if (colorOne.Green != colorTwo.Green) return false;
                if (colorOne.Red != colorTwo.Red) return false;
                if (colorOne.Blue != colorTwo.Blue) return false;
                return true;
            }
            if (colorOne.IsValid == false && colorTwo.IsValid == false) return true;
            return false;
        }

        #region Helpers
        public List<string> GetAlLineWeight()
        {
            return new List<string>() { NoOverrideValueName, "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };
        }
        public List<LinePatternElement> GetAllLinePatterns(Document _document)
        {
            var collector = new FilteredElementCollector(_document);
            var patterns = collector.OfClass(typeof(LinePatternElement))?.Cast<LinePatternElement>().ToList();
            return patterns;
        }
        public List<FillPatternElement> GetAllFillPatterns(Document _document)
        {
            var collector = new FilteredElementCollector(_document);
            var patterns = collector.OfClass(typeof(FillPatternElement))?.Cast<FillPatternElement>().ToList();
            return patterns;
        }
        public LinePatternElement GePattern(List<LinePatternElement> linePatterns, string patternName)
        {
            return linePatterns.FirstOrDefault(pa => pa.Name == patternName);
        }
        public FillPatternElement GePattern(List<FillPatternElement> linePatterns, string patternName)
        {
            return linePatterns.FirstOrDefault(pa => pa.Name == patternName);
        }
        public List<string> GetElementsNames(List<LinePatternElement> elements)
        {
            var names = new List<string>() { NoOverrideValueName , SolidName };
            foreach (var item in elements) names.Add(item.Name);
            return names;
        }
        public List<string> GetElementsNames(List<FillPatternElement> elements)
        {
            var names = new List<string>() { NoOverrideValueName };
            foreach (var item in elements) names.Add(item.Name);
            return names;
        }
        #endregion

        #region UI Property Changed
        private void Lines_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

            if (linePatternColor.SelectedColor.HasValue)
            {
                Color C = linePatternColor.SelectedColor.Value;
                Lines_SelectedLineColor = new Autodesk.Revit.DB.Color(C.R, C.G, C.B);

            }


        }
        private void Pattern_Foreground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

            if (PatterForegroundColor.SelectedColor.HasValue)
            {
                Color C = PatterForegroundColor.SelectedColor.Value;
                Pattern_Foreground_SelectedLineColor = new Autodesk.Revit.DB.Color(C.R, C.G, C.B);

            }

        }
        private void Pattern_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

            if (PatterBackgroundColor.SelectedColor.HasValue)
            {
                Color C = PatterBackgroundColor.SelectedColor.Value;
                Pattern_Background_SelectedLineColor = new Autodesk.Revit.DB.Color(C.R, C.G, C.B);
            }

        }
        private void CutPatterForegroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (CutPatterForegroundColor.SelectedColor.HasValue)
            {
                Color C = CutPatterForegroundColor.SelectedColor.Value;
                CutPattern_Foreground_SelectedLineColor = new Autodesk.Revit.DB.Color(C.R, C.G, C.B);
            }
        }
        private void CutPatterBackgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (CutPatterBackgroundColor.SelectedColor.HasValue)
            {
                Color C = CutPatterBackgroundColor.SelectedColor.Value;
                CutPattern_Background_SelectedLineColor = new Autodesk.Revit.DB.Color(C.R, C.G, C.B);
            }
        }
        private void CutlinePatternColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (CutlinePatternColor.SelectedColor.HasValue)
            {
                Color C = CutlinePatternColor.SelectedColor.Value;
                CutlinePatternColor_SelectedLineColor = new Autodesk.Revit.DB.Color(C.R, C.G, C.B);
            }
        }
        #endregion
        private string GetElementName(ElementId projectionLinePatternId)
        {
            if (projectionLinePatternId != null && projectionLinePatternId.Value != -1)
            {
                var element = this._doc.GetElement(projectionLinePatternId);
                if (element == null) return null;
                return element.Name;
            }
            return NoOverrideValueName;
        }

        #region Properties
        //-----------------------------------------------

        public string Lines_SelectedLinePattern { get; set; }
        public string Lines_SelectedLineWeight { get; set; }
        public Autodesk.Revit.DB.Color Lines_SelectedLineColor { get; set; }
        //-----------------------------------------------
        public string CutLines_SelectedLinePattern { get; set; }
        public string CutLines_SelectedLineWeight { get; set; }
        public Autodesk.Revit.DB.Color CutlinePatternColor_SelectedLineColor { get; private set; }
        //-----------------------------------------------



        public string Pattern_Foreground_SelectedLinePattern { get; set; }
        public string Pattern_Background_SelectedLinePattern { get; set; }
        public Autodesk.Revit.DB.Color Pattern_Foreground_SelectedLineColor { get; set; }
        public Autodesk.Revit.DB.Color Pattern_Background_SelectedLineColor { get; set; }
        public bool? BackgroundPatternVisible { get; set; } = true;
        public bool? ForegroundPatternVisible { get; set; } = true;
        //-----------------------------------------------


        public string CutPattern_Foreground_SelectedLinePattern { get; set; }
        public string CutPattern_Background_SelectedLinePattern { get; set; }
        public bool? CutBackgroundPatternVisible { get; set; } = true;
        public bool? CutForegroundPatternVisible { get; set; } = true;
        public Autodesk.Revit.DB.Color CutPattern_Background_SelectedLineColor { get; private set; }
        public Autodesk.Revit.DB.Color CutPattern_Foreground_SelectedLineColor { get; private set; }
        //-----------------------------------------------

        //-----------------------------------------------

        public int? TransparencyValue { get; set; } = 0;
        public new bool? Visibility { get; set; } = false;
        public bool? Halftone { get; set; } = false;

        //-----------------------------------------------







        public Document _doc { get; }
        public OverrideGraphicSettings OldSettings { get; set; }

        public OverrideElementModel OverrideElementModel { get; set; }
        public ICollection<ElementId> SelectedElements { get; }









        #endregion

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            TransparencyValue = 0;
            Halftone = false;
            Lines_SelectedLinePattern = NoOverrideValueName;
            Lines_SelectedLineWeight = NoOverrideValueName;
            Pattern_Foreground_SelectedLinePattern = NoOverrideValueName;
            Pattern_Background_SelectedLinePattern = NoOverrideValueName;
            BackgroundPatternVisible = true;
            ForegroundPatternVisible = true;
            Pattern_Background_SelectedLineColor = null;
            Pattern_Foreground_SelectedLineColor = null;
            Lines_SelectedLineColor = null;
            PatterBackgroundColor.SelectedColor = null;
            PatterForegroundColor.SelectedColor = null;
            linePatternColor.SelectedColor = null;
            Lines_Pattern.SelectedItem = NoOverrideValueName;
            Lines_LineWieght.SelectedItem = NoOverrideValueName;
            Pattern_ForeGround_Pattern.SelectedItem = NoOverrideValueName;
            Pattern_Background_Pattern.SelectedItem = NoOverrideValueName;
            transparencyBar.Value = 0;
            halftonecheckbox.IsChecked = false;
            backgroundVisible.IsChecked = (true);
            ForegroundVisible.IsChecked = (true);
            PatterBackgroundColor.SelectedColor = null;
            PatterForegroundColor.SelectedColor = null;
            linePatternColor.SelectedColor = null;
            CutLines_SelectedLinePattern = NoOverrideValueName;
            CutLines_SelectedLineWeight = NoOverrideValueName;
            CutPattern_Foreground_SelectedLinePattern = NoOverrideValueName;
            CutPattern_Background_SelectedLinePattern = NoOverrideValueName;
            CutBackgroundPatternVisible = true;
            CutForegroundPatternVisible = true;
            CutPattern_Background_SelectedLineColor = null;
            CutPattern_Foreground_SelectedLineColor = null;
            CutlinePatternColor_SelectedLineColor = null;
            CutPatterBackgroundColor.SelectedColor = null;
            CutPatterForegroundColor.SelectedColor = null;
            CutlinePatternColor.SelectedColor = null;
            CutLines_Pattern.SelectedItem = NoOverrideValueName;
            CutLines_LineWieght.SelectedItem = NoOverrideValueName;
            CutPattern_ForeGround_Pattern.SelectedItem = NoOverrideValueName;
            CutPattern_Background_Pattern.SelectedItem = NoOverrideValueName;
            CutbackgroundVisible.IsChecked = (true);
            CutForegroundVisible.IsChecked = (true);
            CutPatterBackgroundColor.SelectedColor = null;
            CutPatterForegroundColor.SelectedColor = null;
            CutlinePatternColor.SelectedColor = null;

        }
        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {




#if REVIT2025
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "(XML)|*.xml";
            if (dialog.ShowDialog() == DialogResult)

#else
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "(XML)|*.xml";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
#endif
            {
                SetDataToObject("Old");
                SerializerManager.SingelXMLFileSerialization(this.OverrideElementModel, dialog.FileName);
            }
        }
        private void SetDataToObject(string name)
        {
            OverrideElementModel = new OverrideElementModel()
            {
                Name = name,
                CutForegroundPatternName = this.CutPattern_Foreground_SelectedLinePattern,
                CutForegroundColor = this.CutPattern_Foreground_SelectedLineColor,
                CutForegroundVisible = this.CutForegroundPatternVisible.Value,
                CutBckgroundColor = this.CutPattern_Background_SelectedLineColor,
                CutBackgroundVisible = this.CutBackgroundPatternVisible.Value,
                CutBckgroundPatternName = this.CutPattern_Background_SelectedLinePattern,
                CutLineName = this.CutLines_SelectedLinePattern,
                CutLineColor = this.CutlinePatternColor_SelectedLineColor,
                CutLineWeight = this.CutLines_SelectedLineWeight,
                ProjectionLineColor = this.Lines_SelectedLineColor,
                ProjectionLineName = this.Lines_SelectedLinePattern,
                ProjectionLineWeight = this.Lines_SelectedLineWeight,
                SurfaceBackgroundVisible = this.BackgroundPatternVisible.Value,
                SurfaceForegroundVisible = this.ForegroundPatternVisible.Value,
                SurfaceBckgroundPatternName = this.Pattern_Background_SelectedLinePattern,
                SurfaceForegroundPatternName = this.Pattern_Foreground_SelectedLinePattern,
                SurfaceBckgroundColor = this.Pattern_Background_SelectedLineColor,
                SurfaceForegroundColor = this.Pattern_Foreground_SelectedLineColor,
                Visible = this.Visibility.Value,
                Transparency = this.TransparencyValue.Value,
                Halftone = this.Halftone.Value,

            };
            OverrideElementModel.ConvertColorsToString();
        }
        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchWord = ((WatermarkTextBox)sender).Text;

            ListOfOverrideElement.ItemsSource = OverrideElementModels.Where(s => s.Name.Contains(searchWord, StringComparison.OrdinalIgnoreCase));
        }
        private void LoadPresetButton_Click(object sender, RoutedEventArgs e)
        {

#if REVIT2025
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "(XML)|*.xml";
            if (ofd.ShowDialog() ==DialogResult)

#else
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "(XML)|*.xml";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
#endif
            {
                string path = ofd.FileName;
                var objec = (OverrideElementModel)SerializerManager.DeSerializationFromXML(new OverrideElementModel(), path);
                UpdateViewOverrides(objec);
            }
        }
        private void UpdateViewOverrides(OverrideElementModel objec)
        {
            objec.ConvertStringToColors();

            this.CutPattern_Foreground_SelectedLinePattern = objec.CutForegroundPatternName;
            this.CutPattern_Foreground_SelectedLineColor = objec.CutForegroundColor;
            this.CutForegroundPatternVisible = objec.CutForegroundVisible;
            this.CutPattern_Background_SelectedLineColor = objec.CutBckgroundColor;
            this.CutBackgroundPatternVisible = objec.CutBackgroundVisible;
            this.CutPattern_Background_SelectedLinePattern = objec.CutBckgroundPatternName;
            this.CutLines_SelectedLinePattern = objec.CutLineName;
            this.CutlinePatternColor_SelectedLineColor = objec.CutLineColor;
            this.CutLines_SelectedLineWeight = objec.CutLineWeight;
            this.Lines_SelectedLineColor = objec.ProjectionLineColor;
            this.Lines_SelectedLinePattern = objec.ProjectionLineName;
            this.Lines_SelectedLineWeight = objec.ProjectionLineWeight;
            this.BackgroundPatternVisible = objec.SurfaceBackgroundVisible;
            this.ForegroundPatternVisible = objec.SurfaceForegroundVisible;
            this.Pattern_Background_SelectedLinePattern = objec.SurfaceBckgroundPatternName;
            this.Pattern_Foreground_SelectedLinePattern = objec.SurfaceForegroundPatternName;
            this.Pattern_Background_SelectedLineColor = objec.SurfaceBckgroundColor;
            this.Pattern_Foreground_SelectedLineColor = objec.SurfaceForegroundColor;
            this.Visibility = objec.Visible;
            this.TransparencyValue = objec.Transparency;
            this.Halftone = objec.Halftone;
            #region Surface

            if (Pattern_Background_SelectedLineColor.IsValid)
            {
                PatterBackgroundColor.SelectedColor = Color.FromRgb(Pattern_Background_SelectedLineColor.Red, Pattern_Background_SelectedLineColor.Green, Pattern_Background_SelectedLineColor.Blue);
            }
            else PatterBackgroundColor.SelectedColor = null;

            if (Pattern_Foreground_SelectedLineColor.IsValid)
            {
                PatterForegroundColor.SelectedColor = Color.FromRgb(Pattern_Foreground_SelectedLineColor.Red, Pattern_Foreground_SelectedLineColor.Green, Pattern_Foreground_SelectedLineColor.Blue);
            }
            else PatterForegroundColor.SelectedColor = null;

            if (Lines_SelectedLineColor.IsValid)
            {
                linePatternColor.SelectedColor = Color.FromRgb(Lines_SelectedLineColor.Red, Lines_SelectedLineColor.Green, Lines_SelectedLineColor.Blue);
            }
            else linePatternColor.SelectedColor = null;

            if (Pattern_Foreground_SelectedLinePattern == null || Pattern_Foreground_SelectedLinePattern == NoOverrideValueName)
            {
                Pattern_ForeGround_Pattern.SelectedItem = NoOverrideValueName;
            }
            else Pattern_ForeGround_Pattern.SelectedItem = Pattern_Foreground_SelectedLinePattern;
            if (Pattern_Background_SelectedLinePattern == null || Pattern_Background_SelectedLinePattern == NoOverrideValueName)
            {
                Pattern_Background_Pattern.SelectedItem = NoOverrideValueName;
            }
            else Pattern_Background_Pattern.SelectedItem = Pattern_Background_SelectedLinePattern;
            if (Lines_SelectedLinePattern == null || Lines_SelectedLinePattern == NoOverrideValueName)
            {
                Lines_Pattern.SelectedItem = NoOverrideValueName;
            }
            else if(Lines_SelectedLinePattern == SolidName) Lines_Pattern.SelectedItem= SolidName;
            else Lines_Pattern.SelectedItem = Lines_SelectedLinePattern;
            if (Lines_SelectedLineWeight == null || Lines_SelectedLineWeight == NoOverrideValueName)
            {
                Lines_LineWieght.SelectedItem = NoOverrideValueName;
            }
            else Lines_LineWieght.SelectedItem = Lines_SelectedLineWeight;
            #endregion
            #region Cut
            if (CutPattern_Background_SelectedLineColor.IsValid)
            {
                CutPatterBackgroundColor.SelectedColor = Color.FromRgb(CutPattern_Background_SelectedLineColor.Red, CutPattern_Background_SelectedLineColor.Green, CutPattern_Background_SelectedLineColor.Blue);
            }
            else CutPatterBackgroundColor.SelectedColor = null;
            if (CutPattern_Foreground_SelectedLineColor.IsValid)
            {
                CutPatterForegroundColor.SelectedColor = Color.FromRgb(CutPattern_Foreground_SelectedLineColor.Red, CutPattern_Foreground_SelectedLineColor.Green, CutPattern_Foreground_SelectedLineColor.Blue);
            }
            else CutPatterForegroundColor.SelectedColor = null;
            if (CutlinePatternColor_SelectedLineColor.IsValid)
            {
                CutlinePatternColor.SelectedColor = Color.FromRgb(CutlinePatternColor_SelectedLineColor.Red, CutlinePatternColor_SelectedLineColor.Green, CutlinePatternColor_SelectedLineColor.Blue);
            }
            else CutlinePatternColor.SelectedColor = null;
            if (CutPattern_Foreground_SelectedLinePattern == null || CutPattern_Foreground_SelectedLinePattern == NoOverrideValueName)
            {
                CutPattern_ForeGround_Pattern.SelectedItem = NoOverrideValueName;
            }
            else CutPattern_ForeGround_Pattern.SelectedItem = CutPattern_Foreground_SelectedLinePattern;
            if (CutPattern_Background_SelectedLinePattern == null || CutPattern_Background_SelectedLinePattern == NoOverrideValueName)
            {
                CutPattern_Background_Pattern.SelectedItem = NoOverrideValueName;
            }
            else CutPattern_Background_Pattern.SelectedItem = CutPattern_Background_SelectedLinePattern;
            if (CutLines_SelectedLinePattern == null || CutLines_SelectedLinePattern == NoOverrideValueName)
            {
                CutLines_Pattern.SelectedItem = NoOverrideValueName;
            }else if (CutLines_SelectedLinePattern == SolidName) CutLines_SelectedLinePattern = SolidName;
            else CutLines_Pattern.SelectedItem = CutLines_SelectedLinePattern;
            if (CutLines_SelectedLineWeight == null || CutLines_SelectedLineWeight == NoOverrideValueName)
            {
                CutLines_LineWieght.SelectedItem = NoOverrideValueName;
            }
            else CutLines_LineWieght.SelectedItem = CutLines_SelectedLineWeight;
            #endregion
            transparencyBar.Value = TransparencyValue.Value;
            halftonecheckbox.IsChecked = Halftone;
            backgroundVisible.IsChecked = BackgroundPatternVisible;
            ForegroundVisible.IsChecked = ForegroundPatternVisible;
            CutbackgroundVisible.IsChecked = CutBackgroundPatternVisible;
            CutForegroundVisible.IsChecked = CutForegroundPatternVisible;
        }

        public OverrideGraphicSettings CreateFilterOverride()
        {
            var settings = OldSettings;
            if (settings == null) settings = new OverrideGraphicSettings();
            var patternList = LinePatternElements;

            if (Lines_SelectedLinePattern != NoOverrideValueName && Lines_SelectedLinePattern != SolidName && !string.IsNullOrEmpty(Lines_SelectedLinePattern))
            {
                var LinesLinePattern = GePattern(patternList, Lines_SelectedLinePattern);
                if (LinesLinePattern != null)
                {
                    settings.SetProjectionLinePatternId(LinesLinePattern.Id);
                }
            }
            else
            {
                settings.SetProjectionLinePatternId(ElementId.InvalidElementId);
            }
            if (Lines_SelectedLineWeight != NoOverrideValueName && !string.IsNullOrEmpty(Lines_SelectedLineWeight))
            {
                settings.SetProjectionLineWeight(int.Parse(Lines_SelectedLineWeight));
            }
            else
            {
                var lineWeight = new OverrideGraphicSettings().ProjectionLineWeight;
                settings.SetProjectionLineWeight(lineWeight);
            }
            if (Lines_SelectedLineColor != null)
            {
                settings.SetProjectionLineColor(Lines_SelectedLineColor);
            }
            else
            {

                settings.SetProjectionLineColor(Autodesk.Revit.DB.Color.InvalidColorValue);
            }
            if (Pattern_Foreground_SelectedLinePattern != NoOverrideValueName && !string.IsNullOrEmpty(Pattern_Foreground_SelectedLinePattern))
            {
                var LinesLinePattern = GePattern(FillPatterns, Pattern_Foreground_SelectedLinePattern);
                if (LinesLinePattern != null)
                {
#if REVIT2018
#else
                    settings.SetSurfaceForegroundPatternId(LinesLinePattern.Id);
#endif
                }
            }
            else
            {
#if REVIT2018
#else
                settings.SetSurfaceForegroundPatternId(new OverrideGraphicSettings().SurfaceForegroundPatternId);
#endif
            }
            if (Pattern_Foreground_SelectedLineColor != null)
            {
#if REVIT2018
#else
                settings.SetSurfaceForegroundPatternColor(Pattern_Foreground_SelectedLineColor);
#endif
            }
            else
            {
#if REVIT2018
#else
                settings.SetSurfaceForegroundPatternColor(Autodesk.Revit.DB.Color.InvalidColorValue);
#endif
            }
            if (Pattern_Background_SelectedLinePattern != NoOverrideValueName && !string.IsNullOrEmpty(Pattern_Background_SelectedLinePattern))
            {
                var LinesLinePattern = GePattern(FillPatterns, Pattern_Background_SelectedLinePattern);
                if (LinesLinePattern != null)
                {
#if REVIT2018
#else
                    settings.SetSurfaceBackgroundPatternId(LinesLinePattern.Id);
#endif
                }
            }
            else
            {
#if REVIT2018
#else
                settings.SetSurfaceBackgroundPatternId(new OverrideGraphicSettings().SurfaceBackgroundPatternId);
#endif
            }
            if (Pattern_Background_SelectedLineColor != null)
            {
#if REVIT2018
#else
                settings.SetSurfaceBackgroundPatternColor(Pattern_Background_SelectedLineColor);
#endif
            }
            else
            {
#if REVIT2018
#else
                settings.SetSurfaceBackgroundPatternColor(Autodesk.Revit.DB.Color.InvalidColorValue);
#endif
            }
            if (CutLines_SelectedLinePattern != NoOverrideValueName && CutLines_SelectedLinePattern != SolidName && !string.IsNullOrEmpty(CutLines_SelectedLinePattern))
            {
                var LinesLinePattern = GePattern(patternList, CutLines_SelectedLinePattern);
                if (LinesLinePattern != null)
                {
                    settings.SetCutLinePatternId(LinesLinePattern.Id);
                }
            }
            else
            {
                settings.SetCutLinePatternId(ElementId.InvalidElementId);
            }
            if (CutLines_SelectedLineWeight != NoOverrideValueName && !string.IsNullOrEmpty(CutLines_SelectedLineWeight))
            {
                settings.SetCutLineWeight(int.Parse(CutLines_SelectedLineWeight));
            }
            else
            {
                var lineWeight = new OverrideGraphicSettings().CutLineWeight;
                settings.SetCutLineWeight(lineWeight);
            }
            if (CutlinePatternColor_SelectedLineColor != null)
            {
                settings.SetCutLineColor(CutlinePatternColor_SelectedLineColor);
            }
            else
            {

                settings.SetCutLineColor(Autodesk.Revit.DB.Color.InvalidColorValue);
            }
            if (CutPattern_Foreground_SelectedLinePattern != NoOverrideValueName && !string.IsNullOrEmpty(CutPattern_Foreground_SelectedLinePattern))
            {
                var LinesLinePattern = GePattern(FillPatterns, CutPattern_Foreground_SelectedLinePattern);
                if (LinesLinePattern != null)
                {
#if REVIT2018
#else
                    settings.SetCutForegroundPatternId(LinesLinePattern.Id);
#endif
                }
            }
            else
            {
#if REVIT2018
#else
                settings.SetCutForegroundPatternId(new OverrideGraphicSettings().CutForegroundPatternId);
#endif
            }
            if (CutPattern_Foreground_SelectedLineColor != null)
            {
#if REVIT2018
#else
                settings.SetCutForegroundPatternColor(CutPattern_Foreground_SelectedLineColor);
#endif
            }
            else
            {
#if REVIT2018
#else
                settings.SetCutForegroundPatternColor(Autodesk.Revit.DB.Color.InvalidColorValue);
#endif
            }
            if (CutPattern_Background_SelectedLinePattern != NoOverrideValueName && !string.IsNullOrEmpty(CutPattern_Background_SelectedLinePattern))
            {
                var LinesLinePattern = GePattern(FillPatterns, CutPattern_Background_SelectedLinePattern);
                if (LinesLinePattern != null)
                {
#if REVIT2018
#else
                    settings.SetCutBackgroundPatternId(LinesLinePattern.Id);
#endif
                }
            }
            else
            {
#if REVIT2018
#else
                settings.SetCutBackgroundPatternId(new OverrideGraphicSettings().CutBackgroundPatternId);

#endif
            }
            if (CutPattern_Background_SelectedLineColor != null)
            {
#if REVIT2018
#else
                settings.SetCutBackgroundPatternColor(CutPattern_Background_SelectedLineColor);
#endif
            }
            else
            {
#if REVIT2018
#else
                settings.SetCutBackgroundPatternColor(Autodesk.Revit.DB.Color.InvalidColorValue);
#endif
            }
            if (CutForegroundPatternVisible != null)
            {
#if REVIT2018
#else
                settings.SetCutForegroundPatternVisible(CutForegroundPatternVisible.Value);
#endif
            }
            if (CutBackgroundPatternVisible != null)
            {
#if REVIT2018
#else
                settings.SetCutBackgroundPatternVisible(CutBackgroundPatternVisible.Value);
#endif
            }
            if (TransparencyValue != null)
            {
                settings.SetSurfaceTransparency(TransparencyValue.Value);
            }

            if (Halftone != null)
            {
                settings.SetHalftone(Halftone.Value);
            }
            if (ForegroundPatternVisible != null)
            {
#if REVIT2018
#else
                settings.SetSurfaceForegroundPatternVisible(ForegroundPatternVisible.Value);
#endif
            }
            if (BackgroundPatternVisible != null)
            {
#if REVIT2018
#else
                settings.SetSurfaceBackgroundPatternVisible(BackgroundPatternVisible.Value);
#endif
            }

            return settings;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            Expamder_001.IsExpanded = expander.Name == "Expamder_001";
            Expamder_002.IsExpanded = expander.Name == "Expamder_002";
            Expamder_003.IsExpanded = expander.Name == "Expamder_003";
            Expamder_004.IsExpanded = expander.Name == "Expamder_004";
            Expamder_005.IsExpanded = expander.Name == "Expamder_005";
        }

        #region Apply Button
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var overrideObject = CreateFilterOverride();
                if (overrideObject == null)
                {
                    this.Close();
                    return;
                }
                using (Transaction transaction = new Transaction(_uiDoc.Document, "Apply Overrides"))
                {
                    transaction.Start();
                    foreach (var item in SelectedElements)
                    {
                        _uiDoc.Document.ActiveView.SetElementOverrides(item, overrideObject);
                    }
                    transaction.Commit();

                }
                this.Close();
            }
            catch (Exception)
            {
            }
        }
        private void CopyElementOverrids_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();

#if REVIT2025
 if (MessageBox.Show("Select an element to copy Graphic Overrides from.", "Selection", MessageBoxButton.OK) == MessageBoxResult.OK)
#else

                if (System.Windows.Forms.MessageBox.Show("Select an element to copy Graphic Overrides from.", "Selection", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
#endif
                {
                    try
                    {
                        var obj = _uiDoc.Selection.PickObject(ObjectType.Element, "Select Hosting element");
                        if (obj == null) return;
                        var elementOverrides = _uiDoc.Document.ActiveView.GetElementOverrides(obj.ElementId);
                        using (Transaction transaction = new Transaction(_uiDoc.Document, "Copy Element Overrides"))
                        {
                            transaction.Start();
                            foreach (var item in SelectedElements)
                            {
                                _uiDoc.Document.ActiveView.SetElementOverrides(item, elementOverrides);
                            }
                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
                this.WindowState = System.Windows.WindowState.Maximized;
            else
                this.WindowState = System.Windows.WindowState.Normal;
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.aecspot.com");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.aecspot.com/models");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.aecspot.com/tools");
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void New_Preset_Clicked(object sender, RoutedEventArgs e)
        {
            var dialog = new SavePreset(OverrideElementModels.Select(s => s.Name).ToList());
            dialog.Owner = this;
            dialog.ShowDialog();

            if (dialog.IsConfirmed)
            {
                string presetName = dialog.PresetName;
                
                // Save the preset with the given name
                SetDataToObject(presetName);
                this.OverrideElementModel.Guid = Guid.NewGuid();
                SerializerManager.SingelXMLFileSerialization(this.OverrideElementModel, AppDataFolder + "\\" + OverrideElementModel.Guid + ".xml");
                OverrideElementModels.Add(OverrideElementModel);
            }
        }
        private void OverrideButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OverridePreset();
            dialog.Owner = this;


            if (dialog.ShowDialog() == true)
            {
                if (SelectedOverrideElementModels == null) return;
                // Save the preset with the given name
                SetDataToObject(SelectedOverrideElementModels.Name);
                this.OverrideElementModel.Guid = SelectedOverrideElementModels.Guid;

                SerializerManager.SingelXMLFileSerialization(this.OverrideElementModel, AppDataFolder + "\\" + SelectedOverrideElementModels.Guid + ".xml");
                OverrideElementModels.Clear();
                LoadPresets(AppDataFolder);
            }
        }
        private void RenamePreset_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOverrideElementModels == null) return;
            var dialog = new RenamePreset(SelectedOverrideElementModels.Name,OverrideElementModels.Select(s=>s.Name).ToList());
            dialog.Owner = this;


            if (dialog.ShowDialog() == true)
            {
                // Save the preset with the given name
                SetDataToObject(dialog.PresetName);
                this.OverrideElementModel.Guid = SelectedOverrideElementModels.Guid;

                SerializerManager.SingelXMLFileSerialization(this.OverrideElementModel, AppDataFolder + "\\" + SelectedOverrideElementModels.Guid + ".xml");
                OverrideElementModels.Clear();
                LoadPresets(AppDataFolder);
            }
        }
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new DeletePreset();
            dialog.Owner = this;
            dialog.ShowDialog();

            if (dialog.IsConfirmed)
            {
                if (File.Exists(AppDataFolder + "\\" + SelectedOverrideElementModels.Guid + ".xml"))
                {
                    File.Delete(AppDataFolder + "\\" + SelectedOverrideElementModels.Guid + ".xml");
                    OverrideElementModels.Remove(SelectedOverrideElementModels);
                }
            }
        }
        private void DuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOverrideElementModels != null)
            {
                SetDataToObject(SelectedOverrideElementModels.Name + " Copy");
                this.OverrideElementModel.Guid = Guid.NewGuid();
                SerializerManager.SingelXMLFileSerialization(this.OverrideElementModel, AppDataFolder + "\\" + OverrideElementModel.Guid + ".xml");
                OverrideElementModels.Add(OverrideElementModel);

            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedOverrideElementModels != null)
            {
                UpdateViewOverrides(SelectedOverrideElementModels);
                DeleteButton.IsEnabled = true;
                OverrideButton.IsEnabled = true;
                DuplicateButton.IsEnabled = true;
                RenameButton.IsEnabled = true;

            }
            else
            {
                GetElementsSelectedOverrids();
                DeleteButton.IsEnabled = false;
                OverrideButton.IsEnabled = false;
                DuplicateButton.IsEnabled = false;
                RenameButton.IsEnabled = false;
            }
        }
    }
}







