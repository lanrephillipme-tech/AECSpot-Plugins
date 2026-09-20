using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WixSharp;
using File = WixSharp.File;

namespace SelectByFilterInstaller
{
    internal class Program
    {
        public static string versionValue = "1.0.0";
        static void Main()
        {
            var project = new ManagedProject($"AEC Spot - Element Override {versionValue}",
                              new Dir(@"CommonAppDataFolder\Autodesk\ApplicationPlugins",
                                  new Dir(@"ElementOverride.bundle",
                                  new File(@"..\ElementOverride\PackageContents.xml"),
                                      new Dir(@"Contents",
                                       new Dir(@"2018",
                                            new Dir(@"Resources", new Dir(@"Images", new Files(@"..\ElementOverride\bin\Debug\2018\Resources\Images\*.png"))),
                                            new File(@"..\ElementOverride\ElementOverride.addin"),
                                            new File(@"..\ElementOverride\bin\Debug\2018\ElementOverride.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2018\AECSpot.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2018\Xceed.Wpf.AvalonDock.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2018\Xceed.Wpf.AvalonDock.Themes.Aero.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2018\Xceed.Wpf.AvalonDock.Themes.Metro.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2018\Xceed.Wpf.AvalonDock.Themes.VS2010.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2018\Xceed.Wpf.Toolkit.dll",
                                                 new FilePermission("Everyone", GenericPermission.All))





                                            ),

                                         new Dir(@"2019",
                                            new Dir(@"Resources", new Dir(@"Images", new Files(@"..\ElementOverride\bin\Debug\2021\Resources\Images\*.png"))),
                                            new File(@"..\ElementOverride\ElementOverride.addin",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\ElementOverride.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\AECSpot.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Aero.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Metro.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.VS2010.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.Toolkit.dll",
                                                 new FilePermission("Everyone", GenericPermission.All))





                                            ), new Dir(@"2020",
                                            new Dir(@"Resources", new Dir(@"Images", new Files(@"..\ElementOverride\bin\Debug\2021\Resources\Images\*.png"))),
                                            new File(@"..\ElementOverride\ElementOverride.addin", new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\ElementOverride.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\AECSpot.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Aero.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Metro.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.VS2010.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.Toolkit.dll",
                                                 new FilePermission("Everyone", GenericPermission.All))





                                            ), new Dir(@"2021",
                                            new Dir(@"Resources", new Dir(@"Images", new Files(@"..\ElementOverride\bin\Debug\2021\Resources\Images\*.png"))),
                                            new File(@"..\ElementOverride\ElementOverride.addin",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\ElementOverride.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\AECSpot.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Aero.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Metro.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.VS2010.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.Toolkit.dll",
                                                 new FilePermission("Everyone", GenericPermission.All))





                                            ), new Dir(@"2022",
                                            new Dir(@"Resources", new Dir(@"Images", new Files(@"..\ElementOverride\bin\Debug\2021\Resources\Images\*.png"))),
                                            new File(@"..\ElementOverride\ElementOverride.addin",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\ElementOverride.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\AECSpot.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Aero.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Metro.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.VS2010.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.Toolkit.dll",
                                                 new FilePermission("Everyone", GenericPermission.All))





                                            ), new Dir(@"2023",
                                            new Dir(@"Resources", new Dir(@"Images", new Files(@"..\ElementOverride\bin\Debug\2021\Resources\Images\*.png"))),
                                            new File(@"..\ElementOverride\ElementOverride.addin", new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\ElementOverride.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\AECSpot.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Aero.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Metro.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.VS2010.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.Toolkit.dll",
                                                 new FilePermission("Everyone", GenericPermission.All))





                                            ), new Dir(@"2024",
                                            new Dir(@"Resources", new Dir(@"Images", new Files(@"..\ElementOverride\bin\Debug\2021\Resources\Images\*.png"))),
                                            new File(@"..\ElementOverride\ElementOverride.addin",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\ElementOverride.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2021\AECSpot.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Aero.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Metro.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.VS2010.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.Toolkit.dll",
                                                 new FilePermission("Everyone", GenericPermission.All))





                                            ), new Dir(@"2025",
                                            new Dir(@"Resources", new Dir(@"Images", new Files(@"..\ElementOverride\bin\Debug\2025\Resources\Images\*.png"))),
                                            new File(@"..\ElementOverride\ElementOverride.addin", new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2025\ElementOverride.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                            new File(@"..\ElementOverride\bin\Debug\2025\AECSpot.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                    
                                             new File(@"..\ElementOverride\bin\Debug\2025\ElementOverride.deps.json",
                                                 new FilePermission("Everyone", GenericPermission.All)),

                                                   new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Aero.dll",
                                                new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.Metro.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.AvalonDock.Themes.VS2010.dll",
                                                 new FilePermission("Everyone", GenericPermission.All)),
                                             new File(@"..\ElementOverride\bin\Debug\2021\Xceed.Wpf.Toolkit.dll",
                                                 new FilePermission("Everyone", GenericPermission.All))



                                            )
                                       ))));

            //new Dir(@"2019",

            //    new File(@"..\Common_glTF_Exporter\Leia_glTF_Exporter.addin"),
            //    new File(@"..\Revit_glTF_Exporter_2019\bin\Release\Leia_glTF_Exporter.dll.config",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2019\bin\Release\Leia_glTF_Exporter.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2019\bin\Release\DracoWrapper.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2019\bin\Release\MeshOpt.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2019\bin\Release\Newtonsoft.Json.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new Files(@"..\Revit_glTF_Exporter_2019\bin\Release\*.png")),
            //new Dir(@"2020",
            //    new File(@"..\Common_glTF_Exporter\Leia_glTF_Exporter.addin"),
            //    new File(@"..\Revit_glTF_Exporter_2020\bin\Release\Leia_glTF_Exporter.dll.config",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2020\bin\Release\Leia_glTF_Exporter.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2020\bin\Release\DracoWrapper.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2020\bin\Release\MeshOpt.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2020\bin\Release\Newtonsoft.Json.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new Files(@"..\Revit_glTF_Exporter_2020\bin\Release\*.png")),
            //new Dir(@"2021",
            //    new File(@"..\Common_glTF_Exporter\Leia_glTF_Exporter.addin"),
            //    new File(@"..\Revit_glTF_Exporter_2021\bin\Release\Leia_glTF_Exporter.dll.config",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2021\bin\Release\Leia_glTF_Exporter.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2021\bin\Release\DracoWrapper.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2021\bin\Release\MeshOpt.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2021\bin\Release\Newtonsoft.Json.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new Files(@"..\Revit_glTF_Exporter_2021\bin\Release\*.png")),
            //new Dir(@"2022",
            //    new File(@"..\Common_glTF_Exporter\Leia_glTF_Exporter.addin"),
            //    new File(@"..\Revit_glTF_Exporter_2022\bin\Release\Leia_glTF_Exporter.dll.config",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2022\bin\Release\Leia_glTF_Exporter.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2022\bin\Release\DracoWrapper.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2022\bin\Release\MeshOpt.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2022\bin\Release\Newtonsoft.Json.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new Files(@"..\Revit_glTF_Exporter_2022\bin\Release\*.png")),
            //new Dir(@"2023",
            //    new File(@"..\Common_glTF_Exporter\Leia_glTF_Exporter.addin"),
            //    new File(@"..\Revit_glTF_Exporter_2023\bin\Release\Leia_glTF_Exporter.dll.config",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2023\bin\Release\Leia_glTF_Exporter.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2023\bin\Release\DracoWrapper.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2023\bin\Release\MeshOpt.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2023\bin\Release\Newtonsoft.Json.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new Files(@"..\Revit_glTF_Exporter_2023\bin\Release\*.png")),
            //new Dir(@"2024",
            //    new File(@"..\Common_glTF_Exporter\Leia_glTF_Exporter.addin"),
            //    new File(@"..\Revit_glTF_Exporter_2024\bin\Release\Leia_glTF_Exporter.dll.config",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2024\bin\Release\Leia_glTF_Exporter.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2024\bin\Release\DracoWrapper.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2024\bin\Release\MeshOpt.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2024\bin\Release\Newtonsoft.Json.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new Files(@"..\Revit_glTF_Exporter_2024\bin\Release\*.png")),
            // new Dir(@"2025",
            //    new File(@"..\Common_glTF_Exporter\Leia_glTF_Exporter.addin"),
            //    new File(@"..\Revit_glTF_Exporter_2025\bin\Release\net8.0-windows\Leia_glTF_Exporter.dll.config",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2025\bin\Release\net8.0-windows\Leia_glTF_Exporter.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2025\bin\Release\net8.0-windows\DracoWrapper.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2025\bin\Release\net8.0-windows\MeshOpt.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2025\bin\Release\net8.0-windows\Newtonsoft.Json.dll",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new File(@"..\Revit_glTF_Exporter_2025\bin\Release\net8.0-windows\Leia_glTF_Exporter.deps.json",
            //        new FilePermission("Everyone", GenericPermission.All)),
            //    new Files(@"..\Revit_glTF_Exporter_2025\bin\Release\net8.0-windows\*.png"))

            //   );

            project.GUID = new Guid("E5C16EEA-B424-47FF-8F2D-BC18F79D76C7");
            project.ControlPanelInfo.Manufacturer = "AEC Spot";
            project.Version = new Version(versionValue);

            project.ManagedUI = new ManagedUI();

            project.ControlPanelInfo.ProductIcon = "Resources\\Copy-of-AEC-Spot-Logo-3.ico";

            project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                            .Add<LicenceDialog>()
                                            .Add<ProgressDialog>()
                                            .Add<ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
                                           .Add<FeaturesDialog>()
                                           .Add<ProgressDialog>()
                                           .Add<ExitDialog>();


            // Set majorUpgrade to automatically uninstall old versions
            project.MajorUpgrade = new MajorUpgrade
            {
                AllowDowngrades = false,
                AllowSameVersionUpgrades = true,
                DowngradeErrorMessage = "A newer version of your product is already installed.",
                IgnoreRemoveFailure = true,
                Schedule = UpgradeSchedule.afterInstallInitialize
            };

            project.Actions = new WixSharp.Action[]
            {
                new ManagedAction(CustomActions.CheckRevitProcess, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed)
            };

            var msiFile = project.BuildMsi();
        }
    }

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckRevitProcess(Session session)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);


                //MessageBox.Show("Success", "Installation");
                //return ActionResult.Success;

                if (!System.IO.File.Exists($"{appDataPath}\\Autodesk\\ApplicationPlugins\\ElementOverride.bundle\\PackageContents.xml"))
                {
                    //MessageBox.Show(System.IO.File.Exists($"{appDataPath}\\Autodesk\\ApplicationPlugins\\ElementOverride.bundle\\PackageContents.xml").ToString(), $"{appDataPath}\\Autodesk\\ApplicationPlugins\\ElementOverride.bundle\\PackageContents.xml");
                    return ActionResult.Success;
                }
                else
                {
                    //MessageBox.Show(System.IO.File.Exists($"{appDataPath}\\Autodesk\\ApplicationPlugins\\ElementOverride.bundle\\PackageContents.xml").ToString()+$"{appDataPath}\\Autodesk\\ApplicationPlugins\\ElementOverride.bundle\\PackageContents.xml");
                }
                Process[] processes = Process.GetProcessesByName("Revit");
                if (processes.Length > 0)
                {

                    var result = MessageBox.Show("Revit is currently running. Would you like to close it to continue with the installation? \n Yes : Close Revit Sessions \n No : Wait for Revit session to be closed \n Cancel : Cancel Installation", "Revit is running", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.Yes)
                    {
                        foreach (var process in processes)
                        {
                            process.Kill();
                            process.WaitForExit();
                        }

                        string basePath = Environment.ExpandEnvironmentVariables("%AppDataFolder%\\Autodesk\\ApplicationPlugins\\ElementOverride.bundle\\Contents");
                        List<string> filePaths = new List<string>();

                        for (int year = 2018; year <= 2025; year++)
                        {
                            //string filePath = $"{basePath}\\{year}\\ElementOverride.dll";
                            //bool overwrittable = WaitForFilesToBeOverwritable(filePath);
                            //if (!overwrittable)
                            //{
                            //    MessageBox.Show($"The file ElementOverride.dll {year} is still in use", "Warning");
                            //    return ActionResult.Failure;
                            //}
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        foreach (var process in processes)
                        {
                            // process.Kill();
                            process.Wait();
                        }

                        string basePath = Environment.ExpandEnvironmentVariables("%AppDataFolder%\\Autodesk\\ApplicationPlugins\\ElementOverride.bundle\\Contents");
                        List<string> filePaths = new List<string>();

                        for (int year = 2018; year <= 2025; year++)
                        {
                            //string filePath = $"{basePath}\\{year}\\ElementOverride.dll";
                            //bool overwrittable = WaitForFilesToBeOverwritable(filePath);
                            //if (!overwrittable)
                            //{
                            //    MessageBox.Show($"The file ElementOverride.dll {year} is still in use", "Warning");
                            //    return ActionResult.Failure;
                            //}
                        }
                        //return ActionResult.UserExit;
                    }
                    else
                    {
                        return ActionResult.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                session.Log("Error checking Revit process: " + ex.Message);
                MessageBox.Show(ex.Message, "Error");

                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        public static bool WaitForFilesToBeOverwritable(string filePath, int waitTimeMilliseconds = 1000, int maxAttempts = 50)
        {
            int attempts = 0;
            while (IsFileInUse(filePath) && attempts < maxAttempts)
            {
                Thread.Sleep(waitTimeMilliseconds);
                attempts++;
            }

            if (attempts >= maxAttempts)
            {
                MessageBox.Show("Installation Failed", "Failure");
                return false;
            }

            return true;
        }

        public static bool IsFileInUse(string filePath)
        {
            try
            {
                using (FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}
