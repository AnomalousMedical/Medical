using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Platform;
using Engine.ObjectManagement;
using Engine;
using Engine.Attributes;
using Engine.Editing;

namespace Medical
{
    public abstract partial class ShowPropSubAction : ActionSequencerAction, Saveable
    {
        private ShowPropAction showProp;
        private float startTime;
        private float duration;

        public event Action<ShowPropSubAction> StartTimeChanged;
        public event Action<ShowPropSubAction> DurationChanged;

        protected ShowPropSubAction()
        {
            
        }

        internal void _setShowProp(ShowPropAction showProp)
        {
            this.showProp = showProp;
        }

        public void _sortAction()
        {
            showProp._actionStartChanged(this);
        }

        /// <summary>
        /// This function will capture the state of the scene to the action. What this does is action specific and may do nothing.
        /// </summary>
        public virtual void capture()
        {

        }

        public abstract void started(float timelineTime, Clock clock);

        public abstract void skipTo(float timelineTime);

        public abstract void stopped(float timelineTime, Clock clock);

        public abstract void update(float timelineTime, Clock clock);

        public abstract void editing(PropEditController propEditController);

        public virtual void editingCompleted(PropEditController propEditController)
        {

        }

        public virtual void reverseSides()
        {
            
        }

        [EditableMinMax(0, float.MaxValue, 1)]
        public virtual float StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if (startTime != value)
                {
                    startTime = value;
                    if (StartTimeChanged != null)
                    {
                        StartTimeChanged.Invoke(this);
                    }
                    fireDataNeedsRefresh();
                }
            }
        }

        [EditableMinMax(0, float.MaxValue, 1)]
        public virtual float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    if (DurationChanged != null)
                    {
                        DurationChanged.Invoke(this);
                    }
                    fireDataNeedsRefresh();
                }
            }
        }

        public float EndTime
        {
            get
            {
                return StartTime + Duration;
            }
        }

        public abstract bool Finished { get; }

        public abstract String TypeName { get; }

        public SimObject PropSimObject
        {
            get
            {
                return showProp.PropSimObject;
            }
        }

        protected void movePreviewProp(Vector3 translation, Quaternion rotation)
        {
            if (showProp != null)
            {
                showProp._movePreviewProp(translation, rotation);
            }
        }

        #region Saveable Members

        private static readonly String START_TIME = "StartTime";
        private static readonly String DURATION = "Duration";

        protected ShowPropSubAction(LoadInfo info)
        {
            StartTime = info.GetSingle(START_TIME, 0.0f);
            Duration = info.GetSingle(DURATION, 0.0f);
        }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue(START_TIME, StartTime);
            info.AddValue(DURATION, Duration);
        }

        #endregion
    }

    partial class ShowPropSubAction
    {
        [DoNotCopy]
        [DoNotSave]
        private EditInterface editInterface;

        protected void fireDataNeedsRefresh()
        {
            editInterface.fireDataNeedsRefresh();
        }

        [DoNotCopy]
        public virtual EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, GetType().Name, null);
                    editInterface.addCommand(new EditInterfaceCommand("Remove", (callback, caller) =>
                        {
                            showProp.removeSubAction(this);
                        }));
                    customizeEditInterface(editInterface);
                }
                return editInterface;
            }
        }

        protected virtual void customizeEditInterface(EditInterface editInterface)
        {

        }
    }
}
