﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;
using MyGUIPlugin;
using OgreWrapper;
using Engine.ObjectManagement;
using Medical.Controller;

namespace Medical.GUI
{
    class BasicGUI : IDisposable
    {
        private ScreenLayoutManager screenLayoutManager;
        private BasicRibbon basicRibbon;
        private StandaloneController standaloneController;
        private LeftPopoutLayoutContainer animatedContainer;
        private StateWizardPanelController stateWizardPanelController;
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

            animatedContainer = new LeftPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            ScreenLayout.Root.Left = animatedContainer;

            stateWizardPanelController = new StateWizardPanelController(gui, standaloneController.MedicalController, standaloneController.MedicalStateController, standaloneController.NavigationController, standaloneController.LayerController, standaloneController.SceneViewController, standaloneController.TemporaryStateBlender, standaloneController.MovementSequenceController);
            stateWizardController = new StateWizardController(standaloneController.MedicalController.MainTimer, standaloneController.TemporaryStateBlender, standaloneController.NavigationController, this);

            //create a temporary wizard
            StateWizard wizard = new StateWizard("TestWizard", stateWizardController);
            wizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
            wizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
            wizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
            wizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
            wizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
            wizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
            stateWizardController.addWizard(wizard);

            Button testWizard = gui.findWidgetT("TestWizard") as Button;
            testWizard.MouseButtonClick += new MyGUIEvent(testWizard_MouseButtonClick);
        }

        void testWizard_MouseButtonClick(Widget source, EventArgs e)
        {
            stateWizardController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
            stateWizardPanelController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
            stateWizardController.startWizard("TestWizard");
        }

        public void Dispose()
        {
            stateWizardController.Dispose();
            stateWizardPanelController.Dispose();
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            basicRibbon.Dispose();
        }

        public void changeLeftPanel(LayoutContainer leftContainer)
        {
            if (leftContainer != null)
            {
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            animatedContainer.changePanel(leftContainer, 0.25f, animationCompleted);
        }

        private void animationCompleted(LayoutContainer oldChild)
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
            stateWizardPanelController.sceneChanged(standaloneController.MedicalController, scene.getDefaultSubScene().getSimElementManager<SimulationScene>());
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
