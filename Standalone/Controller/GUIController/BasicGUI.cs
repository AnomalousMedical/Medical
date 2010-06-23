using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;
using MyGUIPlugin;
using OgreWrapper;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    class BasicGUI : IDisposable
    {
        private ScreenLayoutManager screenLayoutManager;
        private BasicRibbon basicRibbon;
        private StandaloneController standaloneController;
        private AnimatedLayoutContainer animatedContainer;
        private DistortionsGUIController distortionsController;

        public BasicGUI(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            MyGUIInterface myGui = standaloneController.MedicalController.PluginManager.getPlugin("MyGUIPlugin") as MyGUIInterface;

            Gui gui = Gui.Instance;
            gui.setVisiblePointer(false);

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Layouts", "EngineArchive", "MyGUI", true);
            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Imagesets", "EngineArchive", "MyGUI", true);

            LanguageManager.Instance.loadUserTags("core_theme_black_blue_tag.xml");
            gui.load("core_skin.xml");
            gui.load("LayersToolstrip.xml");

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.Root.SuppressLayout = true;
            basicRibbon = new BasicRibbon(gui, standaloneController);
            screenLayoutManager.Root.Top = new MyGUILayoutContainer(basicRibbon.RibbonRootWidget);
            screenLayoutManager.Root.SuppressLayout = false;

            animatedContainer = new AnimatedLayoutContainer(standaloneController.MedicalController.MainTimer);
            ScreenLayout.Root.Left = animatedContainer;

            distortionsController = new DistortionsGUIController(gui, this);
        }

        public void Dispose()
        {
            distortionsController.Dispose();
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            basicRibbon.Dispose();
        }

        public void changeLeftPanel(MyGUILayoutContainer leftContainer)
        {
            if (leftContainer != null)
            {
                Widget widget = leftContainer.Widget;
                widget.Visible = true;
                LayerManager.Instance.upLayerItem(widget);
            }
            animatedContainer.changePanel(leftContainer, 0.25f, animationCompleted);
        }

        private void animationCompleted(ScreenLayoutContainer oldChild)
        {
            MyGUILayoutContainer myGUIContainer = oldChild as MyGUILayoutContainer;
            if (myGUIContainer != null)
            {
                myGUIContainer.Widget.Visible = false;
            }
        }

        void standaloneController_SceneUnloading(SimScene scene)
        {
            basicRibbon.sceneUnloading(scene);
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            basicRibbon.sceneLoaded(scene);
        }

        public ScreenLayoutManager ScreenLayout
        {
            get
            {
                return screenLayoutManager;
            }
        }
    }
}
