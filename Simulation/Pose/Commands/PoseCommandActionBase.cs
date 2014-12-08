using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
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
        private List<PoseHandlerMapping> poseHandlerMappings = new List<PoseHandlerMapping>(10);

        protected override void link()
        {
            base.link();

            if (poseHandlerMappings.Count == 0)
            {
                blacklist("No pose handler mappings specified.");
            }

            foreach(var mapping in poseHandlerMappings)
            {
                SimObject mappedSimObject = Owner.getOtherSimObject(mapping.SimObjectName);
                if(mappedSimObject == null)
                {
                    blacklist("Cannot find mapped SimObject '{0}' The scene will be unstable, since this error is not handled in any way.", mapping.SimObjectName);
                }

                var poseHandler = mappedSimObject.getElement(mapping.PoseHandlerName) as PoseHandler;
                if(poseHandler == null)
                {
                    blacklist("Cannot find PoseHandler '{0}' on SimObject '{1}' The scene will be unstable, since this error is not handled in any way.", mapping.PoseHandlerName, mapping.SimObjectName);
                }

                poseHandler.addPoseCommandAction(this, mapping.Mode);
            }
        }

        protected override void destroy()
        {
            foreach (var mapping in poseHandlerMappings)
            {
                SimObject mappedSimObject = Owner.getOtherSimObject(mapping.SimObjectName);
                if (mappedSimObject != null)
                {
                    var poseHandler = mappedSimObject.getElement(mapping.PoseHandlerName) as PoseHandler;
                    if (poseHandler != null)
                    {
                        poseHandler.removePoseCommandAction(this, mapping.Mode);
                    }
                }
            }
            base.destroy();
        }

        public abstract void posingEnded();

        public abstract void posingStarted();

        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            info.RebuildList("PoseHandlerMapping", poseHandlerMappings);
            poseHandlerMappings.Capacity = poseHandlerMappings.Count;
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.ExtractList("PoseHandlerMapping", poseHandlerMappings);
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addSubInterfaceForObject(poseHandlerMappings, new ReflectedListLikeEditInterface<PoseHandlerMapping>(poseHandlerMappings, "Pose Handler Mappings", () => new PoseHandlerMapping()));
        }
    }
}
