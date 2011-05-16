using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using OgreWrapper;
using Engine.Platform;

namespace Medical.GUI
{
    public delegate void UIAnimationFinishedCallback();

    public class GUIManager : IDisposable
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

            //Other
            questionProvider.Dispose();
            continuePrompt.Dispose();
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            MedicalConfig.TaskbarAlignment = taskbar.Alignment;
            taskbar.Dispose();
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

            standaloneController.AtlasPluginManager.initializeGUI(this);

            innerBorderLayout = new BorderLayoutContainer();

            //Dialogs
            dialogManager = new DialogManager();
            standaloneController.AtlasPluginManager.createDialogs(dialogManager);

            //Taskbar
            taskbar = new Taskbar(appMenu, standaloneController);
            taskbar.Alignment = MedicalConfig.TaskbarAlignment;
            taskbar.SuppressLayout = true;
            standaloneController.AtlasPluginManager.addToTaskbar(taskbar);

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

            standaloneController.AtlasPluginManager.finishInitialization();
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
            standaloneController.AtlasPluginManager.setMainInterfaceEnabled(enabled);
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
            standaloneController.AtlasPluginManager.createMenus(menu);
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
            
        }

        private void standaloneController_SceneLoaded(SimScene scene)
        {
            this.changeLeftPanel(null);
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
