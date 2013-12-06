using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using OgreWrapper;
using Engine.Platform;
using Medical.Controller;
using Engine;
using Logging;
using System.IO;

namespace Medical.GUI
{
    public enum BorderPanelNames
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3,
        Size
    }

    public enum BorderPanelSets
    {
        Main = 0,
        EditPreview = (int)BorderPanelNames.Size,
    }

    public class GUIManager : IDisposable
    {
        public event Action<ConfigFile> SaveUIConfiguration;
        public event Action<ConfigFile> LoadUIConfiguration;

        private static String INFO_SECTION = "__Info_Section_Reserved__";
        private static String INFO_VERSION = "Version";
        private static String INFO_UISCALE = "UIScale";

        private ScreenLayoutManager screenLayoutManager;
        private StandaloneController standaloneController;

        private AnimatedLayoutContainer[] borderLayoutContainers = new AnimatedLayoutContainer[8];

        private SingleChildLayoutContainer rootContainer;
        private BorderLayoutContainer mainBorderLayout;
        private BorderLayoutContainer editorPreviewBorderLayout;
        private MDILayoutManager mdiManager;

        private bool mainGuiShowing = true;

        private bool saveWindowsOnExit = true;

        //Dialogs
        private DialogManager dialogManager;

        //Other GUI Elements
        private MyGUIContinuePromptProvider continuePrompt;
        private MyGUIImageDisplayFactory imageDisplayFactory;
        private MyGUITextDisplayFactory textDisplayFactory;
        private List<LayoutContainer> fullscreenPopups = new List<LayoutContainer>();
        private MyGUIImageRendererProgress imageRendererProgress;

        //Events
        public event Action MainGUIShown;
        public event Action MainGUIHidden;

        public GUIManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
        }

        public void Dispose()
        {
            IDisposableUtil.DisposeIfNotNull(dialogManager);

            //Containers
            foreach (AnimatedLayoutContainer container in borderLayoutContainers)
            {
                IDisposableUtil.DisposeIfNotNull(container);
            }

            //Other
			IDisposableUtil.DisposeIfNotNull(imageRendererProgress);
			IDisposableUtil.DisposeIfNotNull(continuePrompt);
        }

        public void createGUI(MDILayoutManager mdiManager)
        {
            Gui gui = Gui.Instance;
            this.mdiManager = mdiManager;

            OgreResourceGroupManager.getInstance().addResourceLocation(typeof(GUIManager).AssemblyQualifiedName, "EmbeddedScalableResource", "MyGUI", true);

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.ScreenSizeChanged += new ScreenSizeChanged(screenLayoutManager_ScreenSizeChanged);

            mainBorderLayout = new BorderLayoutContainer();
            mainBorderLayout.Center = mdiManager;

            //Dialogs
            dialogManager = new DialogManager(mdiManager);
        
            //Taskbar
            this.rootContainer = new NullLayoutContainer();
            screenLayoutManager.Root = rootContainer;
            screenLayoutManager.Root.SuppressLayout = true;
            rootContainer.Child = mainBorderLayout;

            //Outer border layout
            int panelPosition = getPanelPosition(BorderPanelNames.Top, BorderPanelSets.Main);
            borderLayoutContainers[panelPosition] = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            mainBorderLayout.Top = borderLayoutContainers[panelPosition];

            panelPosition = getPanelPosition(BorderPanelNames.Left, BorderPanelSets.Main);
            borderLayoutContainers[panelPosition] = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            mainBorderLayout.Left = borderLayoutContainers[panelPosition];

            panelPosition = getPanelPosition(BorderPanelNames.Bottom, BorderPanelSets.Main);
            borderLayoutContainers[panelPosition] = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            mainBorderLayout.Bottom = borderLayoutContainers[panelPosition];

            panelPosition = getPanelPosition(BorderPanelNames.Right, BorderPanelSets.Main);
            borderLayoutContainers[panelPosition] = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            mainBorderLayout.Right = borderLayoutContainers[panelPosition];

            //Inner border layout
            editorPreviewBorderLayout = new BorderLayoutContainer();
            panelPosition = getPanelPosition(BorderPanelNames.Top, BorderPanelSets.EditPreview);
            borderLayoutContainers[panelPosition] = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            editorPreviewBorderLayout.Top = borderLayoutContainers[panelPosition];

            panelPosition = getPanelPosition(BorderPanelNames.Left, BorderPanelSets.EditPreview);
            borderLayoutContainers[panelPosition] = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            editorPreviewBorderLayout.Left = borderLayoutContainers[panelPosition];

            panelPosition = getPanelPosition(BorderPanelNames.Bottom, BorderPanelSets.EditPreview);
            borderLayoutContainers[panelPosition] = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            editorPreviewBorderLayout.Bottom = borderLayoutContainers[panelPosition];

            panelPosition = getPanelPosition(BorderPanelNames.Right, BorderPanelSets.EditPreview);
            borderLayoutContainers[panelPosition] = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            editorPreviewBorderLayout.Right = borderLayoutContainers[panelPosition];

            mdiManager.changeCenterParent(editorPreviewBorderLayout, (center) =>
            {
                editorPreviewBorderLayout.Center = center;
            });

            imageRendererProgress = new MyGUIImageRendererProgress();
            standaloneController.ImageRenderer.ImageRendererProgress = imageRendererProgress;

            screenLayoutManager.Root.SuppressLayout = false;
            screenLayoutManager.Root.layout();

            continuePrompt = new MyGUIContinuePromptProvider();

            imageDisplayFactory = new MyGUIImageDisplayFactory(standaloneController.SceneViewController);
            textDisplayFactory = new MyGUITextDisplayFactory(standaloneController.SceneViewController);
        }

        public void pushRootContainer(SingleChildLayoutContainer root)
        {
            root.SuppressLayout = true;
            root.Child = rootContainer;
            rootContainer = root;

            screenLayoutManager.Root = rootContainer;

            root.SuppressLayout = false;
            rootContainer.invalidate();
        }

        public void removeRootContainer(SingleChildLayoutContainer root)
        {
            screenLayoutManager.Root.SuppressLayout = true;
            foreach (SingleChildLayoutContainer container in RootContainers)
            {
                if (container == root)
                {
                    if (container.ParentContainer == null)
                    {
                        screenLayoutManager.Root = container.Child;
                    }
                    else
                    {
                        SingleChildLayoutContainer parentSingleChild = (SingleChildLayoutContainer)container.ParentContainer;
                        parentSingleChild.Child = container.Child;
                    }
                }
            }
            screenLayoutManager.Root.SuppressLayout = false;
            screenLayoutManager.Root.layout();
        }

        public int getPanelPosition(BorderPanelNames name, BorderPanelSets set)
        {
            return (int)name + (int)set;
        }

        public void giveGUIsToTimelineController(TimelineController timelineController)
        {
            timelineController.ContinuePrompt = continuePrompt;
            timelineController.ImageDisplayFactory = imageDisplayFactory;
            timelineController.TextDisplayFactory = textDisplayFactory;
        }

        public void windowChanged(OSWindow newWindow)
        {
            screenLayoutManager.changeOSWindow(newWindow);
        }

        public void changePanel(BorderPanelSets set, BorderPanelNames name, LayoutContainer container, AnimationCompletedDelegate animationCompleted = null)
        {
            if (container != null)
            {
                container.Visible = true;
                container.bringToFront();
            }
            AnimatedLayoutContainer animatedContainer = borderLayoutContainers[getPanelPosition(name, set)];
            if (animatedContainer.CurrentContainer != container)
            {
                animatedContainer.changePanel(container, 0.25f, animationCompleted);
            }
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            if (mainGuiShowing != enabled)
            {
                screenLayoutManager.Root.SuppressLayout = true;
                standaloneController.AtlasPluginManager.setMainInterfaceEnabled(enabled);
                if (enabled)
                {
                    if (!screenLayoutManager.Root.Visible)
                    {
                        screenLayoutManager.Root.Visible = true;
                        dialogManager.reopenMainGUIDialogs();
                        if (MainGUIShown != null)
                        {
                            MainGUIShown.Invoke();
                        }
                    }
                }
                else
                {
                    screenLayoutManager.Root.Visible = false;
                    dialogManager.closeMainGUIDialogs();
                    if (MainGUIHidden != null)
                    {
                        MainGUIHidden.Invoke();
                    }
                }
                mainGuiShowing = enabled;
                screenLayoutManager.Root.SuppressLayout = false;
                screenLayoutManager.Root.layout();
            }
        }

        public void addManagedDialog(Dialog dialog)
        {
            dialogManager.addManagedDialog(dialog);
        }

        public void addManagedDialog(MDIDialog dialog)
        {
            dialogManager.addManagedDialog(dialog);
        }

        public void removeManagedDialog(MDIDialog dialog)
        {
            dialogManager.removeManagedDialog(dialog);
        }

        public void addFullscreenPopup(LayoutContainer popup)
        {
            popup.Location = new IntVector2((int)mainBorderLayout.Location.x, (int)mainBorderLayout.Location.y);
            popup.WorkingSize = new IntSize2((int)mainBorderLayout.WorkingSize.Width, (int)mainBorderLayout.WorkingSize.Height);
            popup.layout();
            fullscreenPopups.Add(popup);
        }

        public void removeFullscreenPopup(LayoutContainer popup)
        {
            fullscreenPopups.Remove(popup);
        }

        public void autoDisposeDialog(MDIDialog autoDisposeDialog)
        {
            dialogManager.autoDisposeDialog(autoDisposeDialog);
        }

        /// <summary>
        /// Delete the windows file and tell the manager to not write a new one on close.
        /// </summary>
        /// <returns>True if the file was deleted. False otherwise.</returns>
        public bool deleteWindowsFile()
        {
            if (MedicalConfig.WindowsFile != null)
            {
                try
                {
                    File.Delete(MedicalConfig.WindowsFile);
                    saveWindowsOnExit = false;
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error("Could not delete windows file. Reason: {0}", ex.Message);
                }
            }
            return false;
        }

        internal void loadSavedUI(ConfigFile configFile)
        {
            ConfigSection infoSection = configFile.createOrRetrieveConfigSection(INFO_SECTION);
            String versionString = infoSection.getValue(INFO_VERSION, "0.0.0.0");
            float uiScale = infoSection.getValue(INFO_UISCALE, ScaleHelper.ScaleFactor);
            Version version;
            try
            {
                version = new Version(versionString);
            }
            catch (Exception)
            {
                version = new Version("0.0.0.0");
            }
            if (version > new Version("1.0.0.2818"))
            {
                if (uiScale.EpsilonEquals(ScaleHelper.ScaleFactor, 1e-3f)) //Don't load dialog positions if the scales do not match
                {
                    dialogManager.loadDialogLayout(configFile);
                }
            }
            if (LoadUIConfiguration != null)
            {
                LoadUIConfiguration.Invoke(configFile);
            }
        }

        public void saveUI(ConfigFile configFile)
        {
            //Dialogs
            if (saveWindowsOnExit)
            {
                ConfigSection infoSection = configFile.createOrRetrieveConfigSection(INFO_SECTION);
                infoSection.setValue(INFO_VERSION, this.GetType().Assembly.GetName().Version.ToString());
                infoSection.setValue(INFO_UISCALE, ScaleHelper.ScaleFactor);
                if (SaveUIConfiguration != null)
                {
                    SaveUIConfiguration.Invoke(configFile);
                }
                dialogManager.saveDialogLayout(configFile);
            }
        }

        private void screenLayoutManager_ScreenSizeChanged(int width, int height)
        {
            dialogManager.windowResized();
            continuePrompt.ensureVisible();
            int xPos = (int)mainBorderLayout.Location.x;
            int yPos = (int)mainBorderLayout.Location.y;
            int innerWidth = (int)mainBorderLayout.WorkingSize.Width;
            int innerHeight = (int)mainBorderLayout.WorkingSize.Height;
            foreach (LayoutContainer fullscreenPopup in fullscreenPopups)
            {
                fullscreenPopup.Location = new IntVector2(xPos, yPos);
                fullscreenPopup.WorkingSize = new IntSize2(innerWidth, innerHeight);
                fullscreenPopup.layout();
            }
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

        public void layout()
        {
            screenLayoutManager.Root.layout();
        }

        private IEnumerable<SingleChildLayoutContainer> RootContainers
        {
            get
            {
                SingleChildLayoutContainer container = screenLayoutManager.Root as SingleChildLayoutContainer;
                while (container != null)
                {
                    yield return container;
                    container = container.Child as SingleChildLayoutContainer;
                }
            }
        }
    }
}
