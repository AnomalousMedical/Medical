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

        [Editable]
        public bool Fullscreen
        {
            get
            {
                return timeline.Fullscreen;
            }
            set
            {
                timeline.Fullscreen = value;
            }
        }

        private void reverseSides(EditUICallback callback, EditInterfaceCommand caller)
        {
            timeline.reverseSides();
        }
    }

    public class TimelinePreActionEditInterface
    {
        private Timeline timeline;
        private EditInterface editInterface;
        private Browser browser;
        private EditInterfaceManager<TimelineInstantAction> actionManager;
        private static String[] SEPS = { "." };

        public TimelinePreActionEditInterface(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                browser = new Browser("Pre Actions", "Choose Pre Action");
                browser.addNode("", SEPS, new BrowserNode("Change Scene", typeof(OpenNewSceneAction)));
                browser.addNode("", SEPS, new BrowserNode("Show Skip To Post Actions Prompt", typeof(SkipToPostActions)));
                browser.addNode("", SEPS, new BrowserNode("Run Mvc Action", typeof(RunMvcAction)));

                editInterface = ReflectedEditInterface.createEditInterface("Pre Actions", null);
                editInterface.addCommand(new EditInterfaceCommand("Add Pre Action", addAction));

                actionManager = new EditInterfaceManager<TimelineInstantAction>(editInterface);
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
            actionManager.addSubInterface(action, action.getEditInterface());
        }

        public void preActionRemoved(TimelineInstantAction action)
        {
            actionManager.removeSubInterface(action);
        }

        private void addAction(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.showBrowser(browser, delegate(Object result, ref String errorMessage)
            {
                Type createType = (Type)result;
                TimelineInstantAction action = (TimelineInstantAction)Activator.CreateInstance(createType);
                timeline.addPreAction(action);
                return true;
            });
        }

        private void removeAction(EditUICallback callback, EditInterfaceCommand caller)
        {
            EditInterface editInterface = callback.getSelectedEditInterface();
            timeline.removePreAction(actionManager.resolveSourceObject(editInterface));
        }
    }

    public class TimelinePostActionEditInterface
    {
        private Timeline timeline;
        private EditInterface editInterface;
        private Browser browser;
        private static String[] SEPS = { "." };
        private EditInterfaceManager<TimelineInstantAction> actionManager;

        public TimelinePostActionEditInterface(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                browser = new Browser("Post Actions", "Choose Post Action");
                browser.addNode("", SEPS, new BrowserNode("Load Another Timeline", typeof(LoadAnotherTimeline)));
                browser.addNode("", SEPS, new BrowserNode("Repeat Previous", typeof(RepeatPreviousPostActions)));
                browser.addNode("", SEPS, new BrowserNode("Run Mvc Action", typeof(RunMvcAction)));

                editInterface = ReflectedEditInterface.createEditInterface("Post Actions", null);
                editInterface.addCommand(new EditInterfaceCommand("Add Post Action", addAction));

                actionManager = new EditInterfaceManager<TimelineInstantAction>(editInterface);
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
            actionManager.addSubInterface(action, action.getEditInterface());
        }

        public void postActionRemoved(TimelineInstantAction action)
        {
            actionManager.removeSubInterface(action);
        }

        private void addAction(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.showBrowser(browser, delegate(Object result, ref String errorMessage)
            {
                Type createType = (Type)result;
                TimelineInstantAction action = (TimelineInstantAction)Activator.CreateInstance(createType);
                timeline.addPostAction(action);
                return true;
            });
        }

        private void removeAction(EditUICallback callback, EditInterfaceCommand caller)
        {
            EditInterface editInterface = callback.getSelectedEditInterface();
            timeline.removePostAction(actionManager.resolveSourceObject(editInterface));
        }
    }
}
