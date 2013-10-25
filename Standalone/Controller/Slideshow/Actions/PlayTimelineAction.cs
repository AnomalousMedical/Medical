using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical.SlideshowActions
{
    public class PlayTimelineAction : SlideAction
    {
        public enum CustomActions
        {
            EditTimeline
        }

        [DoNotSave]
        private EditInterface editInterface;

        private String name;
        private String timelineFileName;

        public PlayTimelineAction(String name)
        {
            this.Name = name;
        }

        public override EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("Play Timeline");
                editInterface.addCommand(new EditInterfaceCommand("Edit Timeline", (callback, caller) =>
                {
                    callback.runOneWayCustomQuery(CustomActions.EditTimeline, this);
                }));
            }
            return editInterface;
        }

        public override void addToController(Slide slide, MvcController controller)
        {
            if (timelineFileName != null)
            {
                controller.Actions.add(new RunCommandsAction(name, new PlayTimelineCommand()
                {
                    Timeline = Path.Combine(slide.UniqueName, timelineFileName),
                }));
            }
        }

        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                timelineFileName = value + ".tl";
            }
        }

        public String TimelineFileName 
        { 
            get
            {
                return timelineFileName;
            }
        }

        protected PlayTimelineAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
