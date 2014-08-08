using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    public class TimelineEditInterface
    {
        private Timeline timeline;
        private EditInterface editInterface;
        private TimelinePreActionEditInterface preActionEdit;
        private TimelinePostActionEditInterface postActionEdit;

        public TimelineEditInterface(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                preActionEdit = new TimelinePreActionEditInterface(timeline);
                postActionEdit = new TimelinePostActionEditInterface(timeline);

                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, "Timeline", null);
                editInterface.addSubInterface(preActionEdit.getEditInterface());
                editInterface.addSubInterface(postActionEdit.getEditInterface());

                editInterface.addCommand(new EditInterfaceCommand("Reverse Sides", reverseSides));
            }
            return editInterface;
        }

        public TimelinePreActionEditInterface PreActionsEdit
        {
            get
            {
                return preActionEdit;
            }
        }

        public TimelinePostActionEditInterface PostActionsEdit
        {
            get
            {
                return postActionEdit;
            }
        }

        private void reverseSides(EditUICallback callback, EditInterfaceCommand caller)
        {
            timeline.reverseSides();
        }
    }

    public class TimelinePreActionEditInterface
    {
        public enum CustomQueries
        {
            /// <summary>
            /// Sync custom query, returns browser of types that extend TimelineInstantAction
            /// </summary>
            BuildActionBrowser
        }

        private Timeline timeline;
        private EditInterface editInterface;

        public TimelinePreActionEditInterface(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createUnscannedEditInterface("Pre Actions", null);
                editInterface.addCommand(new EditInterfaceCommand("Add Pre Action", addAction));

                var actionManager = editInterface.createEditInterfaceManager<TimelineInstantAction>();
                actionManager.addCommand(new EditInterfaceCommand("Remove", removeAction));

                foreach (TimelineInstantAction action in timeline.PreActions)
                {
                    preActionAdded(action);
                }
            }
            return editInterface;
        }

        public void preActionAdded(TimelineInstantAction action)
        {
            editInterface.addSubInterface(action, action.getEditInterface());
        }

        public void preActionRemoved(TimelineInstantAction action)
        {
            editInterface.removeSubInterface(action);
        }

        private void addAction(EditUICallback callback, EditInterfaceCommand caller)
        {
            var browser = callback.runSyncCustomQuery<Browser>(CustomQueries.BuildActionBrowser);
            if (browser != null)
            {
                callback.showBrowser(browser, delegate(Object result, ref String errorMessage)
                {
                    Type createType = (Type)result;
                    TimelineInstantAction action = (TimelineInstantAction)Activator.CreateInstance(createType);
                    timeline.addPreAction(action);
                    return true;
                });
            }
        }

        private void removeAction(EditUICallback callback, EditInterfaceCommand caller)
        {
            timeline.removePreAction(editInterface.resolveSourceObject<TimelineInstantAction>(callback.getSelectedEditInterface()));
        }
    }

    public class TimelinePostActionEditInterface
    {
        public enum CustomQueries
        {
            /// <summary>
            /// Sync custom query, returns browser of types that extend TimelineInstantAction
            /// </summary>
            BuildActionBrowser
        }

        private Timeline timeline;
        private EditInterface editInterface;

        public TimelinePostActionEditInterface(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createUnscannedEditInterface("Post Actions", null);
                editInterface.addCommand(new EditInterfaceCommand("Add Post Action", addAction));

                var actionManager = editInterface.createEditInterfaceManager<TimelineInstantAction>();
                actionManager.addCommand(new EditInterfaceCommand("Remove", removeAction));

                foreach (TimelineInstantAction action in timeline.PostActions)
                {
                    postActionAdded(action);
                }
            }
            return editInterface;
        }

        public void postActionAdded(TimelineInstantAction action)
        {
            editInterface.addSubInterface(action, action.getEditInterface());
        }

        public void postActionRemoved(TimelineInstantAction action)
        {
            editInterface.removeSubInterface(action);
        }

        private void addAction(EditUICallback callback, EditInterfaceCommand caller)
        {
            var browser = callback.runSyncCustomQuery<Browser>(CustomQueries.BuildActionBrowser);
            if (browser != null)
            {
                callback.showBrowser(browser, delegate(Object result, ref String errorMessage)
                {
                    Type createType = (Type)result;
                    TimelineInstantAction action = (TimelineInstantAction)Activator.CreateInstance(createType);
                    timeline.addPostAction(action);
                    return true;
                });
            }
        }

        private void removeAction(EditUICallback callback, EditInterfaceCommand caller)
        {
            timeline.removePostAction(editInterface.resolveSourceObject<TimelineInstantAction>(callback.getSelectedEditInterface()));
        }
    }
}
