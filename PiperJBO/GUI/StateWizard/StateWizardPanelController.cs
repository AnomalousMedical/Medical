using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine.Saving.XMLSaver;
using Engine;
using Medical.Muscles;

namespace Medical.GUI
{
    public delegate StateWizardPanel CreatePanel();

    public class StateWizardPanelController : IDisposable
    {
        private Dictionary<Object, StateWizardPanel> panelDictionary = new Dictionary<Object, StateWizardPanel>();
        private Dictionary<Object, CreatePanel> panelCreationFunctions = new Dictionary<Object, CreatePanel>();
        private MedicalController medicalController;
        private MedicalStateController stateController;
        private NavigationController navigationController;
        private LayerController layerController;
        private ImageRenderer imageRenderer;
        private MovementSequenceController movementSequenceController;
        private XmlSaver saver = new XmlSaver();
        private SceneViewController sceneViewController;
        private TemporaryStateBlender stateBlender;
        private MeasurementGrid measurementGrid;

        public StateWizardPanelController(Gui gui, MedicalController medicalController, MedicalStateController stateController, NavigationController navigationController, LayerController layerController, SceneViewController sceneViewController, TemporaryStateBlender stateBlender, MovementSequenceController movementSequenceController, ImageRenderer imageRenderer, MeasurementGrid measurementGrid)
        {
            this.stateBlender = stateBlender;
            this.medicalController = medicalController;
            this.stateController = stateController;
            this.navigationController = navigationController;
            this.layerController = layerController;
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;
            this.movementSequenceController = movementSequenceController;
            this.measurementGrid = measurementGrid;
        }

        public void Dispose()
        {
            foreach (StateWizardPanel panel in panelDictionary.Values)
            {
                panel.Dispose();
            }
            panelDictionary.Clear();
        }

        public void addCreationFunction(Object key, CreatePanel createPanelCallback)
        {
            panelCreationFunctions.Add(key, createPanelCallback);
        }

        public StateWizardPanel getPanel(Object key)
        {
            StateWizardPanel panel;
            panelDictionary.TryGetValue(key, out panel);
            if (panel == null)
            {
                panel = panelCreationFunctions[key].Invoke();
                panelDictionary.Add(key, panel);
            }
            return panel;
        }

        public void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            foreach (StateWizardPanel panel in panelDictionary.Values)
            {
                panel.sceneChanged(medicalController, simScene);
                panel.setToDefault();
            }
        }

        public SceneViewWindow CurrentSceneView { get; set; }

        public XmlSaver XmlSaver
        {
            get
            {
                return saver;
            }
        }

        public TemporaryStateBlender StateBlender
        {
            get
            {
                return stateBlender;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return measurementGrid;
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

        public String CurrentWizardName { get; set; }

        internal void setNavigationState(string name)
        {
            navigationController.setNavigationState(name, CurrentSceneView);
        }

        internal void setLayerState(string name)
        {
            layerController.applyLayerState(name);
        }

        /// <summary>
        /// Change the current movement sequence. Will return the movement sequence that was playing.
        /// </summary>
        /// <param name="sequence">The sequence to play.</param>
        /// <returns>The sequence that was playing.</returns>
        internal MovementSequence changeAndPlaySequence(MovementSequence sequence)
        {
            MovementSequence retVal = movementSequenceController.CurrentSequence;
            movementSequenceController.CurrentSequence = sequence;
            movementSequenceController.playCurrentSequence();
            return retVal;
        }

        /// <summary>
        /// Stop the playback of the sequence and set the current sequence to sequence.
        /// </summary>
        /// <param name="sequence">The sequence to set but not play.</param>
        internal void resetAndStopMovementSequence(MovementSequence sequence)
        {
            movementSequenceController.stopPlayback();
            movementSequenceController.CurrentSequence = sequence;
        }

        internal MovementSequence loadSequence(String filename)
        {
            return movementSequenceController.loadSequence(filename);
        }

        internal void showChanges(StateWizardPanel panel, bool immediate)
        {
            MedicalState createdState;
            createdState = stateBlender.createBaselineState();
            panel.applyToState(createdState);
            if (immediate)
            {
                createdState.blend(1.0f, createdState);
            }
            else
            {
                stateBlender.startTemporaryBlend(createdState);
            }
        }
    }
}
