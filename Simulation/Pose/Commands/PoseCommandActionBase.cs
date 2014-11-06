using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    abstract class PoseCommandActionBase : BehaviorInterface, PoseCommandAction
    {
        [DoNotSave]
        private LinkedList<String> commandNames = new LinkedList<string>();

        [DoNotSave]
        private LinkedList<PoseHandlerMapping> poseHandlerMappings = new LinkedList<PoseHandlerMapping>();

        protected override void link()
        {
            base.link();

            if (commandNames.Count == 0)
            {
                blacklist("No command names specified.");
            }

            PoseCommandManager.addAction(this);
        }

        public abstract void posingEnded();

        public abstract void posingStarted();

        public IEnumerable<String> CommandNames
        {
            get
            {
                return commandNames;
            }
        }

        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            info.RebuildLinkedList("CommandName", commandNames);
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.ExtractLinkedList("CommandName", commandNames);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addSubInterface(new StringListlikeEditInterface(commandNames, "Command Names").EditInterface);
            editInterface.addSubInterface(new ReflectedListLikeEditInterface<PoseHandlerMapping>(poseHandlerMappings, "Pose Handler Mappings", () => new PoseHandlerMapping()).EditInterface);
        }
    }
}
