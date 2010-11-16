using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Standalone;
using Medical.Controller;
using Engine.Saving.XMLSaver;
using Engine;
using System.IO;
using System.Xml;
using Logging;
using SoundPlugin;

namespace Medical
{
    delegate void TimeTickEvent(float currentTime);

    class TimelineController : UpdateListener
    {
        public event EventHandler ResourceLocationChanged;
        public event EventHandler PlaybackStarted;
        public event EventHandler PlaybackStopped;
        public event TimeTickEvent TimeTicked; //Called on every update of the TimelineController

        private XmlSaver xmlSaver = new XmlSaver();
        private Timeline activeTimeline;
        private Timeline editingTimeline;
        private Timeline queuedTimeline;
        private UpdateTimer mainTimer;
        private StandaloneController standaloneController;
        private bool updating = false;
        private bool playPrePostActions = true;
        private String resourceLocation = null;

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
            filename = Path.Combine(ResourceLocation, filename);
            //Look on the virtual file system first. If it is not found there search the real file system.
            VirtualFileSystem vfs = VirtualFileSystem.Instance;
            if (vfs.exists(filename))
            {
                using (XmlTextReader file = new XmlTextReader(vfs.openStream(filename, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read)))
                {
                    return xmlSaver.restoreObject(file) as Timeline;
                }
            }
            else
            {
                using (XmlTextReader file = new XmlTextReader(filename))
                {
                    return xmlSaver.restoreObject(file) as Timeline;
                }
            }
        }

        public void saveTimeline(Timeline timeline, String filename)
        {
            filename = Path.Combine(ResourceLocation, filename);
            using (XmlTextWriter writer = new XmlTextWriter(filename, Encoding.Default))
            {
                writer.Formatting = Formatting.Indented;
                xmlSaver.saveObject(timeline, writer);
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
                if (PlaybackStarted != null)
                {
                    PlaybackStarted.Invoke(this, EventArgs.Empty);
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
                if (PlaybackStopped != null)
                {
                    PlaybackStopped.Invoke(this, EventArgs.Empty);
                }
                activeTimeline.stop(playPostActions);
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
                Stream soundStream = new FileStream(Path.Combine(ResourceLocation, soundFile), FileMode.Open, FileAccess.Read);
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
                Stream soundStream = new FileStream(Path.Combine(ResourceLocation, soundFile), FileMode.Open, FileAccess.Read);
                return SoundPluginInterface.Instance.SoundManager.getDuration(soundStream);
            }
            catch (Exception e)
            {
                Log.Warning("Could not load sound {0} because {1}.", soundFile, e.Message);
            }
            return 0.0;
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

        /// <summary>
        /// List the files in the current resource location that match pattern.
        /// </summary>
        /// <param name="pattern"></param>
        public String[] listResourceFiles(String pattern)
        {
            if (ResourceLocation != null)
            {
                try
                {
                    return Directory.GetFiles(ResourceLocation, pattern, SearchOption.TopDirectoryOnly);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not list files in directory {0}.\nReason: {1}", ResourceLocation, ex.Message);
                }
            }
            return new String[0];
        }

        /// <summary>
        /// Import a file into the current ResourceLocation.
        /// </summary>
        /// <param name="path"></param>
        public void importFile(String path)
        {
            String fileName = Path.GetFileName(path);
            String newPath = Path.Combine(ResourceLocation, fileName);
            File.Copy(path, newPath, true);
        }

        public bool resourceExists(String filename)
        {
            return File.Exists(Path.Combine(ResourceLocation, filename));
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

        public ITimelineFileBrowser FileBrowser { get; set; }

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

        public ContinuePromptProvider ContinuePrompt { get; set; }

        /// <summary>
        /// The current directory to read external resources out of.
        /// </summary>
        public String ResourceLocation
        {
            get
            {
                return resourceLocation;
            }
            set
            {
                resourceLocation = value;
                if (ResourceLocationChanged != null)
                {
                    ResourceLocationChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
