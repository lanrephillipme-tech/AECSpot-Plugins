#region NameSpaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

#endregion

namespace AECSpot
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class RevitApplication : IExternalApplication
    {
        public UIApplication UIApplication { get; set; }

        #region Properties

        private UIControlledApplication _application { get; set; }
        private RibbonPanel AECSpotPanel { get; set; }

        private string _tabName { get; set; } = "Add-Ins";
        private string LinesManagerPanelName { get; set; } = "AEC Spot";
        private string LinesManagerToolTip { get; set; } = "Apply Linestyles To The Selected Objects";
        public Assembly _assembly { get; set; } = Assembly.GetExecutingAssembly();

        #endregion

        #region OnShutdown

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        #endregion

        #region OnStartup

        public Result OnStartup(UIControlledApplication application)
        {
            #region Getting the current application and its documents
            var day=DateTime.UtcNow.Date.ToShortDateString();

/*            if (   day == "11/04/2022"
                || day== "11/4/2022"
                || day == "04/11/2022" 
                || day == "4/11/2022")
            {
                return Result.Cancelled;
            }*/
            _application = application;
            ///create New tap if not exist
            try
            {
                ///create ribbon tab
              /*  application.CreateRibbonTab(_tabName);*/
            }
            catch (Exception)
            {
            }
            ///create ribbon panel if not exist
            try
            {
                AECSpotPanel = application.CreateRibbonPanel(LinesManagerPanelName);
            }
            catch (Exception)
            {
                AECSpotPanel = application.GetRibbonPanels().Where(exe => exe.Name == LinesManagerPanelName).First();
            }
            AECSpotPanel.AddSeparator();
            ///create push button if not exist
            try
            {
                //var applayLineStyleBtton = CreatePushButton("btn_AECSpotMainApp", "Apply Lines", typeof(ContentMangerCommand).FullName, "AECSPOT_ApplyLines.png", AECSpotPanel, LinesManagerToolTip);
                //var CreateFilterBtton = CreatePushButton("btn_CreateFilter", "People Filter", typeof(CreateFilterCommand).FullName, "FilterCreator.png", AECSpotPanel, "Add a Rule-based Filter For 3D People to the Current View");
                //var SelectByFilterCommand = CreatePushButton("btn_SelectByFilter", "Select By Filter", typeof(SelectByFilterCommand).FullName, "FilterSelector.png", AECSpotPanel, "Select Elements By Rule Based Filter");
                //var CopyFilterCommand = CreatePushButton("btn_CopyFilter", "Copy Filter", typeof(CopyFilterCommand).FullName, "CopyFilter.png", AECSpotPanel, "Copy Current view Filter Overrides to another views.");
                //var TransferFilterFromObjectCommand = CreatePushButton("btn_TransferFilterFromObject", "Element Overrides", typeof(EODCommand).FullName, "SetOverride.png", AECSpotPanel, "Save, Load and Copy Element Overrides in View");

            }
            catch (Exception) { }

            #endregion

            return Result.Succeeded;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creating push button method
        /// </summary>
        /// <param name="btn_Name">     button internal name </param>
        /// <param name="btn_txt">      button clint text </param>
        /// <param name="className">    targeted command class </param>
        /// <param name="_imageSource"> button image </param>
        /// <param name="_panel">       button panel </param>
        public RibbonItem CreatePushButton(string btn_Name, string btn_txt, string className, string _imageName, RibbonPanel _panel, string ToolTip = null)
        {
            var stream = _assembly.GetManifestResourceStream(typeof(RevitApplication).Namespace + $".Resources.Images.{_imageName}");
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            PushButtonData pushButtonData = new PushButtonData(btn_Name, btn_txt, _assembly.Location, className) { LargeImage = bitmap };
            if (null != ToolTip)
            {
                pushButtonData.ToolTip = ToolTip;
            }
            return _panel.AddItem(pushButtonData);
            //return pushButtonData;
        }

        #endregion
    }
}
