using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Color = System.Windows.Media.Color;

namespace AECSpot
{
    /// <summary>
    /// Interaction logic for AplayFilterVisibilitySettings.xaml
    /// </summary>
    public partial class AplayFilterVisibilitySettings : Window
    {
        public const string NoOverrideValueName = "<No Override>";
        public List<LinePatternElement> LinePatternElements { get; set; }
        public List<FillPatternElement> FillPatterns { get; set; }
        public AplayFilterVisibilitySettings(Document _document, OverrideGraphicSettings OldSettings)
        {
            LinePatternElements = GetAllLinePatterns(_document);
            FillPatterns = GetAllFillPatterns(_document);

            var patterns = GetElementsNames(LinePatternElements);
            var fillPAterns = GetElementsNames(FillPatterns);


            InitializeComponent();
            Pattern_Background_Pattern.ItemsSource = fillPAterns;
            Pattern_ForeGround_Pattern.ItemsSource = fillPAterns;
            Lines_Pattern.ItemsSource = patterns;
            Lines_LineWieght.ItemsSource = GetAlLineWeight();
            this.DataContext = this;
            Document = _document;
            this.OldSettings = OldSettings;
            if (OldSettings != null)
            {
#if REVIT2018
       Lines_SelectedLinePattern = GetElementName(OldSettings.ProjectionLinePatternId);
                Lines_SelectedLineWeight = (OldSettings.ProjectionLineWeight).ToString();
                Pattern_Foreground_SelectedLinePattern = GetElementName(OldSettings.ProjectionFillPatternId);
                Pattern_Background_SelectedLinePattern = GetElementName(OldSettings.CutFillPatternId);
                TransparencyValue = OldSettings.Transparency;
                Halftone = OldSettings.Halftone;
                BackgroundPatternVisible = (OldSettings.IsCutFillPatternVisible);
                ForegroundPatternVisible = (OldSettings.IsProjectionFillPatternVisible);
                if (OldSettings.CutFillColor.IsValid) PatterBackgroundColor.SelectedColor = Color.FromRgb(OldSettings.CutFillColor.Red, OldSettings.CutFillColor.Green, OldSettings.CutFillColor.Blue);
                if (OldSettings.ProjectionFillColor.IsValid) PatterForegroundColor.SelectedColor = Color.FromRgb(OldSettings.ProjectionFillColor.Red, OldSettings.ProjectionFillColor.Green, OldSettings.ProjectionFillColor.Blue);
                if (OldSettings.ProjectionLineColor.IsValid) linePatternColor.SelectedColor = Color.FromRgb(OldSettings.ProjectionLineColor.Red, OldSettings.ProjectionLineColor.Green, OldSettings.ProjectionLineColor.Blue);
#else
                Lines_SelectedLinePattern = GetElementName(OldSettings.ProjectionLinePatternId);
                Lines_SelectedLineWeight = (OldSettings.ProjectionLineWeight).ToString();
                Pattern_Foreground_SelectedLinePattern = GetElementName(OldSettings.SurfaceForegroundPatternId);
                Pattern_Background_SelectedLinePattern = GetElementName(OldSettings.SurfaceBackgroundPatternId);
                TransparencyValue = OldSettings.Transparency;
                Halftone = OldSettings.Halftone;
                BackgroundPatternVisible = (OldSettings.IsSurfaceBackgroundPatternVisible);
                ForegroundPatternVisible = (OldSettings.IsSurfaceForegroundPatternVisible);
                if (OldSettings.SurfaceBackgroundPatternColor.IsValid) PatterBackgroundColor.SelectedColor = Color.FromRgb(OldSettings.SurfaceBackgroundPatternColor.Red, OldSettings.SurfaceBackgroundPatternColor.Green, OldSettings.SurfaceBackgroundPatternColor.Blue);
                if (OldSettings.SurfaceForegroundPatternColor.IsValid) PatterForegroundColor.SelectedColor = Color.FromRgb(OldSettings.SurfaceForegroundPatternColor.Red, OldSettings.SurfaceForegroundPatternColor.Green, OldSettings.SurfaceForegroundPatternColor.Blue);
                if (OldSettings.ProjectionLineColor.IsValid) linePatternColor.SelectedColor = Color.FromRgb(OldSettings.ProjectionLineColor.Red, OldSettings.ProjectionLineColor.Green, OldSettings.ProjectionLineColor.Blue);
#endif
            }
        }
        public OverrideGraphicSettings CreateFilterOverride()
        {
            var settings = OldSettings;

#if REVIT2018
            if (settings == null) settings = new OverrideGraphicSettings();
            var patternList = LinePatternElements;

            if (Lines_SelectedLinePattern != NoOverrideValueName && !string.IsNullOrEmpty(Lines_SelectedLinePattern))
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
                    settings.SetProjectionFillPatternId(LinesLinePattern.Id);
                }
            }
            else
            {
                settings.SetProjectionFillPatternId(new OverrideGraphicSettings().ProjectionFillPatternId);
            }
            if (Pattern_Foreground_SelectedLineColor != null)
            {
                settings.SetProjectionFillColor(Pattern_Foreground_SelectedLineColor);
            }
            else
            {
                settings.SetProjectionFillColor(Autodesk.Revit.DB.Color.InvalidColorValue);
            }
            if (Pattern_Background_SelectedLinePattern != NoOverrideValueName && !string.IsNullOrEmpty(Pattern_Background_SelectedLinePattern))
            {
                var LinesLinePattern = GePattern(FillPatterns, Pattern_Background_SelectedLinePattern);
                if (LinesLinePattern != null)
                {
                    settings.SetCutFillPatternId(LinesLinePattern.Id);
                }
            }
            else
            {
                settings.SetCutFillPatternId(new OverrideGraphicSettings().CutFillPatternId);
            }
            if (Pattern_Background_SelectedLineColor != null)
            {
                settings.SetCutFillColor(Pattern_Background_SelectedLineColor);
            }
            else
            {
                settings.SetCutFillColor(Autodesk.Revit.DB.Color.InvalidColorValue);
            }
            settings.SetSurfaceTransparency(TransparencyValue);
            settings.SetHalftone(Halftone);
            settings.SetProjectionFillPatternVisible(ForegroundPatternVisible);
            settings.SetCutFillPatternVisible(BackgroundPatternVisible);
#else
            if (settings == null) settings = new OverrideGraphicSettings();
            var patternList = LinePatternElements;

            if (Lines_SelectedLinePattern != NoOverrideValueName && !string.IsNullOrEmpty(Lines_SelectedLinePattern))
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
                    settings.SetSurfaceForegroundPatternId(LinesLinePattern.Id);
                }
            }
            else
            {
                settings.SetSurfaceForegroundPatternId(new OverrideGraphicSettings().SurfaceForegroundPatternId);
            }
            if (Pattern_Foreground_SelectedLineColor != null)
            {
                settings.SetSurfaceForegroundPatternColor(Pattern_Foreground_SelectedLineColor);
            }
            else
            {
                settings.SetSurfaceForegroundPatternColor(Autodesk.Revit.DB.Color.InvalidColorValue);
            }
            if (Pattern_Background_SelectedLinePattern != NoOverrideValueName && !string.IsNullOrEmpty(Pattern_Background_SelectedLinePattern))
            {
                var LinesLinePattern = GePattern(FillPatterns, Pattern_Background_SelectedLinePattern);
                if (LinesLinePattern != null)
                {
                    settings.SetSurfaceBackgroundPatternId(LinesLinePattern.Id);
                }
            }
            else
            {
                settings.SetSurfaceBackgroundPatternId(new OverrideGraphicSettings().SurfaceBackgroundPatternId);
            }
            if (Pattern_Background_SelectedLineColor != null)
            {
                settings.SetSurfaceBackgroundPatternColor(Pattern_Background_SelectedLineColor);
            }
            else
            {
                settings.SetSurfaceBackgroundPatternColor(Autodesk.Revit.DB.Color.InvalidColorValue);
            }
            settings.SetSurfaceTransparency(TransparencyValue);
            settings.SetHalftone(Halftone);
            settings.SetSurfaceForegroundPatternVisible(ForegroundPatternVisible);
            settings.SetSurfaceBackgroundPatternVisible(BackgroundPatternVisible);

#endif
            return settings;
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
            var names = new List<string>() { NoOverrideValueName };
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
        #endregion
        private string GetElementName(ElementId projectionLinePatternId)
        {
            if (projectionLinePatternId != null && projectionLinePatternId.Value != -1)
            {
                return this.Document.GetElement(projectionLinePatternId).Name;
            }
            return NoOverrideValueName;
        }

        #region Properties
        public string Lines_SelectedLinePattern { get; set; }


        public string Lines_SelectedLineWeight { get; set; }
        public Autodesk.Revit.DB.Color Lines_SelectedLineColor { get; set; }

        public string Pattern_Foreground_SelectedLinePattern { get; set; }
        public string Pattern_Background_SelectedLinePattern { get; set; }


        public Autodesk.Revit.DB.Color Pattern_Foreground_SelectedLineColor { get; set; }
        public Autodesk.Revit.DB.Color Pattern_Background_SelectedLineColor { get; set; }


        public int TransparencyValue { get; set; }
        public new bool Visibility { get; set; }
        public bool EnableFilter { get; set; }
        public bool Halftone { get; set; }
        public bool BackgroundPatternVisible { get; set; } = true;
        public bool ForegroundPatternVisible { get; set; } = true;
        public Document Document { get; }
        public OverrideGraphicSettings OldSettings { get; }











        #endregion

        #region Apply Button
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        #endregion

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Lines_SelectedLinePattern = NoOverrideValueName;
            Lines_SelectedLineWeight = NoOverrideValueName;
            Pattern_Foreground_SelectedLinePattern = NoOverrideValueName;
            Pattern_Background_SelectedLinePattern = NoOverrideValueName;
            TransparencyValue = 0;
            Halftone = false;
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
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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
    }
}







