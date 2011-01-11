using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    /// <summary>
    /// The interface for actions in the ActionSequencer.
    /// </summary>
    interface ActionSequencerAction
    {
        /// <summary>
        /// Called when the action is starting up.
        /// </summary>
        /// <param name="timelineTime">The current total time for the timeline.</param>
        /// <param name="clock">The clock for the current tick.</param>
        void started(float timelineTime, Clock clock);

        /// <summary>
        /// Called when the action is complete.
        /// </summary>
        /// <param name="timelineTime">The current total time for the timeline.</param>
        /// <param name="clock">The clock for the current tick.</param>
        void stopped(float timelineTime, Clock clock);

        /// <summary>
        /// Called each tick to update the action.
        /// </summary>
        /// <param name="timelineTime">The current total time for the timeline.</param>
        /// <param name="clock">The clock for the current tick.</param>
        void update(float timelineTime, Clock clock);

        /// <summary>
        /// The start time of the action.
        /// </summary>
        float StartTime { get; }

        /// <summary>
        /// True if the action is finished.
        /// </summary>
        bool Finished { get; }
    }

    /// <summary>
    /// This class can play a series of time based actions. It will maintain the
    /// order and do all the processing.
    /// </summary>
    /// <typeparam name="T">The type of this ActionSequencer. Must extend ActionSequencerAction.</typeparam>
    class ActionSequencer<T> : Saveable
        where T : ActionSequencerAction
    {
        int newActionStartIndex;
        float currentTime;
        private List<T> actions = new List<T>();
        private List<T> activeActions = new List<T>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionSequencer()
        {

        }

        /// <summary>
        /// Add an action to the sequencer.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void addAction(T action)
        {
            actions.Add(action);
            actions.Sort(sort);
        }

        /// <summary>
        /// Remove an action from the sequencer.
        /// </summary>
        /// <param name="action">The action to remove.</param>
        public void removeAction(T action)
        {
            actions.Remove(action);
        }

        /// <summary>
        /// Sort the actions in the sequencer. This should be called if an
        /// action's start time has changed.
        /// </summary>
        public void sort()
        {
            actions.Sort(sort);
        }

        /// <summary>
        /// Get the index of a particular action.
        /// </summary>
        /// <param name="action">The action to get the index of.</param>
        /// <returns>The index of the action.</returns>
        public int indexOf(T action)
        {
            return actions.IndexOf(action);
        }

        /// <summary>
        /// Start playback of the sequencer. Must call update as well this will
        /// not automatically update. Call this before starting updates.
        /// </summary>
        public void start()
        {
            newActionStartIndex = 0;
            currentTime = 0.0f;
        }

        /// <summary>
        /// Stop playback of the sequencer. Call this after completing updates.
        /// </summary>
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

        /// <summary>
        /// Update the sequence.
        /// </summary>
        /// <param name="clock">A clock with the next time.</param>
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

        /// <summary>
        /// Returns true if the sequence is finished.
        /// </summary>
        public bool Finished
        {
            get
            {
                //Finished if all new actions have been scanned and the active actions is empty.
                return newActionStartIndex == actions.Count && activeActions.Count == 0;
            }
        }

        /// <summary>
        /// An enumerator over all actions in the sequencer.
        /// </summary>
        public IEnumerable<T> Actions
        {
            get
            {
                return actions;
            }
        }

        /// <summary>
        /// The current time of the sequencer.
        /// </summary>
        public float CurrentTime
        {
            get
            {
                return currentTime;
            }
        }

        /// <summary>
        /// Helper function to sort actions.
        /// </summary>
        /// <param name="x">Action 1.</param>
        /// <param name="y">Action 2.</param>
        /// <returns>Negative: x &lt; y, 0: x == y, Positive: y &gt; x</returns>
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
