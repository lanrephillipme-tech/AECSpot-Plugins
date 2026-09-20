using Autodesk.Revit.UI;
using System.Linq;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace SelectByFilter
{
    public class SelectByFilterApplication : IExternalApplication
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
                catch (Exception ex)
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
                var SelectByFilterCommand = CreatePushButton(path,"btn_SelectByFilter", "Select By Filter", typeof(SelectByFilterCommand).FullName, "FilterSelector.png", AECSpotPanel, "Select elements matching Rule-based Filters");

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
            var stream = _assembly.GetManifestResourceStream(typeof(SelectByFilterApplication).Namespace + $".Resources.Images.{_imageName}");
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            PushButtonData pushButtonData = new PushButtonData(btn_Name, btn_txt, _assembly.Location, className) { LargeImage = bitmap};
            
            if (null != ToolTip)
            {
                pushButtonData.ToolTip = ToolTip;
            }
            var ribbonItem = _panel.AddItem(pushButtonData);
            ribbonItem.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, "https://www.aecspot.com/tools/66d85fc3290fed3ab67200b2"));
            return pushButtonData;
        }
    }
}
