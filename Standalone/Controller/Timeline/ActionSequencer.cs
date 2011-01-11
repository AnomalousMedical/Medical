using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    interface ActionSequencerAction
    {
        void started(float timelineTime, Clock clock);

        void stopped(float timelineTime, Clock clock);

        void update(float timelineTime, Clock clock);

        float StartTime { get; }

        bool Finished { get; }
    }

    class ActionSequencer<T> : Saveable
        where T : ActionSequencerAction
    {
        int newActionStartIndex;
        float currentTime;
        private List<T> actions = new List<T>();
        private List<T> activeActions = new List<T>();

        public ActionSequencer()
        {

        }

        public void addAction(T action)
        {
            actions.Add(action);
            actions.Sort(sort);
        }

        public void removeAction(T action)
        {
            actions.Remove(action);
        }

        public void sort()
        {
            actions.Sort(sort);
        }

        public int indexOf(T action)
        {
            return actions.IndexOf(action);
        }

        public void start()
        {
            newActionStartIndex = 0;
            currentTime = 0.0f;
        }

        public void stop()
        {
            //Stop any running actions in case this was done during playback
            if (activeActions.Count != 0)
            {
                Clock clock = new Clock();
                foreach (ActionSequencerAction action in activeActions)
                {
                    action.stopped(currentTime, clock);
                }
                activeActions.Clear();
            }
        }

        public void update(Clock clock)
        {
            currentTime += clock.fSeconds;

            //Find new active actions for this time
            T currentAction;
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

        public bool Finished
        {
            get
            {
                //Finished if all new actions have been scanned and the active actions is empty.
                return newActionStartIndex == actions.Count && activeActions.Count == 0;
            }
        }

        public IEnumerable<T> Actions
        {
            get
            {
                return actions;
            }
        }

        public float CurrentTime
        {
            get
            {
                return currentTime;
            }
        }

        private int sort(T x, T y)
        {
            return x.StartTime.CompareTo(y.StartTime);
        }

        #region Saveable Members

        private static readonly String ACTIONS = "Actions";

        protected ActionSequencer(LoadInfo info)
        {
            info.RebuildList<T>(ACTIONS, actions);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<T>(ACTIONS, actions);
        }

        #endregion
    }
}
