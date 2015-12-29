using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using Medical.GUI;
using Developer.GUI;
using MyGUIPlugin;
using libRocketPlugin;
using Engine;
using Anomalous.libRocketWidget;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Debugging;
using OgrePlugin;
using System.IO;
using Engine.Platform;

namespace Developer
{
    public class DeveloperAtlasPlugin : AtlasPlugin
    {
        private DataFilePublisher dataFilePublisher;
        private DeveloperRenderPropertiesDialog developerRenderer;
        private DiscControl discControl;
        private AdvancedMandibleMovementDialog advancedMandibleMovement;
        private GridPropertiesDialog gridProperties;
        private PerformanceGui performanceGui;
        private DisablePhysicsTask disablePhysics;
        private MeasurementGUI measurementGUI;
        private ShowLibRocketDebugger libRocketDebugger;
        private ChangeRenderingMode changeRenderingMode;
        private DebugVisualizer debugVisualizer;
        private VirtualTextureDebugger virtualTextureDebugger;
        private AdvancedCameraGui advancedCameraGui;
        private ResolutionGui resolutionGui;

        public DeveloperAtlasPlugin(StandaloneController standaloneController)
        {
            this.AllowUninstall = true;
        }

        public void Dispose()
        {
            resolutionGui.Dispose();
            debugVisualizer.Dispose();
            changeRenderingMode.Dispose();
            libRocketDebugger.Dispose();
            measurementGUI.Dispose();
            advancedMandibleMovement.Dispose();
            dataFilePublisher.Dispose();
            developerRenderer.Dispose();
            discControl.Dispose();
            gridProperties.Dispose();
            performanceGui.Dispose();
            advancedCameraGui.Dispose();
            IDisposableUtil.DisposeIfNotNull(virtualTextureDebugger);
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Developer.Resources.DeveloperImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;

            //UI Helpers
            gridProperties = new GridPropertiesDialog(standaloneController.MeasurementGrid);
            guiManager.addManagedDialog(gridProperties);

            dataFilePublisher = new DataFilePublisher();
            guiManager.addManagedDialog(dataFilePublisher);

            developerRenderer = new DeveloperRenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer, guiManager, standaloneController.NotificationManager);
            guiManager.addManagedDialog(developerRenderer);

            discControl = new DiscControl();
            guiManager.addManagedDialog(discControl);

            advancedMandibleMovement = new AdvancedMandibleMovementDialog(standaloneController.MovementSequenceController, standaloneController.MusclePositionController);
            guiManager.addManagedDialog(advancedMandibleMovement);

            performanceGui = new PerformanceGui(standaloneController);
            guiManager.addManagedDialog(performanceGui);

            measurementGUI = new MeasurementGUI(standaloneController);
            guiManager.addManagedDialog(measurementGUI);

            debugVisualizer = new DebugVisualizer(standaloneController);
            guiManager.addManagedDialog(debugVisualizer);

            advancedCameraGui = new AdvancedCameraGui(standaloneController.SceneViewController);
            guiManager.addManagedDialog(advancedCameraGui);

            libRocketDebugger = new ShowLibRocketDebugger(guiManager, "ShowLibRocketDebugger", "Show LibRocket Debugger", "Developer.libRocketDebugger", "Developer");

            resolutionGui = new ResolutionGui(standaloneController.MainWindow);
            guiManager.addManagedDialog(resolutionGui);

            RocketInterface.Instance.FileInterface.addExtension(new RocketAssemblyResourceLoader(this.GetType().Assembly));

            //Task Controller
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(dataFilePublisher, "Developer.DataFilePublisher", "Data File Publisher", "Developer.PublisherIcon", TaskMenuCategories.Developer));

            taskController.addTask(new MDIDialogOpenTask(developerRenderer, "Developer.DeveloperRender", "Developer Renderer", "Developer.RenderIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(discControl, "Medical.DiscEditor", "Disc Editor", "Developer.DiscEditorIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(advancedMandibleMovement, "Medical.AdvancedMandibleMovement", "Advanced Mandible Movement", "Developer.MovementIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(gridProperties, "Medical.GridProperties", "Grid", "Developer.GridIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(performanceGui, "Medical.Performance", "Performance", "Developer.StatisticsIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(measurementGUI, "Developer.Measurement", "Measurements", "Developer.Measurements", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(debugVisualizer, "Developer.DebugVisualizer", "Debug Visualizer", "Developer.DebugVisualizer", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(advancedCameraGui, "Developer.AdvancedCameraGui", "Advanced Camera Settings", CommonResources.NoIcon, TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(resolutionGui, "Developer.SetResolution", "Set Resolution", CommonResources.NoIcon, TaskMenuCategories.Developer));
            taskController.addTask(libRocketDebugger);
            taskController.addTask(new SaveMicrocodeCacheTask());
            taskController.addTask(new CallbackTask("Developer.SaveToMax", "Save to 3ds Max", "Developer.MaxDumpIcon", TaskMenuCategories.Developer, (item) =>
                {
                    if (!item.Active)
                    {
                        item.setActive(true);
                        MaxExport maxExport = new MaxExport(standaloneController);
                        guiManager.addManagedDialog(maxExport);
                        maxExport.Visible = true;
                        maxExport.Closed += (evt, args) =>
                        {
                            maxExport.Dispose();
                            item.setActive(false);
                            item.closeTask();
                        };
                    }
                }));

            taskController.addTask(new CallbackTask("Developer.TestVolumeCalc", "Test Volume Calculation", CommonResources.NoIcon, TaskMenuCategories.Developer, (item) =>
            {
                VolumeCalculator calc;
                if(VolumeController.tryGetCalculator("LeftMasseter", out calc))
                {
                    Logging.Log.Debug("Volume is {0}", calc.CurrentVolume);
                }
                else
                {
                    Logging.Log.Debug("Cannot find volume calculator");
                }
            }));

            taskController.addTask(new CallbackTask("ToggleCompactMode", "Toggle Compact Mode", CommonResources.NoIcon, TaskMenuCategories.Developer, item =>
            {
                standaloneController.GUIManager.CompactMode = !standaloneController.GUIManager.CompactMode;
                standaloneController.GUIManager.layout();
            }));

            changeRenderingMode = new ChangeRenderingMode(standaloneController.SceneViewController);
            taskController.addTask(changeRenderingMode);

            disablePhysics = new DisablePhysicsTask(int.MaxValue);
            taskController.addTask(disablePhysics);

#if ALLOW_CRASH_PROGRAM
            taskController.addTask(new CallbackTask("Developer.Crash", "Crash The Program", CommonResources.NoIcon, TaskMenuCategories.Developer, (item) =>
            {
                throw new Exception("Manually crashed program");
            }));
#endif

            standaloneController.ViewHostFactory.addFactory(new WizardComponentViews());

            if (PlatformConfig.AllowFullscreenToggle)
            {
                CallbackTask toggleBorderless = new CallbackTask("Developer.ToggleBorderless", "Toggle Borderless", "AnomalousMedical/ToggleFullscreen", TaskMenuCategories.Developer, int.MaxValue, false, (item) =>
                {
                    MainWindow.Instance.toggleBorderless();
                });
                taskController.addTask(toggleBorderless);
            }

            if (standaloneController.VirtualTextureManager != null)
            {
                virtualTextureDebugger = new VirtualTextureDebugger(standaloneController.VirtualTextureManager);
                guiManager.addManagedDialog(virtualTextureDebugger);
                taskController.addTask(new MDIDialogOpenTask(virtualTextureDebugger, "Developer.VirtualTextureDebugger", "Virtual Texture Debugger", CommonResources.NoIcon, TaskMenuCategories.Developer));
            }

            var screenshotButtonEvent = new ButtonEvent(EventLayers.Gui, frameUp: (evtMgr) =>
            {
                try
                {
                    String screenshotFolder = Path.Combine(MedicalConfig.UserDocRoot, "Screenshots");
                    if (!Directory.Exists(screenshotFolder))
                    {
                        Directory.CreateDirectory(screenshotFolder);
                    }

                    OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget.writeContentsToTimestampedFile(Path.Combine(screenshotFolder, "Screenshot"), ".png");
                }
                catch(Exception ex)
                {
                    MessageBox.show(String.Format("{0} saving screenshot. Message {1}", ex.GetType().Name, ex.Message), "Error saving screenshot", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
            });
            screenshotButtonEvent.addButton(KeyboardButtonCode.KC_F8);
            screenshotButtonEvent.addButton(KeyboardButtonCode.KC_LSHIFT);
            standaloneController.MedicalController.EventManager.addEvent(screenshotButtonEvent);
        }

        public void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown)
        {

        }

        public void sceneLoaded(SimScene scene)
        {
            advancedMandibleMovement.sceneLoaded(scene);
            discControl.sceneLoaded(scene);
            disablePhysics.sceneChanged(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            advancedMandibleMovement.sceneUnloading(scene);
            discControl.sceneUnloading();
        }

        public long PluginId
        {
            get
            {
                return 8;
            }
        }

        public String PluginName
        {
            get
            {
                return "Developer Tools";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "Developer/BrandingImage";
            }
        }

        public String Location
        {
            get
            {
                return GetType().Assembly.Location;
            }
        }

        public Version Version
        {
            get
            {
                return GetType().Assembly.GetName().Version;
            }
        }

        public bool AllowUninstall { get; set; }

        public bool AllowRuntimeUninstall
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
            }
        }
    }
}
