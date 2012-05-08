using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using System.IO;
using Medical.Controller.AnomalousMvc;

namespace Medical
{
    class StartDDPluginTimelineTask : DDPluginTask
    {
        private AnomalousMvcContext context;

        public StartDDPluginTimelineTask(String uniqueName, String name, String iconName, String category)
            :base(uniqueName, name, iconName, category)
        {
            ShowOnTaskbar = false;
        }

        public void registerPlugin(DDAtlasPlugin dataDrivenPlugin)
        {

        }

        public override void clicked(TaskPositioner positioner)
        {
            TimelineController timelineController = Plugin.TimelineController;
            if (!timelineController.MultiTimelinePlaybackInProgress)
            {
                VirtualFilesystemResourceProvider resourceProvider = new VirtualFilesystemResourceProvider(Path.Combine(Plugin.PluginRootFolder, TimelineDirectory));
                timelineController.setResourceProvider(resourceProvider);
                timelineController.LEGACY_MultiTimelineStoppedEvent += timelineController_LEGACY_MultiTimelineStoppedEvent;
                //Have to load the timeline to know if it is fullscreen, technicly this loads it twice, but this code will be gone eventually
                Timeline start = timelineController.openTimeline(StartupTimeline);

                //Build a MvcContext
                context = new AnomalousMvcContext();
                context.setResourceProvider(resourceProvider);
                context.AllowShutdown = false;
                context.StartupAction = "Common/Start";
                context.ShutdownAction = "Common/Shutdown";
                MvcController controller = new MvcController("Common");
                RunCommandsAction startAction = new RunCommandsAction("Start");
                PlayTimelineCommand playLegacyTimeline = new PlayTimelineCommand();
                playLegacyTimeline.Timeline = StartupTimeline;
                HideMainInterfaceCommand hideMainInterface = new HideMainInterfaceCommand();
                hideMainInterface.ShowSharedGui = !start.Fullscreen;
                startAction.addCommand(hideMainInterface);
                startAction.addCommand(playLegacyTimeline);
                controller.Actions.add(startAction);
                RunCommandsAction shutdownAction = new RunCommandsAction("Shutdown");
                shutdownAction.addCommand(new ShowMainInterfaceCommand());
                controller.Actions.add(shutdownAction);
                context.Controllers.add(controller);

                Plugin.MvcCore.startRunningContext(context);
            }
        }

        void timelineController_LEGACY_MultiTimelineStoppedEvent(object sender, EventArgs e)
        {
            context.AllowShutdown = true;
            context.shutdown();
            TimelineController timelineController = Plugin.TimelineController;
            timelineController.setResourceProvider(null);
            timelineController.LEGACY_MultiTimelineStoppedEvent -= timelineController_LEGACY_MultiTimelineStoppedEvent;
        }

        [Editable]
        public String TimelineDirectory { get; set; }

        [Editable]
        public String StartupTimeline { get; set; }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        #region Saveable Members

        protected StartDDPluginTimelineTask(LoadInfo info)
            :base(info)
        {
            TimelineDirectory = info.GetString("TimelineDirectory");
            StartupTimeline = info.GetString("StartupTimeline");
            ShowOnTaskbar = false;
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("TimelineDirectory", TimelineDirectory);
            info.AddValue("StartupTimeline", StartupTimeline);
        }

        #endregion
    }
}
