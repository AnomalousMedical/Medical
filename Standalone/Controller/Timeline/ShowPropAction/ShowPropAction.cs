using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.ObjectManagement;
using Engine;
using Medical.GUI;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Show Prop", 128 / 255f, 0 / 255f, 255 / 255f, GUIType = typeof(ShowPropProperties))]
    class ShowPropAction : TimelineAction
    {
        private bool finished;
        private SimObjectBase simObject;
        private String propType;
        private Vector3 translation;
        private Quaternion rotation;

        public ShowPropAction()
        {
            propType = "Arrow";
            translation = Vector3.Zero;
            rotation = Quaternion.Identity;
        }

        public override void started(float timelineTime, Clock clock)
        {
            finished = false;
            makeProp();
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            destroyProp();
        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = timelineTime > StartTime + Duration;
        }

        public override void editing()
        {
            makeProp();
        }

        public override void editingCompleted()
        {
            base.editingCompleted();
            destroyProp();
        }

        public override bool Finished
        {
            get 
            {
                return finished;
            }
        }

        public String PropType
        {
            get
            {
                return propType;
            }
            set
            {
                propType = value;
                if (simObject != null)
                {
                    destroyProp();
                    makeProp();
                }
            }
        }

        public Vector3 Translation
        {
            get
            {
                return translation;
            }
            set
            {
                translation = value;
                if (simObject != null)
                {
                    simObject.updateTranslation(ref value, null);
                }
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                if (simObject != null)
                {
                    simObject.updateRotation(ref value, null);
                }
            }
        }

        private void makeProp()
        {
            simObject = TimelineController.PropFactory.createProp(propType, translation, rotation);
        }

        private void destroyProp()
        {
            simObject.destroy();
            simObject = null;
        }

        #region Saveable

        private const String PROP_TYPE = "propType";
        private const String TRANSLATION = "translation";
        private const String ROTATION = "rotation";

        protected ShowPropAction(LoadInfo info)
            :base(info)
        {
            propType = info.GetString(PROP_TYPE, "Arrow");
            translation = info.GetVector3(TRANSLATION, Vector3.Zero);
            rotation = info.GetQuaternion(ROTATION, Quaternion.Identity);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(PROP_TYPE, propType);
            info.AddValue(TRANSLATION, translation);
            info.AddValue(ROTATION, rotation);
        }

        #endregion Saveable
    }
}
