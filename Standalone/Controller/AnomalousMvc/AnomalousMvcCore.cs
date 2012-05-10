using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.Platform;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;
using Medical.Model;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// The core is the link from AnomalousMVC to the rest of the system.
    /// </summary>
    public class AnomalousMvcCore
    {
        private TimelineController timelineController;
        private ViewHostFactory viewHostFactory;

        private ViewHostManager viewHostManager;
        private GUIManager guiManager;
        private StandaloneController standaloneController;

        private XmlSaver xmlSaver = new XmlSaver();

        public event Action TimelineStopped;

        public AnomalousMvcCore(StandaloneController standaloneController, ViewHostFactory viewHostFactory)
        {
            this.standaloneController = standaloneController;
            this.timelineController = standaloneController.TimelineController;
            timelineController.TimelinePlaybackStopped += new EventHandler(timelineController_TimelinePlaybackStopped);
            this.guiManager = standaloneController.GUIManager;
            this.viewHostFactory = viewHostFactory;

            viewHostManager = new ViewHostManager(guiManager, viewHostFactory);
        }

        public void queueShowView(View view, AnomalousMvcContext context, ViewLocations viewLocation)
        {
            viewHostManager.requestOpen(view, context, viewLocation);
        }

        public void queueCloseView(ViewHost viewHost)
        {
            viewHostManager.requestClose(viewHost);
        }

        public void queueCloseAllViews()
        {
            viewHostManager.requestCloseAll();
        }

        public void processViewChanges()
        {
            viewHostManager.processViewChanges();
        }

        public bool HasOpenViews
        {
            get
            {
                return viewHostManager.HasOpenViews;
            }
        }

        public bool PlayingTimeline
        {
            get
            {
                return timelineController.Playing;
            }
        }

        public void stopPlayingTimeline()
        {
            if (timelineController.Playing)
            {
                timelineController.stopAndStartPlayback(null, false);
            }
        }

        public void playTimeline(String timelineName, bool allowPlaybackStop)
        {
            Timeline timeline = timelineController.openTimeline(timelineName);
            if (timeline != null)
            {
                timeline.AutoFireMultiTimelineStopped = allowPlaybackStop;
                if (timelineController.Playing)
                {
                    timelineController.stopAndStartPlayback(timeline, true);
                }
                else
                {
                    timelineController.startPlayback(timeline);
                }
            }
            else
            {
                Log.Warning("AnomalousMvcCore playback: Error loading timeline '{0}'", timelineName);
            }
        }

        public void applyLayers(LayerState layers)
        {
            if (layers != null)
            {
                layers.apply();
            }
        }

        public void applyPresetState(PresetState presetState, float duration)
        {
            TemporaryStateBlender stateBlender = standaloneController.TemporaryStateBlender;
            MedicalState createdState;
            createdState = stateBlender.createBaselineState();
            presetState.applyToState(createdState);
            stateBlender.startTemporaryBlend(createdState);
        }

        public void applyMedicalState(MedicalState medicalState)
        {
            standaloneController.TemporaryStateBlender.startTemporaryBlend(medicalState);
        }

        public MedicalState generateMedicalState()
        {
            return standaloneController.TemporaryStateBlender.createBaselineState();
        }

        internal void createMedicalState(MedicalStateInfoModel stateInfo)
        {
            standaloneController.TemporaryStateBlender.forceFinishBlend();
            MedicalState createdState = standaloneController.TemporaryStateBlender.createBaselineState();
            createdState.Notes.DataSource = stateInfo.DataSource;
            createdState.Notes.Notes = stateInfo.Notes;
            createdState.Notes.ProcedureDate = stateInfo.ProcedureDate;
            createdState.Name = stateInfo.StateName;
            standaloneController.MedicalStateController.addState(createdState);
        }

        public void applyCameraPosition(CameraPosition cameraPosition)
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(cameraPosition);
            }
        }

        public CameraPosition getCurrentCameraPosition()
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            if (window != null)
            {
                CameraPosition position = new CameraPosition();
                position.Translation = window.Translation;
                position.LookAt = window.LookAt;
                return position;
            }
            return null;
        }

        public AnomalousMvcContext loadContext(Stream stream)
        {
            using (XmlReader xmlReader = new XmlTextReader(stream))
            {
                AnomalousMvcContext context = (AnomalousMvcContext)xmlSaver.restoreObject(xmlReader);
                return context;
            }
        }

        public void startRunningContext(AnomalousMvcContext context)
        {
            timelineController.MvcContext = context;
            context.starting(this);
            context.runAction(context.StartupAction);
        }

        public void hideMainInterface(bool showSharedInterface)
        {
            guiManager.setMainInterfaceEnabled(false, showSharedInterface);
        }

        public void showMainInterface()
        {
            guiManager.setMainInterfaceEnabled(true, false);
        }

        public void shutdownContext(AnomalousMvcContext context)
        {
            timelineController.stopPlayback(false);
            context.runFinalAction(context.ShutdownAction);
        }

        public ViewHostFactory ViewHostFactory
        {
            get
            {
                return viewHostFactory;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return standaloneController.MeasurementGrid;
            }
        }

        public AnatomyController AnatomyController
        {
            get
            {
                return standaloneController.AnatomyController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return standaloneController.ImageRenderer;
            }
        }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            if (TimelineStopped != null && !timelineController.HasQueuedTimeline)
            {
                TimelineStopped.Invoke();
            }
        }
    }
}
