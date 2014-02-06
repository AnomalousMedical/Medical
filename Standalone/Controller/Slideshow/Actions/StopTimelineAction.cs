using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.SlideshowActions
{
    public class StopTimelineAction : SlideAction
    {
        [DoNotSave]
        private EditInterface editInterface;

        private String name;

        public StopTimelineAction(String name)
        {
            this.name = name;
        }

        public override EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("Stop Timeline");
            }
            return editInterface;
        }

        public override void addToController(Slide slide, MvcController controller)
        {
            RunCommandsAction action = new RunCommandsAction(name);
            setupAction(slide, action);
            controller.Actions.add(action);
        }

        public override void setupAction(Slide slide, RunCommandsAction action)
        {
            action.addCommand(new StopTimelineCommand());
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
            }
        }

        protected StopTimelineAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
