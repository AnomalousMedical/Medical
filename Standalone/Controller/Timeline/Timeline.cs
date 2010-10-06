using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    class TimelineActionEventArgs : EventArgs
    {
        public TimelineActionEventArgs(TimelineAction action, int index)
        {
            this.Action = action;
            this.Index = index;
            this.OldIndex = index;
        }

        public TimelineActionEventArgs(TimelineAction action, int oldIndex, int newIndex)
        {
            this.Action = action;
            this.Index = newIndex;
            this.OldIndex = oldIndex;
        }

        public TimelineAction Action { get; private set; }

        public int Index { get; set; }

        public int OldIndex { get; set; }

        public bool IndexChanged { get { return OldIndex != Index; } }
    }

    class Timeline : Saveable
    {
        public event EventHandler<TimelineActionEventArgs> ActionAdded;
        public event EventHandler<TimelineActionEventArgs> ActionRemoved;
        public event EventHandler<TimelineActionEventArgs> ActionStartTimeChanged;

        private List<TimelineAction> actions = new List<TimelineAction>();
        private List<TimelineAction> activeActions = new List<TimelineAction>();
        private List<TimelineInstantAction> preActions = new List<TimelineInstantAction>();
        private List<TimelineInstantAction> postActions = new List<TimelineInstantAction>();

        int newActionStartIndex;
        float currentTime;

        public Timeline()
        {
            
        }

        public void addPreAction(TimelineInstantAction action)
        {
            action._setTimeline(this);
            preActions.Add(action);
        }

        public void removePreAction(TimelineInstantAction action)
        {
            action._setTimeline(null);
            preActions.Remove(action);
        }

        public void addAction(TimelineAction action)
        {
            action._setTimeline(this);
            actions.Add(action);
            actions.Sort(sort);
            if (ActionAdded != null)
            {
                ActionAdded.Invoke(this, new TimelineActionEventArgs(action, actions.IndexOf(action)));
            }
        }

        public void removeAction(TimelineAction action)
        {
            action._setTimeline(null);
            actions.Remove(action);
            if (ActionRemoved != null)
            {
                ActionRemoved.Invoke(this, new TimelineActionEventArgs(action, 0));
            }
        }

        public void addPostAction(TimelineInstantAction action)
        {
            action._setTimeline(this);
            postActions.Add(action);
        }

        public void removePostAction(TimelineInstantAction action)
        {
            action._setTimeline(null);
            postActions.Remove(action);
        }

        public void start()
        {
            newActionStartIndex = 0;
            currentTime = 0.0f;
            foreach (TimelineInstantAction action in preActions)
            {
                action.doAction();
            }
        }

        public void stop()
        {
            foreach (TimelineInstantAction action in postActions)
            {
                action.doAction();
            }
        }

        public void update(Clock clock)
        {
            currentTime += clock.fSeconds;

            //Find new active actions for this time
            TimelineAction currentAction;
            for (int i = newActionStartIndex; i < actions.Count; ++i)
            {
                currentAction = actions[i];
                //If the action's start time is less than the current time, add it to the active actions.
                if (currentAction.StartTime <= currentTime)
                {
                    ++newActionStartIndex;
                    activeActions.Add(currentAction);
                    currentAction.started(currentTime, clock);
                }
                else
                {
                    //Since the list is sorted, we know that any actions after this point are not active so we can stop the scan.
                    break;
                }
            }

            //Update the active actions
            for (int i = 0; i < activeActions.Count; ++i)
            {
                currentAction = activeActions[i];
                currentAction.update(currentTime, clock);
                if (currentAction.Finished)
                {
                    //The action is finished, remove it from the action list.
                    currentAction.stopped(currentTime, clock);
                    activeActions.Remove(currentAction);
                    --i;
                }
            }
        }

        public void reset()
        {
            foreach (TimelineAction action in actions)
            {
                action.reset();
            }
        }

        public bool Finished
        {
            get
            {
                //Finished if all new actions have been scanned and the active actions is empty.
                return newActionStartIndex == actions.Count && activeActions.Count == 0;
            }
        }

        public TimelineController TimelineController { get; internal set; }

        public IEnumerable<TimelineAction> Actions { get { return actions; } }

        internal void _actionStartChanged(TimelineAction action)
        {
            int oldIndex = actions.IndexOf(action);
            actions.Sort(sort);
            int newIndex = actions.IndexOf(action);
            if (ActionStartTimeChanged != null)
            {
                ActionStartTimeChanged.Invoke(this, new TimelineActionEventArgs(action, oldIndex, newIndex));
            }
        }

        private int sort(TimelineAction x, TimelineAction y)
        {
            return x.StartTime.CompareTo(y.StartTime);
        }

        #region Saveable Members

        private static readonly String PRE_ACTIONS = "PreActions";
        private static readonly String POST_ACTIONS = "PostActions";
        private static readonly String ACTIONS = "Actions";

        protected Timeline(LoadInfo info)
        {
            info.RebuildList<TimelineInstantAction>(PRE_ACTIONS, preActions);
            info.RebuildList<TimelineInstantAction>(POST_ACTIONS, postActions);
            info.RebuildList<TimelineAction>(ACTIONS, actions);

            foreach (TimelineInstantAction action in preActions)
            {
                action._setTimeline(this);
            }

            foreach (TimelineInstantAction action in postActions)
            {
                action._setTimeline(this);
            }

            foreach (TimelineAction action in actions)
            {
                action._setTimeline(this);
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<TimelineInstantAction>(PRE_ACTIONS, preActions);
            info.ExtractList<TimelineInstantAction>(POST_ACTIONS, postActions);
            info.ExtractList<TimelineAction>(ACTIONS, actions);
        }

        #endregion
    }
}
