using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Platform;
using Medical.Controller.AnomalousMvc;
using Engine.Platform;

namespace Medical
{
    class MvcEditorContext
    {
        public event Action<MvcEditorContext> Focused;
        public event Action<MvcEditorContext> Blured;

        enum Events
        {
            Save
        }

        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private MvcTypeController mvcTypeController;

        private AnomalousMvcContext editingContext;

        public MvcEditorContext(AnomalousMvcContext editingContext, String file, MvcTypeController mvcTypeController, EditorUICallback uiCallback)
        {
            this.mvcTypeController = mvcTypeController;
            this.currentFile = file;
            this.editingContext = editingContext;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            mvcContext.Models.add(new EditMenuManager());

            GenericPropertiesFormView genericPropertiesView = new GenericPropertiesFormView("MvcContext", editingContext.getEditInterface(), true);
            genericPropertiesView.ViewLocation = ViewLocations.Left;
            genericPropertiesView.IsWindow = true;
            mvcContext.Views.add(genericPropertiesView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "FileToolstrip/Save", "File", "Editor/Save", mvcContext));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor", 
                new RunCommandsAction("Show", 
                    new ShowViewCommand("MvcContext"),
                    new ShowViewCommand("InfoBar")),
                new RunCommandsAction("Close", 
                    new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                {
                    save();
                })));

            mvcContext.Controllers.add(new MvcController("Common", 
                new RunCommandsAction("Start", new RunActionCommand("Editor/Show")),
                new CallbackAction("Focus", context =>
                {
                    GlobalContextEventHandler.setEventContext(eventContext);
                    if (Focused != null)
                    {
                        Focused.Invoke(this);
                    }
                }),
                new CallbackAction("Blur", context =>
                {
                    GlobalContextEventHandler.disableEventContext(eventContext);
                    if (Blured != null)
                    {
                        Blured.Invoke(this);
                    }
                }),
                new RunCommandsAction("Suspended", new SaveViewLayoutCommand()),
                new RunCommandsAction("Resumed", new RestoreViewLayoutCommand())));

            taskbar.addTask(new CallbackTask("PreviewMvc", "Preview", "MvcContextEditor/MVCcomIcon", "", 0, true, (item) =>
            {
                uiCallback.previewMvcContext(editingContext);
            }));

            eventContext = new EventContext();
            eventContext.addEvent(new MessageEvent(Events.Save, 
                frameUp: eventManager =>
                {
                    saveAll();
                }, 
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_S }));
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
            mvcTypeController.EditorController.saveAllCachedResources();
        }

        private void save()
        {
            mvcTypeController.saveFile(editingContext, currentFile);
        }
    }
}
