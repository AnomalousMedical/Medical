using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using Standalone;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    class PiperJBORibbon : IDisposable
    {
        private LayerGUIController layerGUIController;
        private MandibleGUIController mandibleGUIController;
        private SequencesGUIController sequencesGUIController;
        private NavigationGUIController navigationGUIController;
        private RenderGUIController renderGUIController;
        private UtilityGUIController utilityGUIController;
        private StateWizardRibbonTab wizardRibbonTab;
        private Layout ribbon;
        private StandaloneController standaloneController;
        private PiperJBOGUI piperGUI;
        private AppMenu appMenu;
        private WindowLayoutGUIController windowLayoutGUIController;

        public PiperJBORibbon(PiperJBOGUI piperGUI, StandaloneController standaloneController, StateWizardController stateWizardController)
        {
            this.standaloneController = standaloneController;
            this.piperGUI = piperGUI;

            ribbon = LayoutManager.Instance.loadLayout("Medical.Controller.GUIController.PiperJBORibbon.layout");

            Widget ribbonWidget = ribbon.getWidget(0);

            layerGUIController = new LayerGUIController(ribbonWidget, standaloneController.LayerController);
            mandibleGUIController = new MandibleGUIController(ribbonWidget, standaloneController.MedicalController);
            sequencesGUIController = new SequencesGUIController(ribbonWidget, standaloneController.MovementSequenceController);
            navigationGUIController = new NavigationGUIController(ribbonWidget, standaloneController.NavigationController, standaloneController.SceneViewController, standaloneController.LayerController);
            renderGUIController = new RenderGUIController(ribbonWidget, standaloneController.SceneViewController, standaloneController.ImageRenderer);
            utilityGUIController = new UtilityGUIController(ribbonWidget, piperGUI, standaloneController);
            wizardRibbonTab = new StateWizardRibbonTab(ribbonWidget, stateWizardController, piperGUI);

            appMenu = new AppMenu(piperGUI, standaloneController);

            Button appButton = ribbonWidget.findWidget("AppButton") as Button;
            appButton.MouseButtonClick += new MyGUIEvent(appButton_MouseButtonClick);

            windowLayoutGUIController = new WindowLayoutGUIController(ribbonWidget, standaloneController);
        }

        public void Dispose()
        {
            windowLayoutGUIController.Dispose();
            appMenu.Dispose();
            wizardRibbonTab.Dispose();
            utilityGUIController.Dispose();
            renderGUIController.Dispose();
            sequencesGUIController.Dispose();
            layerGUIController.Dispose();
            navigationGUIController.Dispose();
            LayoutManager.Instance.unloadLayout(ribbon);
        }

        public void sceneUnloading(SimScene scene)
        {
            mandibleGUIController.sceneUnloading(scene);
        }

        public void sceneLoaded(SimScene scene)
        {
            mandibleGUIController.sceneLoaded(scene);
            layerGUIController.resetMenus();
        }

        void appButton_MouseButtonClick(Widget source, EventArgs e)
        {
            appMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        public Widget RibbonRootWidget
        {
            get
            {
                return ribbon.getWidget(0);
            }
        }

        public bool AllowLayerShortcuts
        {
            get
            {
                return layerGUIController.AllowShortcuts;
            }
            set
            {
                layerGUIController.AllowShortcuts = value;
            }
        }
    }
}
