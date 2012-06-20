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
            SelectAll,
            Translate,
            Rotate,
            PropTimeline
        }

        public event Action<TimelineEditorContext> Shutdown;

        private String currentFile;
        private Timeline currentTimeline;

        private EventContext eventContext;
        private AnomalousMvcContext mvcContext;
        private TimelineTypeController timelineTypeController;
        private PropEditController propEditController;

        public TimelineEditorContext(Timeline timeline, String path, TimelineTypeController timelineTypeController, PropEditController propEditController)
        {
            this.currentTimeline = timeline;
            this.currentFile = path;
            this.timelineTypeController = timelineTypeController;
            this.propEditController = propEditController;

            mvcContext = new AnomalousMvcContext();
            mvcContext.Models.add(new EditMenuManager());
            mvcContext.Models.add(new EditInterfaceHandler());
            
            mvcContext.Views.add(new TimelineEditorView("TimelineEditor", currentTimeline));

            ExpandingGenericEditorView genericEditor = new ExpandingGenericEditorView("TimelinePropertiesEditor", currentTimeline.getEditInterface());
            genericEditor.IsWindow = true;
            mvcContext.Views.add(genericEditor);
            
            PropTimelineView propTimelineView = new PropTimelineView("PropTimeline");
            propTimelineView.Buttons.add(new CloseButtonDefinition("Close", "PropTimeline/Close"));
            mvcContext.Views.add(propTimelineView);

            OpenPropManagerView propManagerView = new OpenPropManagerView("PropManager");
            propManagerView.Buttons.add(new CloseButtonDefinition("Close", "PropManager/Close"));
            mvcContext.Views.add(propManagerView);

            EditorTaskbarView taskbar = new EditorTaskbarView("TimelineInfoBar", currentFile, "TimelineEditor/Close");
            //taskbar.addTask(new RunMvcContextActionTask("CloseTimeline", "Close Timeline","", "", "TimelineEditor/CloseTimeline"));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Timeline", "FileToolstrip/Save", "", "TimelineEditor/Save", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "", "TimelineEditor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "", "TimelineEditor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "", "TimelineEditor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "", "TimelineEditor/SelectAll", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Translation", "Translation", "Editor/TranslateIcon", "", "TimelineEditor/Translation", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Rotation", "Rotation", "Editor/RotateIcon", "", "TimelineEditor/Rotation", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("PropTimeline", "Prop Timeline Editor", "PropEditorIcon", "", "PropTimeline/ShowIfNotOpen", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("PropManager", "Open Prop Manager", "PropManagerIcon", "", "PropManager/ShowIfNotOpen", mvcContext));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("TimelineEditor",
                new RunCommandsAction("Show",
                    new ShowViewCommand("TimelineEditor"),
                    new ShowViewCommand("TimelinePropertiesEditor"),
                    new ShowViewCommand("TimelineInfoBar")
                ),
                new RunCommandsAction("Close",
                    new CloseAllViewsCommand()),
                new CallbackAction("CloseTimeline", context =>
                    {
                        timelineTypeController.closeTimeline();
                        context.runAction("TimelineEditor/Close");
                    }),
                new CallbackAction("Save", context =>
                    {
                        timelineTypeController.saveTimeline(currentTimeline, currentFile);
                    }),
                new CutAction(),
                new CopyAction(),
                new PasteAction(),
                new SelectAllAction(),
                new CallbackAction("Translation", context =>
                    {
                        propEditController.setMoveMode();
                    }),
                new CallbackAction("Rotation", context =>
                    {
                        propEditController.setRotateMode();
                    })
            ));

            mvcContext.Controllers.add(new MvcController("PropTimeline",
                new RunCommandsAction("ShowIfNotOpen",
                    new ShowViewIfNotOpenCommand("PropTimeline")
                ),
                new RunCommandsAction("Close",
                    new CloseViewCommand())));

            mvcContext.Controllers.add(new MvcController("PropManager",
                new RunCommandsAction("ShowIfNotOpen",
                    new ShowViewIfNotOpenCommand("PropManager")),
                new RunCommandsAction("Close",
                    new CloseViewCommand())));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start",
                    new RunActionCommand("TimelineEditor/Show"),
                    new CallbackCommand(context =>
                        {
                            GlobalContextEventHandler.setEventContext(eventContext);
                        })),
                new CallbackAction("Shutdown", context =>
                    {
                        GlobalContextEventHandler.disableEventContext(eventContext);
                        propEditController.removeAllOpenProps();
                        if (Shutdown != null)
                        {
                            Shutdown.Invoke(this);
                        }
                    })));

            eventContext = new EventContext();

            eventContext.addEvent(new MessageEvent(Events.Save,
                frameUp: eventManager =>
                {
                    timelineTypeController.saveTimeline(currentTimeline, currentFile);
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_S }));

            eventContext.addEvent(new MessageEvent(Events.TogglePlay,
                frameUp: eventManager =>
                {
                    if (timeline.TimelineController.Playing)
                    {
                        timeline.TimelineController.stopPlayback();
                    }
                    else
                    {
                        timeline.TimelineController.startPlayback(timeline, propEditController.MarkerPosition, false);
                    }
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_SPACE }));

            eventContext.addEvent(new MessageEvent(Events.Cut,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("TimelineEditor/Cut");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_X }));

            eventContext.addEvent(new MessageEvent(Events.Copy,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("TimelineEditor/Copy");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_C }));

            eventContext.addEvent(new MessageEvent(Events.Paste,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("TimelineEditor/Paste");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_V }));

            eventContext.addEvent(new MessageEvent(Events.SelectAll,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("TimelineEditor/SelectAll");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_A }));

            eventContext.addEvent(new MessageEvent(Events.Translate,
                frameUp: eventManager =>
                    {
                        mvcContext.runAction("TimelineEditor/Translation");
                    },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_T }));

            eventContext.addEvent(new MessageEvent(Events.Rotate,
                frameUp: eventManager =>
                    {
                        mvcContext.runAction("TimelineEditor/Rotation");
                    },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_R }));

            eventContext.addEvent(new MessageEvent(Events.PropTimeline,
                frameUp: EventManager =>
                    {
                        mvcContext.runAction("PropTimeline/ShowIfNotOpen");
                    },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_P }));
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

        static TimelineEditorContext()
        {
            PropertiesForm.addFormCreationMethod(typeof(ChangeHandPosition), (property, parentWidget) =>
            {
                return new PoseableHandProperties(property, parentWidget);
            });
        }
    }
}
