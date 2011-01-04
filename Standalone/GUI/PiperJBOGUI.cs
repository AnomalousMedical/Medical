using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using Engine.ObjectManagement;
using Medical.Controller;
using Logging;
using Engine.Platform;
using Engine;
using System.Reflection;
using System.IO;

namespace Medical.GUI
{
    public class PiperJBOGUI : IDisposable
    {
        private static String INTERFACE_NAME = typeof(GUIPlugin).Name;

        private ScreenLayoutManager screenLayoutManager;
        private StandaloneController standaloneController;
        private LeftPopoutLayoutContainer leftAnimatedContainer;
        private VerticalPopoutLayoutContainer topAnimatedContainer;
        private VerticalPopoutLayoutContainer bottomAnimatedContainer;
        private StateWizardPanelController stateWizardPanelController;
        private StateWizardController stateWizardController;

        private Taskbar taskbar;
        private BorderLayoutContainer innerBorderLayout;

        //Dialogs
        private DialogManager dialogManager;
        
        //Other GUI Elements
        private MyGUIContinuePromptProvider continuePrompt;
        private MyGUIQuestionProvider questionProvider;
        private List<GUIPlugin> plugins = new List<GUIPlugin>();
        private AppMenu appMenu;

        public PiperJBOGUI(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;
        }

        public void Dispose()
        {
            //Dialogs
            dialogManager.saveDialogLayout(MedicalConfig.WindowsFile);

            foreach (GUIPlugin plugin in plugins)
            {
                plugin.Dispose();
            }

            stateWizardController.Dispose();
            stateWizardPanelController.Dispose();

            //Other
            questionProvider.Dispose();
            continuePrompt.Dispose();
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            taskbar.Dispose();
        }

        public void addPlugin(String dllName)
        {
            String fullPath = Path.GetFullPath(dllName);
            if (File.Exists(fullPath))
            {
                Assembly assembly = Assembly.LoadFile(fullPath);
                Type[] exportedTypes = assembly.GetExportedTypes();
                Type pluginType = null;
                foreach (Type type in exportedTypes)
                {
                    if (type.GetInterface(INTERFACE_NAME) != null)
                    {
                        pluginType = type;
                        break;
                    }
                }
                if (pluginType != null && !pluginType.IsInterface && !pluginType.IsAbstract)
                {
                    GUIPlugin plugin = (GUIPlugin)Activator.CreateInstance(pluginType);
                    addPlugin(plugin);
                }
                else
                {
                    Log.Error("Cannot find GUIPlugin in assembly {0}. Please implement the GUIPlugin function in that assembly.", assembly.FullName);
                }
            }
            else
            {
                Log.Error("Cannot find Assembly {0}.", fullPath);
            }
        }

        public void addPlugin(GUIPlugin plugin)
        {
            OgreResourceGroupManager.getInstance().addResourceLocation(plugin.GetType().AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);
            plugins.Add(plugin);
        }

        public void setAppMenu(AppMenu appMenu)
        {
            this.appMenu = appMenu;
        }

        public void createGUI()
        {
            Gui gui = Gui.Instance;

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Imagesets", "EngineArchive", "MyGUI", true);
            OgreResourceGroupManager.getInstance().addResourceLocation(typeof(PiperJBOGUI).AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);

            typeof(PiperJBOGUI).Assembly.GetManifestResourceNames();

            gui.load("Imagesets.xml");

            stateWizardPanelController = new StateWizardPanelController(gui, standaloneController.MedicalController, standaloneController.MedicalStateController, standaloneController.NavigationController, standaloneController.LayerController, standaloneController.SceneViewController, standaloneController.TemporaryStateBlender, standaloneController.MovementSequenceController, standaloneController.ImageRenderer, standaloneController.MeasurementGrid);
            stateWizardController = new StateWizardController(standaloneController.MedicalController.MainTimer, standaloneController.TemporaryStateBlender, standaloneController.NavigationController, standaloneController.LayerController, this);
            stateWizardController.StateCreated += new MedicalStateCreated(stateWizardController_StateCreated);
            stateWizardController.Finished += new StatePickerFinished(stateWizardController_Finished);

            createWizardPanels();

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.ScreenSizeChanged += new ScreenSizeChanged(screenLayoutManager_ScreenSizeChanged);
            innerBorderLayout = new BorderLayoutContainer();

            foreach (GUIPlugin plugin in plugins)
            {
                plugin.initializeGUI(standaloneController, this);
            }

            //Dialogs
            dialogManager = new DialogManager();
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.createDialogs(dialogManager);
            }
            
            //Taskbar
            taskbar = new Taskbar(this, appMenu, standaloneController);
            taskbar.SuppressLayout = true;
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.addToTaskbar(taskbar);
            }

            taskbar.Child = innerBorderLayout;
            screenLayoutManager.Root = taskbar;

            topAnimatedContainer = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Top = topAnimatedContainer;

            leftAnimatedContainer = new LeftPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Left = leftAnimatedContainer;

            bottomAnimatedContainer = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Bottom = bottomAnimatedContainer;

            screenLayoutManager.Root.SuppressLayout = false;

            standaloneController.SceneViewController.ActiveWindowChanged += new SceneViewWindowEvent(SceneViewController_ActiveWindowChanged);

            standaloneController.ImageRenderer.ImageRendererProgress = new MyGUIImageRendererProgress();

            taskbar.SuppressLayout = false;
            taskbar.layout();

            dialogManager.loadDialogLayout(MedicalConfig.WindowsFile);

            continuePrompt = new MyGUIContinuePromptProvider();
            standaloneController.TimelineController.ContinuePrompt = continuePrompt;

            questionProvider = new MyGUIQuestionProvider(this);
            standaloneController.TimelineController.QuestionProvider = questionProvider;
        }

        public void windowChanged(OSWindow newWindow)
        {
            screenLayoutManager.changeOSWindow(newWindow);
        }

        public void changeTopPanel(LayoutContainer topContainer)
        {
            if (topContainer != null)
            {
                topContainer.Visible = true;
                topContainer.bringToFront();
            }
            topAnimatedContainer.changePanel(topContainer, 0.25f, animationCompleted);
        }

        public void resetTopPanel()
        {
            changeTopPanel(null);
        }

        public void changeLeftPanel(LayoutContainer leftContainer)
        {
            if (leftContainer != null)
            {
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            if (leftAnimatedContainer.CurrentContainer != leftContainer)
            {
                leftAnimatedContainer.changePanel(leftContainer, 0.25f, animationCompleted);
            }
        }

        public void changeBottomPanel(LayoutContainer bottomContainer)
        {
            if (bottomContainer != null)
            {
                bottomContainer.Visible = true;
                bottomContainer.bringToFront();
            }
            bottomAnimatedContainer.changePanel(bottomContainer, 0.25f, animationCompleted);
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.setMainInterfaceEnabled(enabled);
            }
            if (enabled)
            {
                taskbar.Visible = true;
                dialogManager.reopenDialogs();
            }
            else
            {
                taskbar.Visible = false;
                dialogManager.temporarilyCloseDialogs();
            }
        }

#if CREATE_MAINWINDOW_MENU

        public wx.MenuBar createMenuBar()
        {
            wx.MenuBar menu = new wx.MenuBar();
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.createMenuBar(menu);
            }
            return menu;
        }

#endif

        void SceneViewController_ActiveWindowChanged(SceneViewWindow window)
        {
            stateWizardController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
            stateWizardPanelController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
        }

        #region StateWizard Callbacks

        public void startWizard(StateWizard wizard)
        {
            stateWizardPanelController.CurrentWizardName = wizard.Name;
            stateWizardController.startWizard(wizard);
            standaloneController.MovementSequenceController.stopPlayback();
            setMainInterfaceEnabled(false);
        }

        void stateWizardController_Finished()
        {
            setMainInterfaceEnabled(true);
        }

        void stateWizardController_StateCreated(MedicalState state)
        {
            standaloneController.MedicalStateController.addState(state);
        }

        #endregion StateWizard Callbacks

        public BorderLayoutContainer ScreenLayout
        {
            get
            {
                return innerBorderLayout;
            }
        }

        private void animationCompleted(LayoutContainer oldChild)
        {
            if (oldChild != null)
            {
                oldChild.Visible = false;
            }
        }

        private void standaloneController_SceneUnloading(SimScene scene)
        {
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.sceneUnloading(scene);
            }
        }

        private void standaloneController_SceneLoaded(SimScene scene)
        {
            stateWizardPanelController.sceneChanged(standaloneController.MedicalController, scene.getDefaultSubScene().getSimElementManager<SimulationScene>());
            this.changeLeftPanel(null);
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.sceneLoaded(scene);
            }
        }

        void screenLayoutManager_ScreenSizeChanged(int width, int height)
        {
            dialogManager.windowResized();
            continuePrompt.ensureVisible();
        }

        private void createWizardPanels()
        {
            //Create single distortion wizards
            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DOPPLER))
            {
                //Doppler
                StateWizard dopplerWizard = new StateWizard("Doppler", stateWizardController, WizardType.Exam);
                dopplerWizard.TextLine1 = "Doppler";
                dopplerWizard.ImageKey = "DistortionsToolstrip/Doppler";
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(dopplerWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DENTITION))
            {
                //Teeth
                StateWizard teethWizard = new StateWizard("Dentition", stateWizardController, WizardType.Anatomy);
                teethWizard.TextLine1 = "Dentition";
                teethWizard.ImageKey = "DistortionsToolstrip/Dentition";
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(teethWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CEPHALOMETRIC))
            {
                //Profile
                StateWizard profileWizard = new StateWizard("Cephalometric", stateWizardController, WizardType.Anatomy);
                profileWizard.TextLine1 = "Cephalometric";
                profileWizard.ImageKey = "DistortionsToolstrip/Cephalometric";
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(profileWizard);
            }

            

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_MANDIBLE))
            {
                //Bone
                StateWizard boneWizard = new StateWizard("Mandible", stateWizardController, WizardType.Anatomy);
                boneWizard.TextLine1 = "Mandible";
                boneWizard.ImageKey = "DistortionsToolstrip/Mandible";
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(boneWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DISC_SPACE))
            {
                //Disc
                StateWizard discWizard = new StateWizard("Disc Space", stateWizardController, WizardType.Exam);
                discWizard.TextLine1 = "Disc Space";
                discWizard.ImageKey = "DistortionsToolstrip/DiscSpace";
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(discWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DISC_CLOCK))
            {
                //Disc
                StateWizard discClockWizard = new StateWizard("Disc Clock Face", stateWizardController, WizardType.Anatomy);
                discClockWizard.TextLine1 = "Disc";
                discClockWizard.TextLine2 = "Clock Face";
                discClockWizard.ImageKey = "DistortionsToolstrip/DiscClockFace";
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(discClockWizard);
            }

            //Create combination distortion wizards

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION))
            {
                //Profile + Teeth
                StateWizard profileTeethWizard = new StateWizard("Cephalometric and Dentition", stateWizardController, WizardType.Exam);
                profileTeethWizard.TextLine1 = "Cephalometric";
                profileTeethWizard.TextLine2 = "and Dentition";
                profileTeethWizard.ImageKey = "DistortionsToolstrip/CephalometricAndDentition";
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(profileTeethWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CLINICAL_DOPPLER))
            {
                //Clinical
                StateWizard clinicalWizard = new StateWizard("Clinical and Doppler", stateWizardController, WizardType.Exam);
                clinicalWizard.TextLine1 = "Clinical";
                clinicalWizard.TextLine2 = "and Doppler";
                clinicalWizard.ImageKey = "DistortionsToolstrip/ClinicalAndDoppler";
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(clinicalWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_RADIOGRAPHY))
            {
                //CT/Radiography Wizard
                StateWizard ctWizard = new StateWizard("Clinical and Radiography", stateWizardController, WizardType.Exam);
                ctWizard.TextLine1 = "Radiography";
                ctWizard.ImageKey = "DistortionsToolstrip/ClinicalAndRadiography";
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftFossa));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightFossa));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(ctWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_MRI))
            {
                //MRI Wizard
                StateWizard mriWizard = new StateWizard("Clinical and MRI", stateWizardController, WizardType.Exam);
                mriWizard.TextLine1 = "MRI";
                mriWizard.ImageKey = "DistortionsToolstrip/ClinicalAndMRI";
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftFossa));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightFossa));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(mriWizard);
            }
        }
    }
}