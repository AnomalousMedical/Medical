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
    class MovementSequenceEditorContext
    {
        public event Action<MovementSequenceEditorContext> Focus;
        public event Action<MovementSequenceEditorContext> Blur;

        enum Events
        {
            Save, 
            Cut,
            Copy,
            Paste,
            SelectAll,
        }

        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private MovementSequenceTypeController movementSequenceTypeController;

        private MovementSequence movementSequence;

        public MovementSequenceEditorContext(MovementSequence movementSequence, String file, MovementSequenceTypeController movementSequenceTypeController)
        {
            this.movementSequenceTypeController = movementSequenceTypeController;
            this.currentFile = file;
            this.movementSequence = movementSequence;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            mvcContext.Models.add(new EditMenuManager());

            MovementSequenceEditorView movementSequenceView = new MovementSequenceEditorView("MovementSequenceEditor", movementSequence);
            movementSequenceView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Bottom);
            mvcContext.Views.add(movementSequenceView);

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
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor", 
                new RunCommandsAction("Show",
                    new ShowViewCommand("MovementSequenceEditor"),
                    new ShowViewCommand("InfoBar")),
                new RunCommandsAction("Close", new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                    {
                        save();
                    }),
                new CutAction(),
                new CopyAction(),
                new PasteAction(),
                new SelectAllAction()));

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
            MessageEvent saveEvent = new MessageEvent(Events.Save, EventLayers.Gui);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                saveAll();
            };
            eventContext.addEvent(saveEvent);

            eventContext.addEvent(new MessageEvent(Events.Cut, EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/Cut");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_X }));

            eventContext.addEvent(new MessageEvent(Events.Copy, EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/Copy");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_C }));

            eventContext.addEvent(new MessageEvent(Events.Paste, EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/Paste");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_V }));

            eventContext.addEvent(new MessageEvent(Events.SelectAll, EventLayers.Gui,
                frameUp: eventManager =>
                {
                    mvcContext.runAction("Editor/SelectAll");
                },
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_A }));
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
            movementSequenceTypeController.EditorController.saveAllCachedResources();
        }

        private void save()
        {
            movementSequenceTypeController.saveFile(movementSequence, currentFile);
        }
    }
}
