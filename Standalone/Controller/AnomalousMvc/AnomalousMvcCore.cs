using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.Platform;
using Engine.Saving.XMLSaver;
using System.Xml;

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
        private ViewHost viewHost;
        private GUIManager guiManager;

        private XmlSaver xmlSaver = new XmlSaver();

        public event Action TimelineStopped;

        public AnomalousMvcCore(UpdateTimer updateTimer, GUIManager guiManager, TimelineController timelineController, ViewHostFactory viewHostFactory)
        {
            this.timelineController = timelineController;
            timelineController.TimelinePlaybackStopped += new EventHandler(timelineController_TimelinePlaybackStopped);
            this.guiManager = guiManager;
            this.viewHostFactory = viewHostFactory;

            viewHostManager = new ViewHostManager(updateTimer, guiManager);
        }

        public void showView(View view, AnomalousMvcContext context)
        {
            viewHost = viewHostFactory.createViewHost(view, context);
            viewHostManager.requestOpen(viewHost);
        }

        public void closeView()
        {
            if (viewHost != null)
            {
                viewHostManager.requestClose(viewHost);
                viewHost = null;
            }
        }

        public bool HasOpenViews
        {
            get
            {
                return viewHost != null;
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

        public String getFullPath(String file)
        {
            return timelineController.ResourceProvider.getFullFilePath(file);
        }

        public void applyLayers(EditableLayerState layers)
        {
            if (layers != null)
            {
                layers.apply();
            }
        }

        public void applyPresetState(PresetState presetState, float duration)
        {
            TemporaryStateBlender stateBlender = timelineController.StateBlender;
            MedicalState createdState;
            createdState = stateBlender.createBaselineState();
            presetState.applyToState(createdState);
            stateBlender.startTemporaryBlend(createdState);
        }

        public void applyCameraPosition(CameraPosition cameraPosition)
        {
            SceneViewWindow window = timelineController.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(cameraPosition);
            }
        }

        public AnomalousMvcContext loadContext(String fileName)
        {
            using (XmlReader xmlReader = new XmlTextReader(getFullPath(fileName)))
            {
                AnomalousMvcContext context = (AnomalousMvcContext)xmlSaver.restoreObject(xmlReader);
                context._setCore(this);
                return context;
            }
        }

        public void startRunningContext(AnomalousMvcContext context)
        {
            context._setCore(this);
            //hideMainInterface(false); //need way to show shared interface !timeline.Fullscreen
            context.runAction(context.StartupAction); //Need to make this configurable
            this.CurrentContext = context;
        }

        public void hideMainInterface(bool showSharedInterface)
        {
            guiManager.setMainInterfaceEnabled(false, showSharedInterface);
        }

        public void showMainInterface()
        {
            guiManager.setMainInterfaceEnabled(true, false);
        }

        public void shutdownContext()
        {
            if (!runningLegacyMode)
            {
                timelineController.stopPlayback(false);
                showMainInterface();
            }
        }

        public AnomalousMvcContext CurrentContext { get; private set; }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            if (TimelineStopped != null && !timelineController.HasQueuedTimeline)
            {
                TimelineStopped.Invoke();
            }
        }

        //----------------LEGACY TIMELINE CONTROLLER STUFF------------------

        bool runningLegacyMode = false;

        /// <summary>
        /// This method will put the core into a mode to handle legacy timeline
        /// playback. This way it will responst to the
        /// LEGACY_MultiTimelineStopped event and shutdown like the
        /// timelinecontroller used to. This keeps old timelines working.
        /// </summary>
        internal void setLegacyTimelineMode()
        {
            timelineController.LEGACY_MultiTimelineStoppedEvent += timelineController_LEGACY_MultiTimelineStoppedEvent;
            runningLegacyMode = true;
        }

        void timelineController_LEGACY_MultiTimelineStoppedEvent(object sender, EventArgs e)
        {
            //This is the legacy way to deal with old timelines until they can be replaced with new ones
            guiManager.setMainInterfaceEnabled(true, false);
            timelineController.ResourceProvider = null;
            timelineController.LEGACY_MultiTimelineStoppedEvent -= timelineController_LEGACY_MultiTimelineStoppedEvent;
            runningLegacyMode = false;
        }
    }
}
