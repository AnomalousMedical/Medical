using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Controller;
using Engine.Saving.XMLSaver;
using Engine;
using System.IO;
using System.Xml;
using Logging;
using SoundPlugin;
using ZipAccess;
using MyGUIPlugin;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;

namespace Medical
{
    public delegate void TimeTickEvent(float currentTime);

    public class TimelineController : UpdateListener
    {
        public event EventHandler TimelinePlaybackStarted; //Fired whenever an individual timeline starts playing.
        public event EventHandler TimelinePlaybackStopped; //Fired whenever an individual timeline stops playing.
        public event TimeTickEvent TimeTicked; //Called on every update of the TimelineController

        private XmlSaver xmlSaver = new XmlSaver();
        private Timeline activeTimeline;
        private Timeline queuedTimeline;
        private Timeline previousTimeline = null;
        private UpdateTimer mainTimer;
        private StandaloneController standaloneController;
        private bool updating = false;
        private bool playPrePostActions = true;
        private ResourceProvider resourceProvider = null;

        public TimelineController(StandaloneController standaloneController)
        {
            this.mainTimer = standaloneController.MedicalController.MainTimer;
            this.standaloneController = standaloneController;
        }

        public Timeline ActiveTimeline
        {
            get
            {
                return activeTimeline;
            }
        }

        public bool Playing
        {
            get
            {
                return updating;
            }
        }

        public bool HasQueuedTimeline
        {
            get
            {
                return queuedTimeline != null;
            }
        }

        public Timeline openTimeline(String filename)
        {
            if (!String.IsNullOrEmpty(filename) && resourceProvider.exists(filename))
            {
                using (XmlTextReader file = new XmlTextReader(resourceProvider.openFile(filename)))
                {
                    Timeline timeline = xmlSaver.restoreObject(file) as Timeline;
                    return timeline;
                }
            }
            return null;
        }

        public void startPlayback(Timeline timeline)
        {
            startPlayback(timeline, true);
        }

        public void startPlayback(Timeline timeline, bool playPrePostActions)
        {
            startPlayback(timeline, 0.0f, playPrePostActions);
        }

        public void startPlayback(Timeline timeline, float startTime)
        {
            startPlayback(timeline, startTime, true);
        }

        public void startPlayback(Timeline timeline, float startTime, bool playPrePostActions)
        {
            if (!updating)
            {
                if (TimelinePlaybackStarted != null)
                {
                    TimelinePlaybackStarted.Invoke(this, EventArgs.Empty);
                }
                this.playPrePostActions = playPrePostActions;
                activeTimeline = timeline;
                activeTimeline.TimelineController = this;
                if (startTime == 0.0f)
                {
                    activeTimeline.start(playPrePostActions);
                }
                else
                {
                    activeTimeline.skipTo(startTime);
                }
                mainTimer.addFixedUpdateListener(this);
                updating = true;
            }
        }

        public void stopPlayback()
        {
            stopPlayback(playPrePostActions);
        }

        public void stopPlayback(bool playPostActions)
        {
            if (updating)
            {
                activeTimeline.stop(playPostActions);
                previousTimeline = activeTimeline;
                mainTimer.removeFixedUpdateListener(this);
                activeTimeline.TimelineController = null;
                activeTimeline = null;
                updating = false;

                if (TimelinePlaybackStopped != null)
                {
                    TimelinePlaybackStopped.Invoke(this, EventArgs.Empty);
                }

                if (queuedTimeline != null)
                {
                    startPlayback(queuedTimeline, playPostActions);
                    clearQueuedTimeline();
                }
            }
        }

        /// <summary>
        /// This will stop the current timeline, without playing post actions
        /// and start the new timeline optionally playing pre and post actions
        /// on the new timeline.
        /// </summary>
        /// <param name="timeline">The new timeline to play.</param>
        /// <param name="playNewTimelinePrePostAction">True to play pre and post actions.</param>
        public void stopAndStartPlayback(Timeline timeline, bool playNewTimelinePrePostAction)
        {
            if (updating)
            {
                if (TimelinePlaybackStopped != null)
                {
                    TimelinePlaybackStopped.Invoke(this, EventArgs.Empty);
                }
                activeTimeline.stop(false);
                previousTimeline = activeTimeline;
                mainTimer.removeFixedUpdateListener(this);
                activeTimeline.TimelineController = null;
                activeTimeline = null;
                updating = false;
            }
            if (timeline != null)
            {
                startPlayback(timeline, playNewTimelinePrePostAction);
                clearQueuedTimeline();
            }
        }

        public void setAsTimelineController(Timeline timeline)
        {
            timeline.TimelineController = this;
        }

        public void queueTimeline(Timeline timeline)
        {
            queuedTimeline = timeline;
        }

        public void clearQueuedTimeline()
        {
            queuedTimeline = null;
        }

        public void openNewScene(String filename)
        {
            standaloneController.openNewScene(filename);
        }

        public void showContinuePrompt(String text, ContinuePromptCallback callback)
        {
            if (ContinuePrompt != null)
            {
                ContinuePrompt.showPrompt(text, callback);
            }
            else
            {
                throw new Exception("No Continue Prompt defined.");
            }
        }

        public Source playSound(String soundFile)
        {
            try
            {
                Stream soundStream = resourceProvider.openFile(soundFile);
                return SoundPluginInterface.Instance.SoundManager.streamPlayAndForgetSound(soundStream);
            }
            catch (Exception e)
            {
                Log.Warning("Could not load sound {0} because {1}.", soundFile, e.Message);
            }
            return null;
        }

        public double getSoundDuration(String soundFile)
        {
            try
            {
                if (resourceProvider.exists(soundFile))
                {
                    using (Stream soundStream = resourceProvider.openFile(soundFile))
                    {
                        return SoundPluginInterface.Instance.SoundManager.getDuration(soundStream);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning("Could not load sound {0} because {1}.", soundFile, e.Message);
            }
            return 0.0;
        }

        public IImageDisplay showImage(String imageName, String cameraName)
        {
            IImageDisplay imageDisplay = null;
            try
            {
                using (Stream imageStream = resourceProvider.openFile(imageName))
                {
                    imageDisplay = ImageDisplayFactory.createImageDisplay(cameraName);
                    if (imageStream != null)
                    {
                        imageDisplay.setImage(imageStream);
                    }
                    else
                    {
                        Log.Warning("Could not load image {0}.", imageName);
                    }
                    imageDisplay.show();
                    return imageDisplay;
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Could not display image {0} Reason: {1}", imageName, ex.Message);
                if (imageDisplay != null)
                {
                    imageDisplay.Dispose();
                }
                return null;
            }
        }

        internal ITextDisplay showText(String text, String cameraName)
        {
            ITextDisplay textDisplay = null;
            if (TextDisplayFactory != null)
            {
                textDisplay = TextDisplayFactory.createTextDisplay(cameraName);
                textDisplay.setText(text);
                textDisplay.show();
            }
            else
            {
                Log.Warning("Could not display text {0}. No TextDisplayFactory defined.");
            }
            return textDisplay;
        }

        public void setResourceProvider(ResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
        }

        #region UpdateListener Members

        public void exceededMaxDelta()
        {

        }

        public void loopStarting()
        {

        }

        public void sendUpdate(Clock clock)
        {
            activeTimeline.update(clock);
            if (TimeTicked != null)
            {
                TimeTicked.Invoke(activeTimeline.CurrentTime);
            }
            if (activeTimeline.Finished)
            {
                stopPlayback();
            }
        }

        #endregion

        public Timeline PreviousTimeline
        {
            get
            {
                return previousTimeline;
            }
        }

        public IImageDisplayFactory ImageDisplayFactory { get; set; }

        public ITextDisplayFactory TextDisplayFactory { get; set; }

        public GUIManager GUIManager
        {
            get
            {
                return standaloneController.GUIManager;
            }
        }

        public SceneViewController SceneViewController
        {
            get
            {
                return standaloneController.SceneViewController;
            }
        }

        public MedicalStateController MedicalStateController
        {
            get
            {
                return standaloneController.MedicalStateController;
            }
        }

        public TemporaryStateBlender StateBlender
        {
            get
            {
                return standaloneController.TemporaryStateBlender;
            }
        }

        public MovementSequenceController MovementSequenceController
        {
            get
            {
                return standaloneController.MovementSequenceController;
            }
        }

        public PropFactory PropFactory
        {
            get
            {
                return standaloneController.PropFactory;
            }
        }

        public AnomalousMvcContext MvcContext { get; set; }

        public ContinuePromptProvider ContinuePrompt { get; set; }
    }
}
