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
        private StateWizardPanelController distortionsController;
        private StateWizardController stateWizardController;

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

            LanguageManager.Instance.loadUserTags("core_theme_black_orange_tag.xml");
            gui.load("core_skin.xml");
            gui.load("LayersToolstrip.xml");
            gui.load("TeethButtons.xml");

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.Root.SuppressLayout = true;
            basicRibbon = new BasicRibbon(gui, standaloneController);
            screenLayoutManager.Root.Top = new MyGUILayoutContainer(basicRibbon.RibbonRootWidget);
            screenLayoutManager.Root.SuppressLayout = false;

            animatedContainer = new AnimatedLayoutContainer(standaloneController.MedicalController.MainTimer);
            ScreenLayout.Root.Left = animatedContainer;

            distortionsController = new StateWizardPanelController(gui, this);
            stateWizardController = new StateWizardController(standaloneController.TemporaryStateBlender, standaloneController.NavigationController, standaloneController.LayerController, this);

            //create a temporary wizard
            StateWizard wizard = new StateWizard("TestWizard", stateWizardController);
            wizard.addStatePanel(new ToothPanel("DistortionPanels/BottomTeethRemovalPanel.layout", gui.findWidgetT("TestBottomTeeth") as Button, this));
            wizard.addStatePanel(new ToothPanel("DistortionPanels/TopTeethRemovalPanel.layout", gui.findWidgetT("TestTopTeeth") as Button, this));
            stateWizardController.addWizard(wizard);

            Button testWizard = gui.findWidgetT("TestWizard") as Button;
            testWizard.MouseButtonClick += new MyGUIEvent(testWizard_MouseButtonClick);
        }

        void testWizard_MouseButtonClick(Widget source, EventArgs e)
        {
            stateWizardController.startWizard("TestWizard");
        }

        public void Dispose()
        {
            stateWizardController.Dispose();
            distortionsController.Dispose();
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            basicRibbon.Dispose();
        }

        public void changeLeftPanel(ScreenLayoutContainer leftContainer)
        {
            if (leftContainer != null)
            {
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            animatedContainer.changePanel(leftContainer, 0.25f, animationCompleted);
        }

        private void animationCompleted(ScreenLayoutContainer oldChild)
        {
            if (oldChild != null)
            {
                oldChild.Visible = false;
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
