using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Platform;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using Engine.Platform;
using Medical.GUI.AnomalousMvc;

namespace Medical
{
    class TimelineEditorContext
    {
        enum Events
        {
            Save,
            TogglePlay,
            Cut,
            Copy,
            Paste,
            SelectAll
        }

        public event Action<TimelineEditorContext> Shutdown;

        private String currentFile;
        private Timeline currentTimeline;
        private TimelineEditorComponent timelineEditorComponent = null;

        private EventContext eventContext;
        private AnomalousMvcContext mvcContext;
        private TimelineTypeController timelineTypeController;
        private SimObjectMover simObjectMover;

        public TimelineEditorContext(Timeline timeline, String path, TimelineTypeController timelineTypeController, SimObjectMover simObjectMover)
        {
            this.currentTimeline = timeline;
            this.currentFile = path;
            this.timelineTypeController = timelineTypeController;
            this.simObjectMover = simObjectMover;

            mvcContext = new AnomalousMvcContext();
            TimelineEditorView timelineEditorView = new TimelineEditorView("TimelineEditor", currentTimeline);
            timelineEditorView.ComponentCreated += (view, component) =>
            {
                timelineEditorComponent = component;
            };
            mvcContext.Views.add(timelineEditorView);
            GenericEditorView genericEditor = new GenericEditorView("TimelinePropertiesEditor", currentTimeline.getEditInterface());
            genericEditor.IsWindow = true;
            mvcContext.Views.add(genericEditor);
            PropTimelineView propTimelineView = new PropTimelineView("PropTimeline");
            propTimelineView.Buttons.add(new CloseButtonDefinition("Close", "PropTimeline/Close"));
            mvcContext.Views.add(propTimelineView);
            EditorInfoBarView infoBar = new EditorInfoBarView("TimelineInfoBar", String.Format("{0} - Timeline", currentFile), "TimelineEditor/Close");
            infoBar.addAction(new EditorInfoBarAction("Close Timeline", "File", "TimelineEditor/CloseTimeline"));
            infoBar.addAction(new EditorInfoBarAction("Save Timeline", "File", "TimelineEditor/Save"));
            infoBar.addAction(new EditorInfoBarAction("Cut", "Edit", "TimelineEditor/Cut"));
            infoBar.addAction(new EditorInfoBarAction("Copy", "Edit", "TimelineEditor/Copy"));
            infoBar.addAction(new EditorInfoBarAction("Paste", "Edit", "TimelineEditor/Paste"));
            infoBar.addAction(new EditorInfoBarAction("Select All", "Edit", "TimelineEditor/SelectAll"));
            infoBar.addAction(new EditorInfoBarAction("Translation", "Tools", "TimelineEditor/Translation"));
            infoBar.addAction(new EditorInfoBarAction("Rotation", "Tools", "TimelineEditor/Rotation"));
            infoBar.addAction(new EditorInfoBarAction("Prop Timeline Editor", "Props", "PropTimeline/Show"));
            mvcContext.Views.add(infoBar);
            MvcController timelineEditorController = new MvcController("TimelineEditor");
            RunCommandsAction showAction = new RunCommandsAction("Show");
            showAction.addCommand(new ShowViewCommand("TimelineEditor"));
            showAction.addCommand(new ShowViewCommand("TimelinePropertiesEditor"));
            showAction.addCommand(new ShowViewCommand("TimelineInfoBar"));
            timelineEditorController.Actions.add(showAction);
            RunCommandsAction closeAction = new RunCommandsAction("Close");
            closeAction.addCommand(new CloseAllViewsCommand());
            timelineEditorController.Actions.add(closeAction);
            timelineEditorController.Actions.add(new CallbackAction("CloseTimeline", context =>
            {
                timelineTypeController.closeTimeline();
                context.runAction("TimelineEditor/Close");
            }));
            timelineEditorController.Actions.add(new CallbackAction("Save", context =>
            {
                timelineTypeController.saveTimeline(currentTimeline, currentFile);
            }));
            timelineEditorController.Actions.add(new CallbackAction("Cut", context =>
            {
                timelineEditorComponent.cut();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Copy", context =>
            {
                timelineEditorComponent.copy();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Paste", context =>
            {
                timelineEditorComponent.paste();
            }));
            timelineEditorController.Actions.add(new CallbackAction("SelectAll", context =>
            {
                timelineEditorComponent.selectAll();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Translation", context =>
            {
                simObjectMover.ShowMoveTools = true;
                simObjectMover.ShowRotateTools = false;
            }));
            timelineEditorController.Actions.add(new CallbackAction("Rotation", context =>
            {
                simObjectMover.ShowMoveTools = false;
                simObjectMover.ShowRotateTools = true;
            }));

            mvcContext.Controllers.add(timelineEditorController);

            mvcContext.Controllers.add(new MvcController("PropTimeline", 
                new RunCommandsAction("Show", 
                    new ShowViewCommand("PropTimeline")
                ),
                new RunCommandsAction("Close", 
                    new CloseViewCommand()
                )
            ));

            MvcController common = new MvcController("Common");
            RunCommandsAction startup = new RunCommandsAction("Start");
            startup.addCommand(new RunActionCommand("TimelineEditor/Show"));
            startup.addCommand(new CallbackCommand(context =>
            {
                GlobalContextEventHandler.setEventContext(eventContext);
            }));
            common.Actions.add(startup);
            CallbackAction shutdown = new CallbackAction("Shutdown", context =>
            {
                timelineEditorComponent = null;
                GlobalContextEventHandler.disableEventContext(eventContext);
                if (Shutdown != null)
                {
                    Shutdown.Invoke(this);
                }
            });
            common.Actions.add(shutdown);
            mvcContext.Controllers.add(common);

            eventContext = new EventContext();
            MessageEvent saveEvent = new MessageEvent(Events.Save);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                timelineTypeController.saveTimeline(currentTimeline, currentFile);
            };
            eventContext.addEvent(saveEvent);
            MessageEvent togglePlayEvent = new MessageEvent(Events.TogglePlay);
            togglePlayEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            togglePlayEvent.addButton(KeyboardButtonCode.KC_SPACE);
            togglePlayEvent.FirstFrameUpEvent += eventManager =>
            {
                timelineEditorComponent.togglePlayPreview();
            };
            eventContext.addEvent(togglePlayEvent);

            MessageEvent cut = new MessageEvent(Events.Cut);
            cut.addButton(KeyboardButtonCode.KC_LCONTROL);
            cut.addButton(KeyboardButtonCode.KC_X);
            cut.FirstFrameUpEvent += eventManager =>
            {
                mvcContext.runAction("TimelineEditor/Cut");
            };
            eventContext.addEvent(cut);

            MessageEvent copy = new MessageEvent(Events.Copy);
            copy.addButton(KeyboardButtonCode.KC_LCONTROL);
            copy.addButton(KeyboardButtonCode.KC_C);
            copy.FirstFrameUpEvent += eventManager =>
            {
                mvcContext.runAction("TimelineEditor/Copy");
            };
            eventContext.addEvent(copy);

            MessageEvent paste = new MessageEvent(Events.Paste);
            paste.addButton(KeyboardButtonCode.KC_LCONTROL);
            paste.addButton(KeyboardButtonCode.KC_V);
            paste.FirstFrameUpEvent += eventManager =>
            {
                mvcContext.runAction("TimelineEditor/Paste");
            };
            eventContext.addEvent(paste);

            MessageEvent selectAll = new MessageEvent(Events.SelectAll);
            selectAll.addButton(KeyboardButtonCode.KC_LCONTROL);
            selectAll.addButton(KeyboardButtonCode.KC_A);
            selectAll.FirstFrameUpEvent += eventManager =>
            {
                mvcContext.runAction("TimelineEditor/SelectAll");
            };
            eventContext.addEvent(selectAll);
        }

        public void close()
        {
            mvcContext.runAction("TimelineEditor/Close");
        }

        public AnomalousMvcContext MvcContext
        {
            get
            {
                return mvcContext;
            }
        }
    }
}
