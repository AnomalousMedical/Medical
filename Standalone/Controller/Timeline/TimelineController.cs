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

namespace Medical
{
    public delegate void TimeTickEvent(float currentTime);

    public class TimelineController : UpdateListener
    {
        public const String INDEX_FILE_NAME = "index.tix";

        public event EventHandler ResourceLocationChanged;
        public event EventHandler PlaybackStarted;
        public event EventHandler PlaybackStopped;
        public event EventHandler TimelinePlaybackStarted; //Fired whenever an individual timeline starts playing.
        public event EventHandler TimelinePlaybackStopped; //Fired whenever an individual timeline stops playing.
        public event TimeTickEvent TimeTicked; //Called on every update of the TimelineController

        private XmlSaver xmlSaver = new XmlSaver();
        private Timeline activeTimeline;
        private Timeline editingTimeline;
        private Timeline queuedTimeline;
        private Timeline previousTimeline = null;
        private UpdateTimer mainTimer;
        private StandaloneController standaloneController;
        private bool updating = false;
        private bool playPrePostActions = true;
        private TimelineResourceProvider resourceProvider = null;
        private TimelineIndex currentIndex = null;
        private bool multiTimelinePlaybackInProgress = false;

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

        public Timeline openTimeline(String filename)
        {
            using (XmlTextReader file = new XmlTextReader(resourceProvider.openFile(filename)))
            {
                Timeline timeline = xmlSaver.restoreObject(file) as Timeline;
                timeline.SourceFile = filename;
                return timeline;
            }
        }

        public void startPlayback(Timeline timeline)
        {
            startPlayback(timeline, true);
        }

        public void startPlayback(Timeline timeline, bool playPrePostActions)
        {
            if (!updating)
            {
                this.playPrePostActions = playPrePostActions;
                activeTimeline = timeline;
                activeTimeline.TimelineController = this;
                activeTimeline.start(playPrePostActions);
                mainTimer.addFixedUpdateListener(this);
                updating = true;
                if (TimelinePlaybackStarted != null)
                {
                    TimelinePlaybackStarted.Invoke(this, EventArgs.Empty);
                }
                if (!multiTimelinePlaybackInProgress)
                {
                    multiTimelinePlaybackInProgress = true;
                    if (PlaybackStarted != null) //Alert that the multi timeline playback has started.
                    {
                        PlaybackStarted.Invoke(this, EventArgs.Empty);
                    }
                }
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
                if (TimelinePlaybackStopped != null)
                {
                    TimelinePlaybackStopped.Invoke(this, EventArgs.Empty);
                }
                activeTimeline.stop(playPostActions);
                previousTimeline = activeTimeline;
                mainTimer.removeFixedUpdateListener(this);
                if (activeTimeline != editingTimeline)
                {
                    activeTimeline.TimelineController = null;
                }
                activeTimeline = null;
                updating = false;
                if (queuedTimeline != null)
                {
                    startPlayback(queuedTimeline, playPostActions);
                    clearQueuedTimeline();
                }
            }
        }

        /// <summary>
        /// This function will fire the PlaybackStopped event. This should be
        /// called by any post actions that need to shut down the timeline.
        /// </summary>
        public void _fireMultiTimelineStopEvent()
        {
            if (multiTimelinePlaybackInProgress)
            {
                previousTimeline = null;
                multiTimelinePlaybackInProgress = false;
                if (PlaybackStopped != null)
                {
                    PlaybackStopped.Invoke(this, EventArgs.Empty);
                }
            }
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

        public void showContinuePrompt(ContinuePromptCallback callback)
        {
            if (ContinuePrompt != null)
            {
                ContinuePrompt.showPrompt(callback);
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
                using (Stream soundStream = resourceProvider.openFile(soundFile))
                {
                    return SoundPluginInterface.Instance.SoundManager.getDuration(soundStream);
                }
            }
            catch (Exception e)
            {
                Log.Warning("Could not load sound {0} because {1}.", soundFile, e.Message);
            }
            return 0.0;
        }

        public IImageDisplay showImage(String imageName)
        {
            IImageDisplay imageDisplay = null;
            try
            {
                using (Stream imageStream = resourceProvider.openFile(imageName))
                {
                    imageDisplay = ImageDisplayFactory.createImageDisplay();
                    imageDisplay.setImage(imageStream);
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

        public void promptForFile(String filterString, FileChosenCallback callback)
        {
            if (FileBrowser != null)
            {
                FileBrowser.promptForFile(filterString, callback);
            }
            else
            {
                Log.Warning("Tried to show ITimelineFileBrowser, but it is null. Nothing changed.");
            }
        }

        public void saveTimeline(Timeline timeline, String filename)
        {
            if (resourceProvider != null)
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (XmlTextWriter writer = new XmlTextWriter(memoryStream, Encoding.Default))
                        {
                            writer.Formatting = Formatting.Indented;
                            xmlSaver.saveObject(timeline, writer);
                            writer.Flush();

                            memoryStream.Seek(0, SeekOrigin.Begin);
                            resourceProvider.addStream(filename, memoryStream);
                        }
                    }
                    timeline.SourceFile = filename;
                }
                catch (Exception e)
                {
                    Log.Error("Could not save timeline because of {0}.", e.Message);
                }
            }
        }

        public void saveIndex(TimelineIndex index)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                //Save index to memory stream.
                XmlTextWriter xmlWriter = new XmlTextWriter(memStream, Encoding.Default);
                xmlWriter.Formatting = Formatting.Indented;
                xmlSaver.saveObject(index, xmlWriter);
                xmlWriter.Flush();
                memStream.Seek(0, SeekOrigin.Begin);

                //Import the stream.
                resourceProvider.addStream(INDEX_FILE_NAME, memStream);
            }
            currentIndex = index;
        }

        /// <summary>
        /// List the files in the current resource location that match pattern.
        /// </summary>
        /// <param name="pattern"></param>
        public String[] listResourceFiles(String pattern)
        {
            if (resourceProvider != null)
            {
                return resourceProvider.listFiles(pattern);
            }
            return new String[0];
        }

        /// <summary>
        /// Import a file into the current ResourceLocation.
        /// </summary>
        /// <param name="path"></param>
        public void importFile(String path)
        {
            if (path.EndsWith(".tlp"))
            {
                throw new TimelineException("Do not import Timeline Projects (.tlp) into other Timeline Projects. No changes made.");
            }
            else if (resourceProvider != null)
            {
                resourceProvider.addFile(path);
            }
        }

        public bool resourceExists(String filename)
        {
            if (resourceProvider != null)
            {
                return resourceProvider.exists(filename);
            }
            return false;
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

        /// <summary>
        /// Set a timeline as the current editing target. This will give the
        /// timeline access to the controller and will keep the controller from
        /// being nulled out if the timeline is played.
        /// </summary>
        public Timeline EditingTimeline
        {
            get
            {
                return editingTimeline;
            }
            set
            {
                if (editingTimeline != null)
                {
                    editingTimeline.TimelineController = null;
                }
                editingTimeline = value;
                if (editingTimeline != null)
                {
                    editingTimeline.TimelineController = this;
                }
            }
        }

        public Timeline PreviousTimeline
        {
            get
            {
                return previousTimeline;
            }
        }

        public TimelineIndex CurrentTimelineIndex
        {
            get
            {
                return currentIndex;
            }
        }

        public ITimelineFileBrowser FileBrowser { get; set; }

        public IImageDisplayFactory ImageDisplayFactory { get; set; }

        public SimObjectMover SimObjectMover { get; set; }

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

        public ContinuePromptProvider ContinuePrompt { get; set; }

        public IQuestionProvider QuestionProvider { get; set; }

        /// <summary>
        /// Set the resource provider. This will cause the ResourceProvider to
        /// become owned by the TimelineController. You will not be able to
        /// recovery it without it being disposed. So as soon as you set this
        /// property forget about the TimelineResourceProvider you just set.
        /// 
        /// You can clear the active provider by setting this to null.
        /// </summary>
        public TimelineResourceProvider ResourceProvider
        {
            get
            {
                return resourceProvider;
            }
            set
            {
                if (resourceProvider != null)
                {
                    resourceProvider.Dispose();
                }

                resourceProvider = value;
                currentIndex = null;
                if (resourceProvider != null)
                {

                    if (resourceProvider.exists(INDEX_FILE_NAME))
                    {
                        using (XmlTextReader file = new XmlTextReader(resourceProvider.openFile(INDEX_FILE_NAME)))
                        {
                            currentIndex = xmlSaver.restoreObject(file) as TimelineIndex;
                        }
                    }
                    else
                    {
                        //Legacy check for indexes. This can probably be removed since no real timelines have been created yet.
                        Log.Warning("Loaded timeline project with no index file. Creating default index.");

                        using (MemoryStream memStream = new MemoryStream())
                        {
                            saveIndex(new TimelineIndex());
                        }
                    }
                }

                if (ResourceLocationChanged != null)
                {
                    ResourceLocationChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
