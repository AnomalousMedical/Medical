using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class Timeline
    {
        private List<TimelineAction> actions = new List<TimelineAction>();
        private List<TimelineAction> activeActions = new List<TimelineAction>();

        int newActionStartIndex;
        float currentTime;

        public Timeline()
        {
            
        }

        public void addAction(TimelineAction action)
        {
            actions.Add(action);
            actions.Sort(sort);
        }

        public void start()
        {
            newActionStartIndex = 0;
            currentTime = 0.0f;
        }

        public void stop()
        {

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

        public bool Finished
        {
            get
            {
                //Finished if all new actions have been scanned and the active actions is empty.
                return newActionStartIndex == actions.Count && activeActions.Count == 0;
            }
        }

        private int sort(TimelineAction x, TimelineAction y)
        {
            return x.StartTime.CompareTo(y.StartTime);
        }
    }
}
