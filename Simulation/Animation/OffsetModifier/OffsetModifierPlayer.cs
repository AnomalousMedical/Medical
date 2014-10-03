using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Medical
{
    public class OffsetModifierPlayer : BehaviorInterface
    {
        private static XmlSaver saver = new XmlSaver();

        private static List<OffsetModifierPlayer> editablePlayers = new List<OffsetModifierPlayer>();

        public static IEnumerable<OffsetModifierPlayer> EditablePlayers
        {
            get
            {
                return editablePlayers;
            }
        }

        [Editable]
        String targetSimObjectName = "this";

        [Editable]
        String targetFollowerName = "SimObjectFollower";

        [Editable]
        String blendDriverSimObjectName = "this";

        [Editable]
        String blendDriverElementName = "BlendDriver";

        [Editable]
        String sequenceFileName;

        [Editable]
        bool editable = true;

        [DoNotCopy]
        [DoNotSave]
        BlendDriver blendDriver;

        [DoNotCopy]
        [DoNotSave]
        SimObjectFollowerWithRotation follower;

        [DoNotCopy]
        [DoNotSave]
        OffsetModifierSequence currentSequence;

        protected override void constructed()
        {
            base.constructed();
        }

        protected override void link()
        {
            base.link();

            SimObject targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if(targetSimObject == null)
            {
                blacklist("The target SimObject {0} could not be found.", targetSimObjectName);
            }

            follower = targetSimObject.getElement(targetFollowerName) as SimObjectFollowerWithRotation;
            if(follower == null)
            {
                blacklist("The target SimObject {0} does not have a SimObjectFollowerWithRotation named {1}.", targetSimObjectName, targetFollowerName);
            }

            SimObject blendDriverSimObject = Owner.getOtherSimObject(blendDriverSimObjectName);
            if (blendDriverSimObject == null)
            {
                blacklist("The blend driver SimObject {0} could not be found.", blendDriverSimObjectName);
            }

            blendDriver = targetSimObject.getElement(blendDriverElementName) as BlendDriver;
            if (blendDriver == null)
            {
                blacklist("The blend driver SimObject {0} does not have a BlendDriver named {1}.", blendDriverSimObjectName, blendDriverElementName);
            }

            loadSequence(sequenceFileName);

            if(editable)
            {
                editablePlayers.Add(this);
            }
        }

        protected override void destroy()
        {
            if(editable)
            {
                editablePlayers.Remove(this);
            }
            blendDriver.BlendAmountChanged -= blendDriver_BlendAmountChanged;
            base.destroy();
        }

        /// <summary>
        /// Load a sequence from the Virtual File System. If the sequence does not exist the current sequence will be set to null.
        /// </summary>
        /// <param name="filename"></param>
        public void loadSequence(String filename)
        {
            if(filename != null && VirtualFileSystem.Instance.exists(filename))
            {
                using (var reader = new XmlTextReader(VirtualFileSystem.Instance.openStream(filename, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read)))
                {
                    Sequence = saver.restoreObject(reader) as OffsetModifierSequence;
                }
            }
            else
            {
                Sequence = null;
            }
        }

        /// <summary>
        /// Blend to a specified value.
        /// </summary>
        /// <param name="amount"></param>
        public void blend(float amount)
        {
            currentSequence.blend(amount, follower);
        }

        /// <summary>
        /// Restore the default sequence to this player.
        /// </summary>
        public void restoreDefaultSequence()
        {
            loadSequence(sequenceFileName);
        }

        public OffsetModifierSequence Sequence
        {
            get
            {
                return currentSequence;
            }
            set
            {
                //Make sure we are not subscribing multiple times
                blendDriver.BlendAmountChanged -= blendDriver_BlendAmountChanged;

                currentSequence = value;

                //Subscribe if we have a sequence
                if (currentSequence != null)
                {
                    blendDriver.BlendAmountChanged += blendDriver_BlendAmountChanged;
                    blend(blendDriver.BlendAmount);
                }
            }
        }

        public BlendDriver BlendDriver
        {
            get
            {
                return blendDriver;
            }
        }

        void blendDriver_BlendAmountChanged(BlendDriver obj)
        {
            blend(obj.BlendAmount);
        }
    }
}
