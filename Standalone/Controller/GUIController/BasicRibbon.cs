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
    class BasicRibbon : IDisposable
    {
        private LayerGUIController layerGUIController;
        private MandibleGUIController mandibleGUIController;
        private SequencesGUIController sequencesGUIController;
        private NavigationGUIController navigationGUIController;
        private RenderGUIController renderGUIController;
        private WindowGUIController windowGUIController;
        private Layout ribbon;
        private StandaloneController standaloneController;
        private BasicGUI basicGUI;

        public BasicRibbon(Gui gui, BasicGUI basicGUI, StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.basicGUI = basicGUI;

            ribbon = LayoutManager.Instance.loadLayout("Ribbon.layout");
            layerGUIController = new LayerGUIController(gui, standaloneController.LayerController);
            mandibleGUIController = new MandibleGUIController(gui, standaloneController.MedicalController);
            sequencesGUIController = new SequencesGUIController(gui, standaloneController.MovementSequenceController);
            navigationGUIController = new NavigationGUIController(ribbon.getWidget(0), standaloneController.NavigationController, standaloneController.SceneViewController, standaloneController.LayerController);
            renderGUIController = new RenderGUIController(ribbon.getWidget(0), standaloneController.SceneViewController, standaloneController.ImageRenderer);
            windowGUIController = new WindowGUIController(ribbon.getWidget(0), standaloneController);

            Button changeSceneButton = gui.findWidgetT("File/ChangeScene") as Button;
            Button openButton = gui.findWidgetT("File/Open") as Button;
            Button saveButton = gui.findWidgetT("File/Save") as Button;
            Button saveAsButton = gui.findWidgetT("File/SaveAs") as Button;
            Button quitButton = gui.findWidgetT("File/Quit") as Button;

            changeSceneButton.MouseButtonClick += new MyGUIEvent(changeSceneButton_MouseButtonClick);
            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);
            saveButton.MouseButtonClick += new MyGUIEvent(saveButton_MouseButtonClick);
            saveAsButton.MouseButtonClick += new MyGUIEvent(saveAsButton_MouseButtonClick);
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);
        }

        public void Dispose()
        {
            windowGUIController.Dispose();
            renderGUIController.Dispose();
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

        void changeSceneButton_MouseButtonClick(Widget source, EventArgs e)
        {
            basicGUI.showChooseSceneDialog();
        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            basicGUI.save();
        }

        void saveAsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            basicGUI.saveAs();
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            basicGUI.open();
        }

        void quitButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.shutdown();
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
