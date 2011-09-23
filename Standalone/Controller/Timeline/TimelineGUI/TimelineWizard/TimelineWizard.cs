using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Medical.Controller;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;
using System.Drawing;

namespace Medical.GUI
{
    /// <summary>
    /// This class allows a collection of TimelineWizardPanels to act as a wizard.
    /// </summary>
    public class TimelineWizard : IDisposable
    {
        //State
        private bool wizardInterfaceShown = false;
        private StandaloneController standaloneController;
        private XmlSaver xmlSaver = new XmlSaver();
        private List<TimelineEntry> timelines = new List<TimelineEntry>();
        private int currentTimeline = 0;

        //Startup options
        Vector3 cameraPosition;
        Vector3 cameraLookAt;
        LayerState layers;
        TemporaryStateBlender stateBlender;

        //Controllers
        private ImageRenderer imageRenderer;

        public TimelineWizard(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            this.stateBlender = standaloneController.TemporaryStateBlender;
            this.imageRenderer = standaloneController.ImageRenderer;
        }

        public void Dispose()
        {
            
        }

        public void addTimeline(TimelineEntry timeline)
        {
            timelines.Add(timeline);
        }

        public void clearTimelines()
        {
            timelines.Clear();
            currentTimeline = 0;
        }

        /// <summary>
        /// Called by TimelineWizardPanels when their action instructs them to
        /// show themselves. Puts the panel under management of this
        /// TimelineWizard.
        /// </summary>
        /// <param name="panel">The panel to show.</param>
        public void show(TimelineWizardPanel panel)
        {
            //Set panel scene properties
            MedicalController medicalController = standaloneController.MedicalController;
            SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
            SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
            panel.opening(medicalController, medicalScene);

            //Show panel
            if (!wizardInterfaceShown) //If this is false no interfaces have been shown yet for this wizard.
            {
                wizardInterfaceShown = true;
                //Store scene settings
                resetNotes();
                SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
                if (window != null)
                {
                    cameraPosition = window.Translation;
                    cameraLookAt = window.LookAt;
                }
                layers = new LayerState("");
                layers.captureState();
                stateBlender.recordUndoState();
            }
        }

        /// <summary>
        /// Finish the wizard.
        /// </summary>
        public void finish()
        {
            if (wizardInterfaceShown)
            {
                wizardInterfaceShown = false;
                restoreCameraAndLayers();

                //Create state
                stateBlender.forceFinishBlend();
                MedicalState createdState = stateBlender.createBaselineState();

                applyNotes(createdState);

                standaloneController.MedicalStateController.addState(createdState);
            }
        }

        /// <summary>
        /// Cancel the wizard.
        /// </summary>
        public void cancel()
        {
            if (wizardInterfaceShown)
            {
                wizardInterfaceShown = false;
                restoreCameraAndLayers();
                stateBlender.blendToUndo();
            }
        }

        public void applyPresetState(PresetState presetState)
        {
            MedicalState createdState;
            createdState = stateBlender.createBaselineState();
            presetState.applyToState(createdState);
            stateBlender.startTemporaryBlend(createdState);
        }

        public String CurrentTimeline
        {
            get
            {
                if (currentTimeline < timelines.Count)
                {
                    return timelines[currentTimeline].Timeline;
                }
                return null;
            }
            set
            {
                currentTimeline = 0;
                foreach (TimelineEntry timeline in timelines)
                {
                    if (timeline.Timeline == value)
                    {
                        break;
                    }
                    ++currentTimeline;
                }
            }
        }

        public String PreviousTimeline
        {
            get
            {
                if (currentTimeline > 0)
                {
                    return timelines[currentTimeline - 1].Timeline;
                }
                return null;
            }
        }

        public String NextTimeline
        {
            get
            {
                if (currentTimeline + 1 < timelines.Count)
                {
                    return timelines[currentTimeline + 1].Timeline;
                }
                return null;
            }
        }

        public XmlSaver Saver
        {
            get
            {
                return xmlSaver;
            }
        }

        public TemporaryStateBlender StateBlender
        {
            get
            {
                return stateBlender;
            }
        }

        public SceneViewController SceneViewController
        {
            get
            {
                return standaloneController.SceneViewController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }

        public AnatomyController AnatomyController
        {
            get
            {
                return standaloneController.AnatomyController;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return standaloneController.MeasurementGrid;
            }
        }

        public bool WizardInterfaceShown
        {
            get
            {
                return wizardInterfaceShown;
            }
        }

        public String DataSource { get; set; }

        public String Notes { get; set; }

        public String StateName { get; set; }

        public DateTime ProcedureDate { get; set; }

        public Bitmap Thumbnail { get; set; }

        private void restoreCameraAndLayers()
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(cameraPosition, cameraLookAt);
                layers.apply();
            }
        }

        private void resetNotes()
        {
            DataSource = "Piper's JBO";
            StateName = "Custom Distortion";
            Notes = "";
            ProcedureDate = DateTime.Now;
            Thumbnail = null;
        }

        private void applyNotes(MedicalState createdState)
        {
            createdState.Notes.DataSource = DataSource;
            createdState.Notes.Notes = Notes;
            createdState.Notes.ProcedureDate = ProcedureDate;
            createdState.Name = StateName;
            createdState.Thumbnail = Thumbnail;
        }
    }
}
