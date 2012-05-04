using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using System.IO;
using Medical.Controller.AnomalousMvc;
using Medical.Controller.AnomalousMvc.Legacy;

namespace Medical
{
    class StartDDPluginTimelineTask : DDPluginTask
    {
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
                timelineController.ResourceProvider = new TimelineVirtualFSResourceProvider(Path.Combine(Plugin.PluginRootFolder, TimelineDirectory));
                //Have to load the timeline to know if it is fullscreen, technicly this loads it twice, but this code will be gone eventually
                Timeline start = timelineController.openTimeline(StartupTimeline);

                //Build a MvcContext
                AnomalousMvcContext context = new AnomalousMvcContext();
                context.StartupAction = "Common/Start";
                context.ShutdownAction = "Common/Shutdown";
                MvcController controller = new MvcController("Common");
                RunCommandsAction action = new RunCommandsAction("Start");
                PlayLegacyTimelineCommand playLegacyTimeline = new PlayLegacyTimelineCommand();
                playLegacyTimeline.Timeline = StartupTimeline;
                HideMainInterfaceCommand hideMainInterface = new HideMainInterfaceCommand();
                hideMainInterface.ShowSharedGui = !start.Fullscreen;
                action.addCommand(hideMainInterface);
                action.addCommand(playLegacyTimeline);
                controller.Actions.add(action);
                context.Controllers.add(controller);

                Plugin.MvcCore.startRunningContext(context);

                //timelineController.TEMP_MVC_CORE.hideMainInterface(!start.Fullscreen);
                //timelineController.startPlayback(start);
            }
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
