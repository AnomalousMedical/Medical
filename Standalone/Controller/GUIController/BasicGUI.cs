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
        AnimatedLayoutContainer animatedContainer;

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

            //temp
            Button panelPopTest = gui.findWidgetT("PanelPopTest") as Button;
            panelPopTest.MouseButtonClick += new MyGUIEvent(panelPopTest_MouseButtonClick);
            panelPopTest.Caption = "аттачим";
            Button panelPopTest2 = gui.findWidgetT("PopPanel2") as Button;
            panelPopTest2.MouseButtonClick += new MyGUIEvent(panelPopTest2_MouseButtonClick);

            leftLayout = LayoutManager.Instance.loadLayout("left.layout");
            leftLayout.getWidget(0).Visible = false;
            leftLayout2 = LayoutManager.Instance.loadLayout("left2.layout");
            leftLayout2.getWidget(0).Visible = false;
        }

        Layout leftLayout;
        Layout leftLayout2;

        void panelPopTest2_MouseButtonClick(Widget source, EventArgs e)
        {
            if (leftLayout2.getWidget(0).Visible)
            {
                animatedContainer.changePanel(null, 0.25f, animationCompleted);
            }
            else
            {
                leftLayout2.getWidget(0).Visible = true;
                LayerManager.Instance.upLayerItem(leftLayout2.getWidget(0));
                animatedContainer.changePanel(new MyGUILayoutContainer(leftLayout2.getWidget(0)), 0.25f, animationCompleted);
            }
        }

        void panelPopTest_MouseButtonClick(Widget source, EventArgs e)
        {
            if (leftLayout.getWidget(0).Visible)
            {
                animatedContainer.changePanel(null, 0.25f, animationCompleted);
            }
            else
            {
                leftLayout.getWidget(0).Visible = true;
                LayerManager.Instance.upLayerItem(leftLayout.getWidget(0));
                animatedContainer.changePanel(new MyGUILayoutContainer(leftLayout.getWidget(0)), 0.25f, animationCompleted);
            }
        }

        public void animationCompleted(ScreenLayoutContainer oldChild)
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

        public void Dispose()
        {
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            basicRibbon.Dispose();
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
