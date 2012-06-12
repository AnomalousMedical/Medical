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
        private TimelineEditorComponent timelineEditorComponent = null;
        private PropTimeline propTimeline = null;
        private OpenPropManager openPropManager = null;

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
            propTimelineView.ComponentCreated += (view, component) =>
            {
                propTimeline = component;
            };
            mvcContext.Views.add(propTimelineView);

            OpenPropManagerView propManagerView = new OpenPropManagerView("PropManager");
            propManagerView.Buttons.add(new CloseButtonDefinition("Close", "PropManager/Close"));
            propManagerView.ComponentCreated += (view, component) =>
            {
                openPropManager = component;
            };
            mvcContext.Views.add(propManagerView);
            
            EditorInfoBarView infoBar = new EditorInfoBarView("TimelineInfoBar", String.Format("{0} - Timeline", currentFile), "TimelineEditor/Close");
            infoBar.addAction(new EditorInfoBarAction("Close Timeline", "File", "TimelineEditor/CloseTimeline"));
            infoBar.addAction(new EditorInfoBarAction("Save Timeline", "File", "TimelineEditor/Save"));
            infoBar.addAction(new EditorInfoBarAction("Cut", "Edit", "TimelineEditor/Cut"));
            infoBar.addAction(new EditorInfoBarAction("Copy", "Edit", "TimelineEditor/Copy"));
            infoBar.addAction(new EditorInfoBarAction("Paste", "Edit", "TimelineEditor/Paste"));
            infoBar.addAction(new EditorInfoBarAction("Select All", "Edit", "TimelineEditor/SelectAll"));
            infoBar.addAction(new EditorInfoBarAction("Translation", "Tools", "TimelineEditor/Translation"));
            infoBar.addAction(new EditorInfoBarAction("Rotation", "Tools", "TimelineEditor/Rotation"));
            infoBar.addAction(new EditorInfoBarAction("Prop Timeline Editor", "Props", "PropTimeline/ShowIfNotOpen"));
            infoBar.addAction(new EditorInfoBarAction("Open Prop Manager", "Props", "PropManager/ShowIfNotOpen"));
            mvcContext.Views.add(infoBar);

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
                new CallbackAction("ShowIfNotOpen", context =>
                    {
                        if (propTimeline == null)
                        {
                            context.runAction("PropTimeline/Show");
                        }
                    }),
                new RunCommandsAction("Show",
                    new ShowViewCommand("PropTimeline")
                ),
                new RunCommandsAction("Close",
                    new CloseViewCommand(),
                    new CallbackCommand(context =>
                    {
                        propTimeline = null;
                    }))));

            mvcContext.Controllers.add(new MvcController("PropManager",
                new CallbackAction("ShowIfNotOpen", context =>
                    {
                        if (openPropManager == null)
                        {
                            context.runAction("PropManager/Show");
                        }
                    }),
                new RunCommandsAction("Show",
                    new ShowViewCommand("PropManager")),
                new RunCommandsAction("Close",
                    new CloseViewCommand(),
                    new CallbackCommand(context =>
                        {
                            openPropManager = null;
                        }))));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start",
                    new RunActionCommand("TimelineEditor/Show"),
                    new CallbackCommand(context =>
                        {
                            GlobalContextEventHandler.setEventContext(eventContext);
                        })),
                new CallbackAction("Shutdown", context =>
                    {
                        timelineEditorComponent = null;
                        propTimeline = null;
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
    }
}
