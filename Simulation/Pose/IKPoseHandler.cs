using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using Medical.Pose.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose
{
    class IKPoseHandler : BehaviorInterface, PoseHandler
    {
        [Editable]
        private String boneSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [DoNotCopy]
        [DoNotSave]
        private Dictionary<String, PoseCommand> modePoseCommands = new Dictionary<string, PoseCommand>(); //Dictionary of pose commands that only run when the mode is active

        [DoNotCopy]
        [DoNotSave]
        private BEPUikBone bone;

        protected override void link()
        {
            base.link();

            SimObject boneSimObject = Owner.getOtherSimObject(boneSimObjectName);
            if (boneSimObject == null)
            {
                blacklist("Cannot find Bone SimObject named '{0}'", boneSimObjectName);
            }

            bone = boneSimObject.getElement(boneName) as BEPUikBone;
            if (bone == null)
            {
                blacklist("Cannot find BEPUikBone '{0}' in Bone SimObject '{1}'", boneName, boneSimObjectName);
            }
        }

        public void addPoseCommandAction(PoseCommandAction action, String mode)
        {
            PoseCommand command;
            if (!modePoseCommands.TryGetValue(mode, out command))
            {
                command = new PoseCommand();
                modePoseCommands.Add(mode, command);
            }
            command.addAction(action);
        }

        public void removePoseCommandAction(PoseCommandAction action, String mode)
        {
            PoseCommand command;
            if (modePoseCommands.TryGetValue(mode, out command))
            {
                command.removeAction(action);
                if(command.IsEmpty)
                {
                    modePoseCommands.Remove(mode);
                }
            }
        }

        public void posingStarted(IEnumerable<String> modes)
        {
            foreach (String mode in modes)
            {
                PoseCommand command;
                if(modePoseCommands.TryGetValue(mode, out command))
                {
                    command.posingStarted();
                }
            }
        }

        public void posingEnded(IEnumerable<String> modes)
        {
            foreach (String mode in modes)
            {
                PoseCommand command;
                if(modePoseCommands.TryGetValue(mode, out command))
                {
                    command.posingEnded();
                }
            }
        }

        public BEPUikBone Bone
        {
            get
            {
                return bone;
            }
        }
    }
}
