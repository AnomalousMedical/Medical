﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Controller.AnomalousMvc;
using Engine.Platform;
using Medical.GUI.AnomalousMvc;
using Medical;
using Lecture.GUI;
using Engine;
using Medical.Controller;
using System.IO;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;

namespace Lecture
{
    class TimelineEditorContext
    {
        public event Action<TimelineEditorContext> Focus;
        public event Action<TimelineEditorContext> Blur;

        private Timeline currentTimeline;

        private EventContext eventContext;
        private AnomalousMvcContext mvcContext;
        private SlideshowEditController slideshowEditController;
        private PropEditController propEditController;
        private Slide slide;
        private TimelineEditorComponent timelineEditor;

        public TimelineEditorContext(Timeline timeline, Slide slide, String name, SlideshowEditController slideshowEditController, PropEditController propEditController, PropFactory propFactory, EditorController editorController, MedicalUICallback uiCallback, TimelineController timelineController)
        {
            this.slide = slide;
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

            var timelineEditorView = new TimelineEditorView("TimelineEditor", currentTimeline, timelineController, editorController, propEditController)
                {
                    DisplayTitle = "Main Timeline"
                };
            timelineEditorView.ComponentCreated += timelineEditorView_ComponentCreated;
            mvcContext.Views.add(timelineEditorView);

            ExpandingGenericEditorView genericEditor = new ExpandingGenericEditorView("TimelinePropertiesEditor", currentTimeline.getEditInterface(), editorController, uiCallback)
                {
                    DisplayTitle = "Properties"
                };
            genericEditor.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left)
            {
                AllowedDockLocations = DockLocation.Left | DockLocation.Right | DockLocation.Floating
            };
            mvcContext.Views.add(genericEditor);

            PropTimelineView propTimelineView = new PropTimelineView("PropTimeline", propEditController, propFactory)
                {
                    DisplayTitle = "Prop Timeline"
                };
            propTimelineView.Buttons.add(new CloseButtonDefinition("Close", "PropTimeline/Close"));
            mvcContext.Views.add(propTimelineView);

            OpenPropManagerView propManagerView = new OpenPropManagerView("PropManager", propEditController)
                {
                    DisplayTitle = "Prop Manager"
                };
            propManagerView.Buttons.add(new CloseButtonDefinition("Close", "PropManager/Close"));
            mvcContext.Views.add(propManagerView);

            MovementSequenceEditorView movementSequenceEditor = new MovementSequenceEditorView("MovementSequenceEditor", listenForSequenceChanges: true)
                {
                    DisplayTitle = "Movement Sequence"
                };
            movementSequenceEditor.Buttons.add(new CloseButtonDefinition("Close", "MovementSequenceEditor/Close"));
            movementSequenceEditor.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Top)
            {
                AllowedDockLocations = DockLocation.Top | DockLocation.Bottom | DockLocation.Floating
            };
            mvcContext.Views.add(movementSequenceEditor);

            SlideTaskbarView taskbar = new SlideTaskbarView("TimelineInfoBar", name);
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "CommonToolstrip/Save", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "", "TimelineEditor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "", "TimelineEditor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "", "TimelineEditor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "", "TimelineEditor/SelectAll", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Translation", "Translation", "Editor/TranslateIcon", "", "TimelineEditor/Translation", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Rotation", "Rotation", "Editor/RotateIcon", "", "TimelineEditor/Rotation", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("PropTimeline", "Prop Timeline Editor", "Editor/PropTimelineEditorIcon", "", "PropTimeline/ShowIfNotOpen", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("PropManager", "Open Prop Manager", "Editor/PropManagerIcon", "", "PropManager/ShowIfNotOpen", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("MovementSequenceEditor", "Movement Sequence Editor", "Editor/MovementSequenceEditorIcon", "", "MovementSequenceEditor/ShowIfNotOpen", mvcContext));
            taskbar.addTask(new CallbackTask("EditSlide", "Edit Slide", "Lecture.Icon.EditSlide", "", 0, true, item =>
            {
                slideshowEditController.editSlide(slide);
            }));
            mvcContext.Views.add(taskbar);

            RunCommandsAction showCommand = new RunCommandsAction("Show",
                    new ShowViewCommand("TimelineEditor"),
                    new ShowViewCommand("TimelinePropertiesEditor"),
                    new ShowViewCommand("TimelineInfoBar"));

            refreshPanelPreviews(showCommand);

            mvcContext.Controllers.add(new MvcController("TimelineEditor",
                showCommand,
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

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    saveAll();
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_S }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
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

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("TimelineEditor/Cut");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_X }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("TimelineEditor/Copy");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_C }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("TimelineEditor/Paste");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_V }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("TimelineEditor/SelectAll");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_A }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                    {
                        mvcContext.runAction("TimelineEditor/Translation");
                    },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_T }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                    {
                        mvcContext.runAction("TimelineEditor/Rotation");
                    },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_R }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
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

        public void clearSelection()
        {
            timelineEditor.clearSelection();
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
            slideshowEditController.safeSave();
        }

        private void refreshPanelPreviews(RunCommandsAction showEditorWindowsCommand)
        {
            SlideDisplayManager displayManager = new SlideDisplayManager(true);
            SlideInstanceLayoutStrategy instanceLayout = slide.LayoutStrategy.createLayoutStrategy(displayManager);
            foreach (RmlSlidePanel panel in slide.Panels.Where(p => p is RmlSlidePanel))
            {
                String editorViewName = panel.createViewName("RmlView");
                RmlView rmlView = new RmlView(editorViewName);
                rmlView.ElementName = panel.ElementName;
                rmlView.RmlFile = panel.getRmlFilePath(slide);
                instanceLayout.addView(rmlView);
                mvcContext.Views.add(rmlView);
                showEditorWindowsCommand.addCommand(new ShowViewCommand(rmlView.Name));
            }
        }

        void timelineEditorView_ComponentCreated(TimelineEditorComponent timelineEditor)
        {
            this.timelineEditor = timelineEditor;
        }
    }
}
