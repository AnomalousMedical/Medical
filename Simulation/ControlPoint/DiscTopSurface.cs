using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class DiscTopSurface : BehaviorObject
    {
        [Editable]
        String boneBaseName;

        [Editable]
        float boneStartOffset = -0.1f;

        [Editable]
        float boneDelta = 0.013f;

        [Editable]
        String followSimObjectName = "Skull";

        [DoNotCopy]
        [DoNotSave]
        List<DiscBonePair> bones = new List<DiscBonePair>();

        [DoNotCopy]
        [DoNotSave]
        SimObject owner;

        [DoNotCopy]
        [DoNotSave]
        Fossa fossa;

        [DoNotCopy]
        [DoNotSave]
        SimObject followSimObject;

        public DiscTopSurface()
        {

        }

        protected DiscTopSurface(LoadInfo info)
            :base(info)
        {

        }

        public void initialize(Skeleton skeleton, Fossa fossa, SimObject owner)
        {
            this.fossa = fossa;
            this.owner = owner;
            float current = boneStartOffset;
            for (int i = 1; skeleton.hasBone(boneBaseName + i); ++i)
            {
                Bone bone = skeleton.getBone(boneBaseName + i);
                bone.setManuallyControlled(true);
                bones.Add(new DiscBonePair(bone, current));
                current += boneDelta;
            }
            followSimObject = owner.getOtherSimObject(followSimObjectName);

            //if (boneBaseName == "RightEmenence")
            //{
            //    SceneNodeElement node = owner.getElement("Node") as SceneNodeElement;
            //    Entity entity = node.getNodeObject("Entity") as Entity;
            //    entity.setDisplaySkeleton(true);
            //}
        }

        public void update(float location)
        {
            foreach (DiscBonePair bone in bones)
            {
                float loc = location + bone.offset;
                if (loc < 0.0f)
                {
                    loc = 0.0f;
                }
                else if (loc > 1.0f)
                {
                    loc = 1.0f;
                }

                bone.bone.setPosition(Quaternion.quatRotate(owner.Rotation.inverse(), Quaternion.quatRotate(followSimObject.Rotation, fossa.getPosition(loc)) + followSimObject.Translation - owner.Translation));
                bone.bone.needUpdate(true);
            }
        }
    }
}
