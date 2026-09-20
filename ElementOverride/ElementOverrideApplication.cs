using Autodesk.Revit.UI;
using System.Linq;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;


namespace ElementOverride
{
    public class ElementOverrideApplication : IExternalApplication
    {


        private string LinesManagerTabName { get; set; } = "AEC Spot";
        private string LinesManagerPanelName { get; set; } = "Element Overrides";


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
                    application.CreateRibbonTab(LinesManagerTabName);
                    AECSpotPanel = application.CreateRibbonPanel(LinesManagerTabName, LinesManagerPanelName);
                }
                catch (Exception ex)
                {
                    try
                    {
                        AECSpotPanel = application.CreateRibbonPanel(LinesManagerTabName, LinesManagerPanelName);

                    }
                    catch
                    {

                        AECSpotPanel = application.GetRibbonPanels(LinesManagerTabName).Where(exe => exe.Name == LinesManagerPanelName).First();
                    }
                }
                //RibbonPanel CopyFilterPanel = application.CreateRibbonPanel("AEC Spot", "AEC Spot");

                var TransferFilterFromObjectCommand = CreatePushButton(path, "btn_TransferFilterFromObject", "Manage Element Overrides", typeof(ElementOverrideCommand).FullName, "Override.png", AECSpotPanel, "Save, Load and Copy Element Overrides in View");
                var CopyFilterFromObjectCommand = CreatePushButton(path, "btn_CopyFilterFromObjectCommand", "Copy Element Overrides", typeof(CopyElementOverrideCommand).FullName, "SetOverride.png", AECSpotPanel, "Copy Element Overrides in View");

                //var CopyFilterCommand = CreatePushButton(path, "btn_CopyFilter", "Copy Filters", typeof(ElementOverrideCommand).FullName, "CopyFilter.png", AECSpotPanel, "Copy current view filters and overrides to other views.");

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
        public PushButtonData CreatePushButton(Assembly _assembly, string btn_Name, string btn_txt, string className, string _imageName, RibbonPanel _panel, string ToolTip = null)
        {
            var stream = _assembly.GetManifestResourceStream(typeof(ElementOverrideApplication).Namespace + $".Resources.Images.{_imageName}");
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            PushButtonData pushButtonData = new PushButtonData(btn_Name, btn_txt, _assembly.Location, className) { LargeImage = bitmap };
            if (null != ToolTip)
            {
                pushButtonData.ToolTip = ToolTip;
            }
            var ribbonItem =  _panel.AddItem(pushButtonData);
            ribbonItem.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://www.aecspot.com/tools/66d86dcc290fed3ab6720130"));
            return pushButtonData;
        }
    }
}
