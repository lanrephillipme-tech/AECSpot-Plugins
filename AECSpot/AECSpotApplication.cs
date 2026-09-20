using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace AECSpot
{
    public class AECSpotApplication : IExternalApplication
    {
       
        private string LinesManagerToolTip { get; set; } = "Apply Linestyles To The Selected Objects";

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded; 
        }

        public Result OnStartup(UIControlledApplication application)
        {

            try
            {
                
                var path = Assembly.GetExecutingAssembly();

                //application.CreateRibbonTab("AEC Spot");
                //RibbonPanel AECSpotPanel = application.CreateRibbonPanel("AEC Spot", "AEC Spot");
                //var applayLineStyleBtton = CreatePushButton(path, "btn_AECSpotMainApp", "Apply Lines", typeof(ContentMangerCommand).FullName, "AECSPOT_ApplyLines.png", AECSpotPanel, LinesManagerToolTip);
                //var CreateFilterBtton = CreatePushButton(path, "btn_CreateFilter", "People Filter", typeof(CreateFilterCommand).FullName, "FilterCreator.png", AECSpotPanel, "Add a Rule-based Filter For 3D People to the Current View");
                //var SelectByFilterCommand = CreatePushButton(path, "btn_SelectByFilter", "Select By Filter", typeof(SelectByFilterCommand).FullName, "FilterSelector.png", AECSpotPanel, "Select Elements By Rule Based Filter");
                //var CopyFilterCommand = CreatePushButton(path, "btn_CopyFilter", "Copy Filter", typeof(CopyFilterCommand).FullName, "CopyFilter.png", AECSpotPanel, "Copy Current view Filter Overrides to another views.");
                //var TransferFilterFromObjectCommand = CreatePushButton(path, "btn_TransferFilterFromObject", "Element Overrides", typeof(EODCommand).FullName, "SetOverride.png", AECSpotPanel, "Save, Load and Copy Element Overrides in View");
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
        public PushButtonData CreatePushButton(Assembly _assembly,string btn_Name, string btn_txt, string className, string _imageName, RibbonPanel _panel, string ToolTip = null)
        {
            var stream = _assembly.GetManifestResourceStream(typeof(AECSpotApplication).Namespace + $".Resources.Images.{_imageName}");
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            PushButtonData pushButtonData = new PushButtonData(btn_Name, btn_txt, _assembly.Location, className) { LargeImage = bitmap };
            if (null != ToolTip)
            {
                pushButtonData.ToolTip = ToolTip;
            }
            _panel.AddItem(pushButtonData);
            return pushButtonData;
        }
    }
}
