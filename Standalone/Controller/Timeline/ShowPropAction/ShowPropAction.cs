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

        private float fadeDuration;
        PropFadeBehavior propFade;

        public ShowPropAction()
        {
            propType = "Arrow";
            translation = Vector3.Zero;
            rotation = Quaternion.Identity;
            fadeDuration = 0.3f;
        }

        public override void started(float timelineTime, Clock clock)
        {
            finished = false;
            makeProp();
            propFade = simObject.getElement(PropFactory.FadeBehaviorName) as PropFadeBehavior;
            if (propFade != null)
            {
                propFade.hide();
                propFade.fade(1.0f, fadeDuration);
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            destroyProp();
        }

        public override void update(float timelineTime, Clock clock)
        {
            float endTime = StartTime + Duration;
            if (propFade != null && timelineTime > endTime - fadeDuration)
            {
                propFade.fade(0.0f, fadeDuration);
                propFade = null; //Null this out, we are done with it here.
            }
            finished = timelineTime > endTime;
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

        public float FadeDuration
        {
            get
            {
                return fadeDuration;
            }
            set
            {
                fadeDuration = value;
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
            propFade = null;
        }

        #region Saveable

        private const String PROP_TYPE = "propType";
        private const String TRANSLATION = "translation";
        private const String ROTATION = "rotation";
        private const String FADE_DURATION = "fadeDuration";

        protected ShowPropAction(LoadInfo info)
            :base(info)
        {
            propType = info.GetString(PROP_TYPE, "Arrow");
            translation = info.GetVector3(TRANSLATION, Vector3.Zero);
            rotation = info.GetQuaternion(ROTATION, Quaternion.Identity);
            fadeDuration = info.GetFloat(FADE_DURATION, 0.3f);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(PROP_TYPE, propType);
            info.AddValue(TRANSLATION, translation);
            info.AddValue(ROTATION, rotation);
            info.AddValue(FADE_DURATION, fadeDuration);
        }

        #endregion Saveable
    }
}
