using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public abstract partial class TimelineAction : ActionSequencerAction, Saveable
    {
        public event Action<TimelineAction> StartTimeChanged;
        public event Action<TimelineAction> DurationChanged;

        private Timeline timeline;
        private float startTime;
        private float duration;

        protected TimelineAction()
        {
            
        }

        internal void _setTimeline(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public void _sortAction()
        {
            timeline._actionStartChanged(this);
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

        public abstract void editing();

        public abstract void findFileReference(TimelineStaticInfo info);

        public virtual void editingCompleted()
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

        public TimelineController TimelineController
        {
            get
            {
                if (timeline == null)
                {
                    return null;
                }
                return timeline.TimelineController;
            }
        }

        #region Saveable Members

        private static readonly String START_TIME = "StartTime";
        private static readonly String DURATION = "Duration";

        protected TimelineAction(LoadInfo info)
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

    partial class TimelineAction
    {
        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = createEditInterface();
            }
            return editInterface;
        }

        protected virtual EditInterface createEditInterface()
        {
            EditInterface editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
            addRemoveCommand(editInterface);
            customizeEditInterface(editInterface);
            return editInterface;
        }

        protected virtual void customizeEditInterface(EditInterface editInterface)
        {

        }

        protected void addRemoveCommand(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Remove", (callback, caller) =>
                {
                    timeline.removeAction(this);
                }));
        }
    }
}
