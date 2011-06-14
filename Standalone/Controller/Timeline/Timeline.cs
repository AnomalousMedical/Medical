using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class Timeline : Saveable
    {
        private static CopySaver copySaver = new CopySaver();

        public event EventHandler<TimelineActionEventArgs> ActionAdded;
        public event EventHandler<TimelineActionEventArgs> ActionRemoved;

        private List<TimelineInstantAction> preActions = new List<TimelineInstantAction>();
        private List<TimelineInstantAction> postActions = new List<TimelineInstantAction>();
        private ActionSequencer<TimelineAction> sequencer;
        private int postActionIndex = -1;
        private bool autoFireMultitimelineStopped = true; //True to fire the multi timeline stopped event if there are no post actions.

        private TimelineEditInterface timelineEditInterface;

        public Timeline()
        {
            sequencer = new ActionSequencer<TimelineAction>();
        }

        public EditInterface getEditInterface()
        {
            if (timelineEditInterface == null)
            {
                timelineEditInterface = new TimelineEditInterface(this);
            }
            return timelineEditInterface.getEditInterface();
        }

        public void addPreAction(TimelineInstantAction action)
        {
            action._setTimeline(this);
            preActions.Add(action);
            if (timelineEditInterface != null)
            {
                timelineEditInterface.PreActionsEdit.preActionAdded(action);
            }
        }

        public void removePreAction(TimelineInstantAction action)
        {
            if (timelineEditInterface != null)
            {
                timelineEditInterface.PreActionsEdit.preActionRemoved(action);
            }
            action._setTimeline(null);
            preActions.Remove(action);
        }

        public void clearPreActions()
        {
            foreach (TimelineInstantAction action in preActions)
            {
                if (timelineEditInterface != null)
                {
                    timelineEditInterface.PreActionsEdit.preActionRemoved(action);
                }
                action._setTimeline(null);
            }
            preActions.Clear();
        }

        public void addAction(TimelineAction action)
        {
            action._setTimeline(this);
            sequencer.addAction(action);
            if (ActionAdded != null)
            {
                ActionAdded.Invoke(this, new TimelineActionEventArgs(action, sequencer.indexOf(action)));
            }
        }

        public void removeAction(TimelineAction action)
        {
            action._setTimeline(null);
            sequencer.removeAction(action);
            if (ActionRemoved != null)
            {
                ActionRemoved.Invoke(this, new TimelineActionEventArgs(action, 0));
            }
        }

        public void addPostAction(TimelineInstantAction action)
        {
            action._setTimeline(this);
            postActions.Add(action);
            if (timelineEditInterface != null)
            {
                timelineEditInterface.PostActionsEdit.postActionAdded(action);
            }
        }

        public void removePostAction(TimelineInstantAction action)
        {
            if (timelineEditInterface != null)
            {
                timelineEditInterface.PostActionsEdit.postActionRemoved(action);
            }
            int index = postActions.IndexOf(action);
            postActions.RemoveAt(index);
            //Adjust the iteration index backwards if the element being removed is before or on the index.
            //This way nothing gets skipped.
            if (index != -1 && index <= postActionIndex)
            {
                --postActionIndex;
            }
            action._setTimeline(null);
        }

        public void clearPostActions()
        {
            foreach (TimelineInstantAction action in postActions)
            {
                if (timelineEditInterface != null)
                {
                    timelineEditInterface.PostActionsEdit.postActionRemoved(action);
                }
                action._setTimeline(null);
            }
            postActions.Clear();
        }

        public List<TimelineInstantAction> duplicatePostActions()
        {
            List<TimelineInstantAction> copiedActions = new List<TimelineInstantAction>();
            foreach (TimelineInstantAction action in postActions)
            {
                copiedActions.Add(copySaver.copy<TimelineInstantAction>(action));
            }
            return copiedActions;
        }

        /// <summary>
        /// Dump info about the post actions to the log.
        /// </summary>
        public void dumpPostActionsToLog()
        {
            foreach (TimelineInstantAction action in postActions)
            {
                action.dumpToLog();
            }
        }

        public void start(bool playPreActions)
        {
            sequencer.start();
            if (playPreActions)
            {
                foreach (TimelineInstantAction action in preActions)
                {
                    action.doAction();
                }
            }
        }

        public void stop(bool playPostActions)
        {
            sequencer.stop();
            if (playPostActions)
            {
                if (postActions.Count == 0)
                {
                    if (autoFireMultitimelineStopped)
                    {
                        TimelineController._fireMultiTimelineStopEvent();
                    }
                }
                else
                {
                    for (postActionIndex = 0; postActionIndex < postActions.Count; ++postActionIndex)
                    {
                        postActions[postActionIndex].doAction();
                    }
                }
            }
            else
            {
                TimelineController._fireMultiTimelineStopEvent();
            }
        }

        public void update(Clock clock)
        {
            sequencer.update(clock);
        }

        public void skipTo(float time)
        {
            sequencer.skipTo(time);
        }

        public void reverseSides()
        {
            foreach (TimelineAction action in sequencer.Actions)
            {
                action.reverseSides();
            }
        }

        public bool Finished
        {
            get
            {
                return sequencer.Finished;
            }
        }

        /// <summary>
        /// If this is true and the timeline stops playing it will fire the
        /// PlaybackStopped event on the TimelineController if there are no post
        /// actions. If this is false this event will not be fired. If there are
        /// post actions the timeline will rely on them to fire this event when
        /// needed. If this is set to false then the event that set it to false
        /// should shutdown the timeline somehow.
        /// </summary>
        public bool AutoFireMultiTimelineStopped
        {
            get
            {
                return autoFireMultitimelineStopped;
            }
            set
            {
                autoFireMultitimelineStopped = value;
            }
        }

        /// <summary>
        /// The file this timeline loaded out of. Will be set by the
        /// TimelineController if it is loaded that way, otherwise this can be
        /// set manually. It is not used by the timeline in any way, it is just a reference.
        /// </summary>
        public String SourceFile { get; set; }

        public TimelineController TimelineController { get; internal set; }

        public void findFileReference(TimelineStaticInfo info)
        {
            foreach (TimelineInstantAction action in PreActions)
            {
                action.findFileReference(info);
            }

            foreach (TimelineAction action in Actions)
            {
                action.findFileReference(info);
            }

            foreach (TimelineInstantAction action in PostActions)
            {
                action.findFileReference(info);
            }
        }

        public IEnumerable<TimelineAction> Actions
        {
            get
            {
                return sequencer.Actions;
            }
        }

        public IEnumerable<TimelineInstantAction> PreActions
        {
            get
            {
                return preActions;
            }
        }

        public IEnumerable<TimelineInstantAction> PostActions
        {
            get
            {
                return postActions;
            }
        }

        public float CurrentTime
        {
            get
            {
                return sequencer.CurrentTime;
            }
        }

        internal void _actionStartChanged(TimelineAction action)
        {
            sequencer.sort();
        }

        #region Saveable Members

        private static readonly String PRE_ACTIONS = "PreActions";
        private static readonly String POST_ACTIONS = "PostActions";
        private static readonly String ACTIONS = "Actions";
        private static readonly String SEQUENCER = "Sequencer";

        protected Timeline(LoadInfo info)
        {
            info.RebuildList<TimelineInstantAction>(PRE_ACTIONS, preActions);
            info.RebuildList<TimelineInstantAction>(POST_ACTIONS, postActions);

            sequencer = info.GetValue<ActionSequencer<TimelineAction>>(SEQUENCER, null);
            if (sequencer == null)
            {
                sequencer = new ActionSequencer<TimelineAction>();
                //Must load old style list.
                List<TimelineAction> actions = new List<TimelineAction>();
                info.RebuildList<TimelineAction>(ACTIONS, actions);
                foreach (TimelineAction action in actions)
                {
                    sequencer.addAction(action);
                }
            }

            foreach (TimelineInstantAction action in preActions)
            {
                action._setTimeline(this);
            }

            foreach (TimelineInstantAction action in postActions)
            {
                action._setTimeline(this);
            }

            foreach (TimelineAction action in sequencer.Actions)
            {
                action._setTimeline(this);
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<TimelineInstantAction>(PRE_ACTIONS, preActions);
            info.ExtractList<TimelineInstantAction>(POST_ACTIONS, postActions);
            info.AddValue(SEQUENCER, sequencer);
        }

        #endregion
    }

    public class TimelineActionEventArgs : EventArgs
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
}
