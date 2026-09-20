using Autodesk.Revit.UI;
using System.Linq;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;


namespace FilterManager
{
    public class FilterManagerApplication : IExternalApplication
    {


        private string LinesManagerPanelName { get; set; } = "AEC Spot";

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {

            try
            {

                var path = Assembly.GetExecutingAssembly();


                RibbonPanel AECSpotPanel;
                try
                {
                application.CreateRibbonTab(LinesManagerPanelName);
                    AECSpotPanel = application.CreateRibbonPanel(LinesManagerPanelName, "Filter Manager");
                }
                catch (Exception)
                {
                    try
                    {
                        AECSpotPanel = application.CreateRibbonPanel(LinesManagerPanelName, "Filter Manager");

                    }
                    catch
                    {

                        AECSpotPanel = application.GetRibbonPanels(LinesManagerPanelName).Where(exe => exe.Name == "Filter Manager").First();
                    }
                }
                //RibbonPanel CopyFilterPanel = application.CreateRibbonPanel("AEC Spot", "AEC Spot");



                CreatePushButton(path, "btn_CopyFilter", "Transfer Filters", typeof(CopyFilterCommand).FullName, "CopyFilter.png", AECSpotPanel, "https://www.aecspot.com/tools/66d86a7c290fed3ab6720108", "Copy current view filters and overrides to other views.");
                CreatePushButton(path, "btn_CreateFilter", "People Filter", typeof(CreateFilterCommand).FullName, "FilterCreator.png", AECSpotPanel, "https://www.aecspot.com/tools/66d86c39290fed3ab6720125", "Add a Rule-based Filter For 3D People to the Current View");
                CreatePushButton(path, "btn_SelectByFilter", "Select By Filter", typeof(SelectByFilterCommand).FullName, "FilterSelector.png", AECSpotPanel, "https://www.aecspot.com/tools/66d85fc3290fed3ab67200b2", "Select elements matching Rule-based Filters");
                CreatePushButton(path, "btn_FindFilters", "Find Filters", typeof(FindFiltersCommand).FullName, "FilterSelector.png", AECSpotPanel, "https://www.aecspot.com/tools", "Find where rule-based filters are used in project views and templates.");
                CreatePushButton(path, "btn_DuplicateFilters", "Duplicate Filters", typeof(DuplicateFiltersCommand).FullName, "CopyFilter.png", AECSpotPanel, "https://www.aecspot.com/tools", "Find rule-based filters with equivalent definitions.");
                CreatePushButton(path, "btn_FilterTargets", "Filter Targets", typeof(FilterTargetsCommand).FullName, "FilterSelector.png", AECSpotPanel, "https://www.aecspot.com/tools", "Inspect filter categories, usage, and matching elements.");
                CreatePushButton(path, "btn_BulkRenameFilters", "Bulk Rename Filters", typeof(BulkRenameFiltersCommand).FullName, "FilterCreator.png", AECSpotPanel, "https://www.aecspot.com/tools", "Preview and apply validated bulk filter renames.");
                CreatePushButton(path, "btn_ReplaceFilters", "Replace Filters", typeof(ReplaceFiltersCommand).FullName, "CopyFilter.png", AECSpotPanel, "https://www.aecspot.com/tools", "Replace filter assignments while preserving source view settings.");
                CreatePushButton(path, "btn_PurgeFilters", "Purge Filters", typeof(PurgeFiltersCommand).FullName, "FilterCreator.png", AECSpotPanel, "https://www.aecspot.com/tools", "Preview and delete unused rule-based filters.");
                CreatePushButton(path, "btn_QuickFilter", "Quick Filter", typeof(QuickFilterCommand).FullName, "FilterSelector.png", AECSpotPanel, "https://www.aecspot.com/tools", "Build a session-only rule filter to select elements or save it to the project.");


                return Result.Succeeded;

            }
            catch
            {

                return Result.Succeeded;

            }

        }

        /// <summary>
        /// Creating push button method
        /// </summary>
        /// <param name="btn_Name">     button internal name </param>
        /// <param name="btn_txt">      button clint text </param>
        /// <param name="className">    targeted command class </param>
        /// <param name="_imageSource"> button image </param>
        /// <param name="_panel">       button panel </param>
        public PushButtonData CreatePushButton(Assembly _assembly, string btn_Name, string btn_txt, string className, string _imageName, RibbonPanel _panel,string url ,string ToolTip = null)
        {
            var stream = _assembly.GetManifestResourceStream(typeof(FilterManagerApplication).Namespace + $".Resources.Images.{_imageName}");
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            PushButtonData pushButtonData = new PushButtonData(btn_Name, btn_txt, _assembly.Location, className) { LargeImage = bitmap };
            if (null != ToolTip)
            {
                pushButtonData.ToolTip = ToolTip;
            }
            var ribbonItem = _panel.AddItem(pushButtonData);
            ribbonItem.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, url));
            return pushButtonData;
        }
    }
}
