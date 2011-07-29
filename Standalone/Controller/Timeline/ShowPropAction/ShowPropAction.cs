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
    [TimelineActionProperties("Show Prop")]
    public class ShowPropAction : TimelineAction
    {
        public delegate void UpdatedDelegate(float time);

        public event UpdatedDelegate Updated;

        private bool finished;
        private SimObjectBase simObject;
        private String propType;
        private Vector3 translation;
        private Quaternion rotation;

        private float fadeDuration;
        PropFadeBehavior propFade;
        private ActionSequencer<ShowPropSubAction> sequencer;

        public ShowPropAction()
        {
            propType = "Arrow";
            translation = Vector3.Zero;
            rotation = Quaternion.Identity;
            fadeDuration = 0.3f;
            sequencer = new ActionSequencer<ShowPropSubAction>();
        }

        public override void started(float timelineTime, Clock clock)
        {
            finished = false;
            makeProp();
            if (propFade != null)
            {
                propFade.hide();
                propFade.fade(1.0f, fadeDuration);
            }
            sequencer.start();
        }

        public override void skipTo(float timelineTime)
        {
            if (timelineTime <= EndTime)
            {
                finished = false;
                makeProp();
                sequencer.skipTo(timelineTime - StartTime);
            }
            else
            {
                finished = true;
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            destroyProp();
            sequencer.stop();
        }

        public override void update(float timelineTime, Clock clock)
        {
            float endTime = StartTime + Duration;
            if (propFade != null && timelineTime > endTime - fadeDuration)
            {
                propFade.fade(0.0f, fadeDuration);
                propFade = null; //Null this out, we are done with it here.
            }
            sequencer.update(clock);
            if (Updated != null)
            {
                Updated.Invoke(timelineTime - StartTime);
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

        public override void capture()
        {
            moveToPropStartPosition();
        }

        public void moveToPropStartPosition()
        {
            TimelineController.PropFactory.getInitialPosition(propType, ref translation, ref rotation);
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        public override void reverseSides()
        {
            Translation = new Vector3(-Translation.x, Translation.y, Translation.z);
            //Vector3 euler = Rotation.getEuler();
            //Rotation = new Quaternion(euler.x, -euler.y, euler.z);
            foreach (ShowPropSubAction subAction in sequencer.Actions)
            {
                subAction.reverseSides();
            }
        }

        public void addSubAction(ShowPropSubAction subAction)
        {
            subAction._setShowProp(this);
            sequencer.addAction(subAction);
        }

        public void removeSubAction(ShowPropSubAction subAction)
        {
            sequencer.removeAction(subAction);
            subAction._setShowProp(null);
        }

        public void clearSubActions()
        {
            sequencer.clearActions();
        }

        public IEnumerable<ShowPropSubAction> SubActions
        {
            get
            {
                return sequencer.Actions;
            }
        }

        public override bool Finished
        {
            get 
            {
                return finished;
            }
        }

        internal void _actionStartChanged(ShowPropSubAction showPropSubAction)
        {
            sequencer.sort();
        }

        public void _movePreviewProp(Vector3 translation, Quaternion rotation)
        {
            if (propFade != null)
            {
                propFade.changePosition(translation, rotation);
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

        public int SubActionCount
        {
            get
            {
                return sequencer.Count;
            }
        }

        internal SimObject PropSimObject
        {
            get
            {
                return simObject;
            }
        }

        private void makeProp()
        {
            simObject = TimelineController.PropFactory.createProp(propType, translation, rotation);
            if (simObject != null)
            {
                propFade = simObject.getElement(PropFactory.FadeBehaviorName) as PropFadeBehavior;
            }
        }

        private void destroyProp()
        {
            if (simObject != null)
            {
                simObject.destroy();
                simObject = null;
                propFade = null;
            }
        }

        #region Saveable

        private const String PROP_TYPE = "propType";
        private const String TRANSLATION = "translation";
        private const String ROTATION = "rotation";
        private const String FADE_DURATION = "fadeDuration";
        private const String SEQUENCER = "Sequencer";

        protected ShowPropAction(LoadInfo info)
            :base(info)
        {
            propType = info.GetString(PROP_TYPE, "Arrow");
            translation = info.GetVector3(TRANSLATION, Vector3.Zero);
            rotation = info.GetQuaternion(ROTATION, Quaternion.Identity);
            fadeDuration = info.GetFloat(FADE_DURATION, 0.3f);
            sequencer = info.GetValue<ActionSequencer<ShowPropSubAction>>(SEQUENCER, null);
            if (sequencer == null)
            {
                sequencer = new ActionSequencer<ShowPropSubAction>();
            }
            foreach (ShowPropSubAction action in sequencer.Actions)
            {
                action._setShowProp(this);
            }
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(PROP_TYPE, propType);
            info.AddValue(TRANSLATION, translation);
            info.AddValue(ROTATION, rotation);
            info.AddValue(FADE_DURATION, fadeDuration);
            info.AddValue(SEQUENCER, sequencer);
        }

        #endregion Saveable
    }
}
