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
    public delegate void UIAnimationFinishedCallback();

    public class GUIManager : IDisposable
    {
        private static String INTERFACE_NAME = typeof(GUIPlugin).Name;

        private ScreenLayoutManager screenLayoutManager;
        private StandaloneController standaloneController;
        private HorizontalPopoutLayoutContainer leftAnimatedContainer;
        private HorizontalPopoutLayoutContainer rightAnimatedContainer;
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

        //Animation callbacks
        private UIAnimationFinishedCallback leftAnimationFinished;

        public GUIManager(StandaloneController standaloneController)
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
            MedicalConfig.TaskbarAlignment = taskbar.Alignment;
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

            OgreResourceGroupManager.getInstance().addResourceLocation(typeof(GUIManager).AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.ScreenSizeChanged += new ScreenSizeChanged(screenLayoutManager_ScreenSizeChanged);
            

            foreach (GUIPlugin plugin in plugins)
            {
                plugin.initializeGUI(standaloneController, this);
            }

            stateWizardPanelController = new StateWizardPanelController(gui, standaloneController.MedicalController, standaloneController.MedicalStateController, standaloneController.NavigationController, standaloneController.LayerController, standaloneController.SceneViewController, standaloneController.TemporaryStateBlender, standaloneController.MovementSequenceController, standaloneController.ImageRenderer, standaloneController.MeasurementGrid);
            stateWizardController = new StateWizardController(standaloneController.MedicalController.MainTimer, standaloneController.TemporaryStateBlender, standaloneController.NavigationController, standaloneController.LayerController, this);
            stateWizardController.StateCreated += new MedicalStateCreated(stateWizardController_StateCreated);
            stateWizardController.Finished += new StatePickerFinished(stateWizardController_Finished);

            innerBorderLayout = new BorderLayoutContainer();

            //Dialogs
            dialogManager = new DialogManager();
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.createDialogs(dialogManager);
            }
            
            //Taskbar
            taskbar = new Taskbar(appMenu, standaloneController);
            taskbar.Alignment = MedicalConfig.TaskbarAlignment;
            taskbar.SuppressLayout = true;
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.addToTaskbar(taskbar);
            }

            taskbar.Child = innerBorderLayout;
            screenLayoutManager.Root = taskbar;

            topAnimatedContainer = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Top = topAnimatedContainer;

            leftAnimatedContainer = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Left = leftAnimatedContainer;

            bottomAnimatedContainer = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Bottom = bottomAnimatedContainer;

            rightAnimatedContainer = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Right = rightAnimatedContainer;

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

            foreach (GUIPlugin plugin in plugins)
            {
                plugin.finishInitialization();
            }
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
            changeLeftPanel(leftContainer, null);
        }

        public void changeLeftPanel(LayoutContainer leftContainer, UIAnimationFinishedCallback animationFinished)
        {
            if (leftAnimationFinished != null)
            {
                leftAnimationFinished.Invoke();
            }
            leftAnimationFinished = animationFinished;
            if (leftContainer != null)
            {
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            if (leftAnimatedContainer.CurrentContainer != leftContainer)
            {
                leftAnimatedContainer.changePanel(leftContainer, 0.25f, leftAnimationCompleted);
            }
        }

        public void changeRightPanel(LayoutContainer rightContainer)
        {
            if (rightContainer != null)
            {
                rightContainer.Visible = true;
                rightContainer.bringToFront();
            }
            if (rightAnimatedContainer.CurrentContainer != rightContainer)
            {
                rightAnimatedContainer.changePanel(rightContainer, 0.25f, animationCompleted);
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

        public void createMenuBar(NativeMenuBar menu)
        {
            foreach (GUIPlugin plugin in plugins)
            {
                plugin.createMenuBar(menu);
            }
        }

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

        public StateWizardController StateWizardController
        {
            get
            {
                return stateWizardController;
            }
        }

        public StateWizardPanelController StateWizardPanelController
        {
            get
            {
                return stateWizardPanelController;
            }
        }

        private void animationCompleted(LayoutContainer oldChild)
        {
            if (oldChild != null)
            {
                oldChild.Visible = false;
            }
        }

        private void leftAnimationCompleted(LayoutContainer oldChild)
        {
            if (leftAnimationFinished != null)
            {
                leftAnimationFinished.Invoke();
                leftAnimationFinished = null;
            }
            animationCompleted(oldChild);
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

        private void screenLayoutManager_ScreenSizeChanged(int width, int height)
        {
            dialogManager.windowResized();
            continuePrompt.ensureVisible();
        }

        public event ScreenSizeChanged ScreenSizeChanged
        {
            add
            {
                screenLayoutManager.ScreenSizeChanged += value;
            }
            remove
            {
                screenLayoutManager.ScreenSizeChanged -= value;
            }
        }
    }
}