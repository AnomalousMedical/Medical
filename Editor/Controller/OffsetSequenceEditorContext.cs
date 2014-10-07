using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Medical.Platform;
using Medical.Muscles;
using Medical.GUI;
using Engine.Platform;
using Medical.Controller;

namespace Medical
{
    class OffsetSequenceEditorContext
    {
        public event Action<OffsetSequenceEditorContext> Focus;
        public event Action<OffsetSequenceEditorContext> Blur;

        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private OffsetSequenceTypeController typeController;
        private SimObjectMover simObjectMover;

        private OffsetModifierSequence movementSequence;

        public OffsetSequenceEditorContext(OffsetModifierSequence movementSequence, String file, OffsetSequenceTypeController typeController, MedicalUICallback uiCallback, SimObjectMover simObjectMover)
        {
            this.typeController = typeController;
            this.currentFile = file;
            this.movementSequence = movementSequence;
            this.simObjectMover = simObjectMover;
            simObjectMover.ShowMoveTools = true;
            simObjectMover.ShowRotateTools = false;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            mvcContext.Models.add(new EditMenuManager());

            OffsetSequenceEditorView offsetSequenceView = new OffsetSequenceEditorView("SequenceEditor", uiCallback, simObjectMover, movementSequence);
            offsetSequenceView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Bottom);
            mvcContext.Views.add(offsetSequenceView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Movement Sequence File", "CommonToolstrip/Save", "File", "Editor/Save", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "", "Editor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "", "Editor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "", "Editor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "", "Editor/SelectAll", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Translation", "Translation", "Editor/TranslateIcon", "", "Editor/Translation", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Rotation", "Rotation", "Editor/RotateIcon", "", "Editor/Rotation", mvcContext));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor", 
                new RunCommandsAction("Show",
                    new ShowViewCommand("SequenceEditor"),
                    new ShowViewCommand("InfoBar")),
                new RunCommandsAction("Close", new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                    {
                        save();
                    }),
                new CutAction(),
                new CopyAction(),
                new PasteAction(),
                new SelectAllAction(),
                new CallbackAction("Translation", context =>
                {
                    this.simObjectMover.ShowMoveTools = true;
                    this.simObjectMover.ShowRotateTools = false;
                }),
                new CallbackAction("Rotation", context =>
                {
                    this.simObjectMover.ShowMoveTools = false;
                    this.simObjectMover.ShowRotateTools = true;
                })));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start", new RunActionCommand("Editor/Show")),
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
                        if (Blur != null)
                        {
                            Blur.Invoke(this);
                        }
                    }),
                new RunCommandsAction("Suspended", new SaveViewLayoutCommand()),
                new RunCommandsAction("Resumed", new RestoreViewLayoutCommand())));

            eventContext = new EventContext();
            ButtonEvent saveEvent = new ButtonEvent(EventLayers.Gui);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                saveAll();
            };
            eventContext.addEvent(saveEvent);

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/Cut");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_X }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/Copy");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_C }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/Paste");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_V }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/SelectAll");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_A }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/Translation");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_T }));

            eventContext.addEvent(new ButtonEvent(EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/Rotation");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_R }));
        }

        public void close()
        {
            mvcContext.runAction("Editor/Close");
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
            typeController.EditorController.saveAllCachedResources();
        }

        private void save()
        {
            typeController.saveFile(movementSequence, currentFile);
        }
    }
}
