using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using Medical.Pose;
using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class will search for poseables by raycasting onto an entity.
    /// </summary>
    class PoseableByAnatomyIdentifier : BehaviorInterface, PoseableIdentifier
    {
        [Editable]
        private String anatomySimObjectName = "this";

        [Editable]
        private String anatomyName = "Anatomy";

        [Editable]
        private String poseHandlerSimObjectName = "this";

        [Editable]
        private String poseHandlerName = "PoseHandler";

        [Editable]
        private String poseCommandName = null;

        [DoNotCopy]
        [DoNotSave]
        private AnatomyIdentifier anatomy;

        [DoNotCopy]
        [DoNotSave]
        private PoseHandler poseHandler;

        protected override void link()
        {
            base.link();
            SimObject entitySimObject = Owner.getOtherSimObject(anatomySimObjectName);
            if(entitySimObject == null)
            {
                blacklist("Cannot find Anatomy SimObject named '{0}'", anatomySimObjectName);
            }

            anatomy = entitySimObject.getElement(anatomyName) as AnatomyIdentifier;
            if (anatomy == null)
            {
                blacklist("Cannot find AnatomyIdentifier '{0}' on Anatomy SimObject '{1}'", anatomyName, anatomySimObjectName);
            }

            SimObject poseHandlerSimObject = Owner.getOtherSimObject(poseHandlerSimObjectName);
            if(poseHandlerSimObject == null)
            {
                blacklist("Cannot find PoseHandler SimObject named '{0}'", poseHandlerSimObjectName);
            }

            poseHandler = poseHandlerSimObject.getElement(poseHandlerName) as PoseHandler;
            if (poseHandler == null)
            {
                blacklist("Cannot find PoseHandler '{0}' in SimObject '{1}'", poseHandlerName, poseHandlerSimObjectName);
            }

            PoseableObjectsManager.add(this);
        }

        protected override void destroy()
        {
            PoseableObjectsManager.remove(this);
            base.destroy();
        }

        public bool checkCollision(Ray3 ray, ref float distanceOnRay)
        {
            return anatomy.CurrentAlpha > 0.0f && anatomy.checkCollision(ray, ref distanceOnRay);
        }

        public PoseHandler PoseHandler
        {
            get
            {
                return poseHandler;
            }
        }

        public String PoseCommandName
        {
            get
            {
                return poseCommandName;
            }
        }
    }
}
