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
        private Layout ribbon;
        private StandaloneController standaloneController;

        public BasicRibbon(Gui gui, StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;

            ribbon = LayoutManager.Instance.loadLayout("Ribbon.layout");
            layerGUIController = new LayerGUIController(gui, standaloneController.LayerController);
            mandibleGUIController = new MandibleGUIController(gui, standaloneController.MedicalController);
            sequencesGUIController = new SequencesGUIController(gui, standaloneController.MovementSequenceController);

            Button quitButton = gui.findWidgetT("File/Quit") as Button;
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);
        }

        public void Dispose()
        {
            layerGUIController.Dispose();
            LayoutManager.Instance.unloadLayout(ribbon);
        }

        public void sceneUnloading(SimScene scene)
        {
            mandibleGUIController.sceneUnloading(scene);
        }

        public void sceneLoaded(SimScene scene)
        {
            mandibleGUIController.sceneLoaded(scene);
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
    }
}
