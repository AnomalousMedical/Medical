using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine;
using OgrePlugin;
using Engine.ObjectManagement;
using OgreWrapper;
using Engine.Attributes;

namespace Medical
{
    class PoseManipulator : Interface, AnimationManipulator
    {
        [Editable]
        String targetSimObject;

        [Editable]
        String targetNode;

        [Editable]
        String targetEntity;

        [Editable]
        String manualAnimationName = "ManualPoseAnim";

        [Editable]
        private float defaultPosition = 0;

        [Editable]
        private float position = 0;

        [Editable]
        private String uiName;

        [Editable]
        private String poseName;

        [DoNotCopy]
        [DoNotSave]
        VertexPoseKeyFrame manualKeyFrame;

        [DoNotCopy]
        [DoNotSave]
        AnimationState manualAnimationState;

        [DoNotCopy]
        [DoNotSave]
        private ushort poseIndex;

        [DoNotCopy]
        [DoNotSave]
        MeshPtr mesh;

        protected override void constructed()
        {
            SimObject targetObject = Owner.getOtherSimObject(targetSimObject);
            if (targetObject == null)
            {
                blacklist("Could not find Target SimObject {0}.", targetSimObject);
            }
            SceneNodeElement node = targetObject.getElement(targetNode) as SceneNodeElement;
            if (node == null)
            {
                blacklist("Could not find target SceneNodeElement {0}.", targetNode);
            }
            Entity entity = node.getNodeObject(targetEntity) as Entity;
            if (entity == null)
            {
                blacklist("Could not find Entity {0}.", targetEntity);
            }
            mesh = entity.getMesh();
            OgreWrapper.Animation anim;
            VertexAnimationTrack track;
            if (!mesh.Value.hasAnimation(manualAnimationName))
            {
                anim = mesh.Value.createAnimation(manualAnimationName, 0.0f);
                track = anim.createVertexTrack(0, VertexAnimationType.VAT_POSE);
                manualKeyFrame = track.createVertexPoseKeyFrame(0);
            }
            else
            {
                anim = mesh.Value.getAnimation(manualAnimationName);
                track = anim.getVertexTrack(1);
                manualKeyFrame = track.getKeyFrame(0) as VertexPoseKeyFrame;
            }
            //Must look this up this way to get the correct pose index.
            uint poseCount = mesh.Value.getPoseCount();
            Pose pose = null;
            for(ushort i = 0; i < poseCount; ++i)
            {
                Pose innerPose = mesh.Value.getPose(i);
                if(innerPose.getName() == poseName)
                {
                    pose = innerPose;
                    poseIndex = i;
                    break;
                }
            }
            if (pose == null)
            {
                blacklist("Cannot find pose {0}.", poseName);
            }
            manualKeyFrame.addPoseReference(poseIndex, 0.0f);
            manualAnimationState = entity.getAnimationState(manualAnimationName);
            manualAnimationState.setLength(0.0f);
            manualAnimationState.setTimePosition(0.0f);
            manualAnimationState.setEnabled(true);
            BoneManipulatorController.addBoneManipulator(this);
        }

        protected override void destroy()
        {
            if (mesh != null)
            {
                mesh.Dispose();
            }
            BoneManipulatorController.removeBoneManipulator(this);
        }

        public BoneManipulatorStateEntry createStateEntry()
        {
            return null;
        }

        [DoNotCopy]
        public float DefaultPosition
        {
            get 
            {
                return defaultPosition;
            }
        }

        [DoNotCopy]
        public float Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                manualKeyFrame.updatePoseReference(poseIndex, value);
                manualAnimationState.getParent().notifyDirty();
            }
        }

        [DoNotCopy]
        public string UIName
        {
            get 
            {
                return uiName;
            }
        }
    }
}
