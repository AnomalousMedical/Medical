using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Platform;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using Engine.Platform;
using Medical.GUI.AnomalousMvc;
using Medical;
using Lecture.GUI;

namespace Lecture
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

        public event Action<TimelineEditorContext> Focus;
        public event Action<TimelineEditorContext> Blur;

        private Timeline currentTimeline;

        private EventContext eventContext;
        private AnomalousMvcContext mvcContext;
        private SlideshowEditController slideshowEditController;
        private PropEditController propEditController;

        public TimelineEditorContext(Timeline timeline, Slide slide, String name, SlideshowEditController slideshowEditController, PropEditController propEditController, EditorController editorController, MedicalUICallback uiCallback, TimelineController timelineController)
        {
            this.currentTimeline = timeline;
            this.slideshowEditController = slideshowEditController;
            this.propEditController = propEditController;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            mvcContext.Models.add(new EditMenuManager());
            mvcContext.Models.add(new EditInterfaceHandler());
            
            mvcContext.Views.add(new TimelineEditorView("TimelineEditor", currentTimeline, timelineController, editorController, propEditController));

            ExpandingGenericEditorView genericEditor = new ExpandingGenericEditorView("TimelinePropertiesEditor", currentTimeline.getEditInterface(), editorController, uiCallback);
            genericEditor.IsWindow = true;
            genericEditor.ViewLocation = ViewLocations.Left;
            mvcContext.Views.add(genericEditor);
            
            PropTimelineView propTimelineView = new PropTimelineView("PropTimeline", propEditController);
            propTimelineView.Buttons.add(new CloseButtonDefinition("Close", "PropTimeline/Close"));
            mvcContext.Views.add(propTimelineView);

            OpenPropManagerView propManagerView = new OpenPropManagerView("PropManager", propEditController);
            propManagerView.Buttons.add(new CloseButtonDefinition("Close", "PropManager/Close"));
            mvcContext.Views.add(propManagerView);

            MovementSequenceEditorView movementSequenceEditor = new MovementSequenceEditorView("MovementSequenceEditor", listenForSequenceChanges: true);
            movementSequenceEditor.Buttons.add(new CloseButtonDefinition("Close", "MovementSequenceEditor/Close"));
            movementSequenceEditor.ViewLocation = ViewLocations.Top;
            mvcContext.Views.add(movementSequenceEditor);

            SlideTaskbarView taskbar = new SlideTaskbarView("TimelineInfoBar", name);
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "FileToolstrip/Save", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "", "TimelineEditor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "", "TimelineEditor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "", "TimelineEditor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "", "TimelineEditor/SelectAll", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Translation", "Translation", "Editor/TranslateIcon", "", "TimelineEditor/Translation", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Rotation", "Rotation", "Editor/RotateIcon", "", "TimelineEditor/Rotation", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("PropTimeline", "Prop Timeline Editor", "PropEditorIcon", "", "PropTimeline/ShowIfNotOpen", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("PropManager", "Open Prop Manager", "PropManagerIcon", "", "PropManager/ShowIfNotOpen", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("MovementSequenceEditor", "Movement Sequence Editor", "MovementSequenceEditorIcon", "", "MovementSequenceEditor/ShowIfNotOpen", mvcContext));
            taskbar.addTask(new CallbackTask("EditSlide", "Edit Slide", "Lecture.Icon.EditSlide", "", 0, true, item =>
            {
                slideshowEditController.editSlide(slide);
            }));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("TimelineEditor",
                new RunCommandsAction("Show",
                    new ShowViewCommand("TimelineEditor"),
                    new ShowViewCommand("TimelinePropertiesEditor"),
                    new ShowViewCommand("TimelineInfoBar")
                ),
                new RunCommandsAction("Close",
                    new CloseAllViewsCommand()),
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

            mvcContext.Controllers.add(new MvcController("MovementSequenceEditor",
                new RunCommandsAction("ShowIfNotOpen",
                    new ShowViewIfNotOpenCommand("MovementSequenceEditor")
                ),
                new RunCommandsAction("Close",
                    new CloseViewCommand())));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start", new RunActionCommand("TimelineEditor/Show")),
                new CallbackAction("Focus", context =>
                    {
                        GlobalContextEventHandler.setEventContext(eventContext);
                        if (Focus != null)
                        {
                            Focus.Invoke(this);
                        }
                    }),
                new CallbackAction("Blur", context =>
                    {
                        GlobalContextEventHandler.disableEventContext(eventContext);
                        propEditController.removeAllOpenProps();
                        if (Blur != null)
                        {
                            Blur.Invoke(this);
                        }
                    }),
                new RunCommandsAction("Suspended", new SaveViewLayoutCommand()),
                new RunCommandsAction("Resumed", new RestoreViewLayoutCommand())));

            eventContext = new EventContext();

            eventContext.addEvent(new MessageEvent(Events.Save,
                frameUp: eventManager =>
                {
                    saveAll();
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

        private void saveAll()
        {
            slideshowEditController.save();
        }
    }
}
