using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    public enum WizardPanels
    {
        LeftCondylarDegeneration,
        RightCondylarDegeneration,
        LeftCondylarGrowth,
        RightCondylarGrowth
    }

    public class StatePickerPanelController : IDisposable
    {
        private Dictionary<WizardPanels, StatePickerPanel> panelDictionary = new Dictionary<WizardPanels, StatePickerPanel>();
        private StatePickerUIHost uiHost;
        private MedicalController medicalController;
        private MedicalStateController stateController;
        private NavigationController navigationController;
        private LayerController layerController;
        private ImageRenderer imageRenderer;

        public StatePickerPanelController(StatePickerUIHost uiHost, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, ImageRenderer imageRenderer)
        {
            this.uiHost = uiHost;
            this.medicalController = medicalController;
            this.stateController = stateController;
            this.navigationController = navigationController;
            this.layerController = layerController;
            this.imageRenderer = imageRenderer;
        }

        public void Dispose()
        {
            foreach (StatePickerPanel panel in panelDictionary.Values)
            {
                panel.Dispose();
            }
        }

        public void addPanel(WizardPanels key, StatePickerPanel panel)
        {
            panelDictionary.Add(key, panel);
        }

        public void removePanel(WizardPanels key)
        {
            panelDictionary.Remove(key);
        }

        public StatePickerPanel getPanel(WizardPanels key)
        {
            StatePickerPanel panel;
            panelDictionary.TryGetValue(key, out panel);
            return panel;
        }

        public void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            foreach (StatePickerPanel panel in panelDictionary.Values)
            {
                panel.sceneChanged(medicalController, simScene);
            }
        }

        public StatePickerUIHost UiHost
        {
            get
            {
                return uiHost;
            }
        }

        public MedicalController MedicalController
        {
            get
            {
                return medicalController;
            }
        }

        public MedicalStateController StateController
        {
            get
            {
                return stateController;
            }
        }

        public NavigationController NavigationController
        {
            get
            {
                return navigationController;
            }
        }

        public LayerController LayerController
        {
            get
            {
                return layerController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }
    }
}
