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
using libRocketPlugin;
using OgreWrapper;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// The core is the link from AnomalousMVC to the rest of the system.
    /// </summary>
    public class AnomalousMvcCore : IDisposable
    {
        private TimelineController timelineController;
        private ViewHostFactory viewHostFactory;

        private ViewHostManager viewHostManager;
        private GUIManager guiManager;
        private StandaloneController standaloneController;
        private ActiveContextManager contextManager = new ActiveContextManager();
        private ResourceProviderRocketFSExtension currentFSExtension = null;

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

        public void Dispose()
        {
            if (contextManager.CurrentContext != null)
            {
                contextManager.CurrentContext.suspend();
                contextManager.CurrentContext.queueShutdown();
                this.processViewChanges();
                shutdownContext(contextManager.CurrentContext, true, false);
            }
            viewHostManager.Dispose();
        }

        public bool isViewOpen(String name)
        {
            return viewHostManager.isViewOpen(name);
        }

        public ViewHost findViewHost(String name)
        {
            return viewHostManager.findViewHost(name);
        }

        public void queueShowView(View view, AnomalousMvcContext context)
        {
            viewHostManager.requestOpen(view, context);
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

        public void playTimeline(String timelineName)
        {
            Timeline timeline = timelineController.openTimeline(timelineName);
            if (timeline != null)
            {
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

            if (stateInfo.ThumbInfo != null)
            {
                createdState.Thumbnail = ImageRenderer.renderImage(stateInfo.ThumbInfo);
            }

            standaloneController.MedicalStateController.addState(createdState);
        }

        public void applyCameraPosition(CameraPosition cameraPosition)
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(cameraPosition.computeTranslationWithIncludePoint(window), cameraPosition.LookAt);
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

        /// <summary>
        /// Start running an mvc context. The context should be a unique
        /// instance because its internal state will be modified as it is run.
        /// So you should load a fresh context from a file or make a copy before
        /// passing it into this method.
        /// </summary>
        /// <param name="context">The context to run.</param>
        public void startRunningContext(AnomalousMvcContext context)
        {
            if (contextManager.CurrentContext != null)
            {
                contextManager.CurrentContext.suspend();
                contextManager.CurrentContext.queueShutdown();
                this.processViewChanges();
                this.shutdownContext(contextManager.CurrentContext, false, false);
            }
            contextManager.pushContext(context);
            setupContextToRun(context);
            context.startup();
        }

        public void shutdownContext(AnomalousMvcContext context, bool removeContext, bool resumePreviousContext)
        {
            RocketGuiManager.clearAllCaches();
            RocketInterface.Instance.FileInterface.removeExtension(currentFSExtension);
            currentFSExtension = null;
            
            OgreResourceGroupManager.getInstance().removeResourceLocation(context.ResourceProvider.BackingLocation, "RocketMvc");
            OgreResourceGroupManager.getInstance().destroyResourceGroup("RocketMvc");
            OgreArchiveManager.getInstance().unload(context.ResourceProvider.BackingLocation);

            timelineController.stopPlayback(false);
            if (removeContext)
            {
                context.shutdown();
                contextManager.removeContext(context);
            }

            if (resumePreviousContext)
            {
                AnomalousMvcContext nextContext = contextManager.CurrentContext;
                if (nextContext != null)
                {
                    setupContextToRun(nextContext);
                    nextContext.resume();
                }
            }
        }

        public void hideMainInterface()
        {
            guiManager.setMainInterfaceEnabled(false);
        }

        public void showMainInterface()
        {
            guiManager.setMainInterfaceEnabled(true);
        }

        internal StoredViewCollection generateSavedViewLayout()
        {
            return viewHostManager.generateSavedViewLayout();
        }

        internal void restoreSavedViewLayout(StoredViewCollection storedViews, AnomalousMvcContext context)
        {
            viewHostManager.restoreSavedViewLayout(storedViews, context);
            processViewChanges();
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

        private void setupContextToRun(AnomalousMvcContext context)
        {
            timelineController.MvcContext = context;
            currentFSExtension = new ResourceProviderRocketFSExtension(context.ResourceProvider);
            RocketInterface.Instance.FileInterface.addExtension(currentFSExtension);

            OgreResourceGroupManager.getInstance().createResourceGroup("RocketMvc");
            OgreResourceProviderArchiveFactory.AddResourceProvider(context.ResourceProvider.BackingLocation, context.ResourceProvider);
            OgreResourceGroupManager.getInstance().addResourceLocation(context.ResourceProvider.BackingLocation, OgreResourceProviderArchiveFactory.Name, "RocketMvc", false);
            //OgreResourceGroupManager.getInstance().initializeResourceGroup("RocketMvc");

            context.Core = this;
        }
    }
}
