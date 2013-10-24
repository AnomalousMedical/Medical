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
    public class PlayTimelineAction : SlideAction
    {
        private RunCommandsAction action;

        [DoNotSave]
        private EditInterface editInterface;

        public PlayTimelineAction(String name)
        {
            action = new RunCommandsAction(name);
            action.addCommand(new PlayTimelineCommand()
            {
                Timeline = name + ".tl",
            });
        }

        public override EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("Play Timeline");
            }
            return editInterface;
        }

        public override void addToController(MvcController controller)
        {
            controller.Actions.add(action);
        }

        public override string Name
        {
            get
            {
                return action.Name;
            }
            set
            {
                action.Name = value;
            }
        }

        protected PlayTimelineAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
