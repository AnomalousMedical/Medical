﻿using System;
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
        public event Action<MvcEditorContext> Shutdown;

        enum Events
        {
            Save
        }

        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private MvcTypeController mvcTypeController;

        private AnomalousMvcContext editingContext;

        public MvcEditorContext(AnomalousMvcContext editingContext, String file, MvcTypeController mvcTypeController)
        {
            this.mvcTypeController = mvcTypeController;
            this.currentFile = file;
            this.editingContext = editingContext;

            mvcContext = new AnomalousMvcContext();

            mvcContext.Models.add(new EditMenuManager());

            GenericPropertiesFormView genericPropertiesView = new GenericPropertiesFormView("MvcContext", editingContext.getEditInterface(), true);
            genericPropertiesView.ViewLocation = ViewLocations.Right;
            genericPropertiesView.IsWindow = true;
            mvcContext.Views.add(genericPropertiesView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            //taskbar.addTask(new RunMvcContextActionTask("Close", "Close Rml File", "NoIcon", "File", "Editor/CloseCurrentFile", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "FileToolstrip/Save", "File", "Editor/Save", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "NoIcon", "Edit", "Editor/Cut", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "NoIcon", "Edit", "Editor/Copy", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "NoIcon", "Edit", "Editor/Paste", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "NoIcon", "Edit", "Editor/SelectAll", mvcContext));
            mvcContext.Views.add(taskbar);

            MvcController timelineEditorController = new MvcController("Editor");
            RunCommandsAction showAction = new RunCommandsAction("Show");
            showAction.addCommand(new ShowViewCommand("MvcContext"));
            showAction.addCommand(new ShowViewCommand("InfoBar"));
            timelineEditorController.Actions.add(showAction);
            RunCommandsAction closeAction = new RunCommandsAction("Close");
            closeAction.addCommand(new CloseAllViewsCommand());
            timelineEditorController.Actions.add(closeAction);
            timelineEditorController.Actions.add(new CallbackAction("CloseCurrentFile", context =>
            {
                close();
                context.runAction("Editor/Close");
            }));
            timelineEditorController.Actions.add(new CallbackAction("Save", context =>
            {
                save();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Cut", context =>
            {
                //textEditorComponent.cut();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Copy", context =>
            {
                //textEditorComponent.copy();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Paste", context =>
            {
                //textEditorComponent.paste();
            }));
            timelineEditorController.Actions.add(new CallbackAction("SelectAll", context =>
            {
                //textEditorComponent.selectAll();
            }));
            mvcContext.Controllers.add(timelineEditorController);
            MvcController common = new MvcController("Common");
            RunCommandsAction startup = new RunCommandsAction("Start");
            startup.addCommand(new RunActionCommand("Editor/Show"));
            startup.addCommand(new CallbackCommand(context =>
            {
                GlobalContextEventHandler.setEventContext(eventContext);
            }));
            common.Actions.add(startup);
            CallbackAction shutdown = new CallbackAction("Shutdown", context =>
            {
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
                save();
            };
            eventContext.addEvent(saveEvent);
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

        private void save()
        {
            mvcTypeController.saveFile(editingContext, currentFile);
        }
    }
}
