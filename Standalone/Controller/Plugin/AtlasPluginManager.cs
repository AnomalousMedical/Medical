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

    public class AtlasPluginManager : IDisposable
    {
        private ScreenLayoutManager screenLayoutManager;
        private StandaloneController standaloneController;
        private HorizontalPopoutLayoutContainer leftAnimatedContainer;
        private HorizontalPopoutLayoutContainer rightAnimatedContainer;
        private VerticalPopoutLayoutContainer topAnimatedContainer;
        private VerticalPopoutLayoutContainer bottomAnimatedContainer;

        private Taskbar taskbar;
        private BorderLayoutContainer innerBorderLayout;

        //Dialogs
        private DialogManager dialogManager;
        
        //Other GUI Elements
        private MyGUIContinuePromptProvider continuePrompt;
        private MyGUIQuestionProvider questionProvider;
        private List<AtlasPlugin> plugins = new List<AtlasPlugin>();
        private AppMenu appMenu;

        //Animation callbacks
        private UIAnimationFinishedCallback leftAnimationFinished;

        public AtlasPluginManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;
        }

        public void Dispose()
        {
            //Dialogs
            dialogManager.saveDialogLayout(MedicalConfig.WindowsFile);

            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.Dispose();
            }

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
                object[] attributes = assembly.GetCustomAttributes(typeof(AtlasPluginEntryPointAttribute), true);
                if (attributes.Length > 0)
                {
                    AtlasPluginEntryPointAttribute entryPointAttribute = (AtlasPluginEntryPointAttribute)attributes[0];
                    entryPointAttribute.createPlugin(standaloneController);
                }
                else
                {
                    Log.Error("Cannot find AtlasPluginEntryPointAttribute in assembly {0}. Please add this property to the assembly.", assembly.FullName);
                }
            }
            else
            {
                Log.Error("Cannot find Assembly {0}.", fullPath);
            }
        }

        public void addPlugin(AtlasPlugin plugin)
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

            OgreResourceGroupManager.getInstance().addResourceLocation(typeof(AtlasPluginManager).AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.ScreenSizeChanged += new ScreenSizeChanged(screenLayoutManager_ScreenSizeChanged);
            

            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.initializeGUI(standaloneController, this);
            }

            innerBorderLayout = new BorderLayoutContainer();

            //Dialogs
            dialogManager = new DialogManager();
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.createDialogs(dialogManager);
            }
            
            //Taskbar
            taskbar = new Taskbar(appMenu, standaloneController);
            taskbar.Alignment = MedicalConfig.TaskbarAlignment;
            taskbar.SuppressLayout = true;
            foreach (AtlasPlugin plugin in plugins)
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

            standaloneController.ImageRenderer.ImageRendererProgress = new MyGUIImageRendererProgress();

            taskbar.SuppressLayout = false;
            taskbar.layout();

            dialogManager.loadDialogLayout(MedicalConfig.WindowsFile);

            continuePrompt = new MyGUIContinuePromptProvider();
            standaloneController.TimelineController.ContinuePrompt = continuePrompt;

            questionProvider = new MyGUIQuestionProvider(this);
            standaloneController.TimelineController.QuestionProvider = questionProvider;

            foreach (AtlasPlugin plugin in plugins)
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
            foreach (AtlasPlugin plugin in plugins)
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
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.createMenuBar(menu);
            }
        }

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
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.sceneUnloading(scene);
            }
        }

        private void standaloneController_SceneLoaded(SimScene scene)
        {
            this.changeLeftPanel(null);
            foreach (AtlasPlugin plugin in plugins)
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