using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Medical.GUI;
using System.IO;
using MyGUIPlugin;
using Medical.Platform;
using Engine.Platform;

namespace Medical
{
    class RcssEditorContext
    {
        public event Action<RcssEditorContext> Shutdown;

        enum Events
        {
            Save
        }

        private TextEditorComponent textEditorComponent;
        private RmlWidgetComponent rmlComponent;
        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private RcssTypeController rcssTypeController;
        private String rmlPreviewFile;

        public RcssEditorContext(String rcssText, String file, String rmlPreviewFile, RcssTypeController rcssTypeController)
        {
            this.rcssTypeController = rcssTypeController;
            this.currentFile = file;
            this.rmlPreviewFile = rmlPreviewFile;

            mvcContext = new AnomalousMvcContext();
            TextEditorView textEditorView = new TextEditorView("RmlEditor", rcssText, wordWrap: false);
            textEditorView.ViewLocation = ViewLocations.Floating;
            textEditorView.IsWindow = true;
            textEditorView.ComponentCreated += (view, component) =>
            {
                textEditorComponent = component;
            };
            mvcContext.Views.add(textEditorView);

            RmlView rmlView = new RmlView("RmlView");
            rmlView.ViewLocation = ViewLocations.Right;
            rmlView.IsWindow = true;
            rmlView.RmlFile = rmlPreviewFile;
            rmlView.ComponentCreated += (view, component) =>
            {
                rmlComponent = component;
            };
            mvcContext.Views.add(rmlView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", String.Format("{0} - Rml", currentFile), "Editor/Close");
            //taskbar.addTask(new RunMvcContextActionTask("Close", "Close Rml File", "NoIcon", "File", "Editor/CloseCurrentFile", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "FileToolstrip/Save", "File", "Editor/Save", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "NoIcon", "Edit", "Editor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "NoIcon", "Edit", "Editor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "NoIcon", "Edit", "Editor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "NoIcon", "Edit", "Editor/SelectAll", mvcContext));
            mvcContext.Views.add(taskbar);

            MvcController timelineEditorController = new MvcController("Editor");
            RunCommandsAction showAction = new RunCommandsAction("Show");
            showAction.addCommand(new ShowViewCommand("RmlEditor"));
            showAction.addCommand(new ShowViewCommand("RmlView"));
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
                textEditorComponent.cut();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Copy", context =>
            {
                textEditorComponent.copy();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Paste", context =>
            {
                textEditorComponent.paste();
            }));
            timelineEditorController.Actions.add(new CallbackAction("SelectAll", context =>
            {
                textEditorComponent.selectAll();
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
            if (textEditorComponent != null)
            {
                rcssTypeController.saveFile(textEditorComponent.Text, currentFile);
                if (rmlComponent != null)
                {
                    rmlComponent.reloadDocument(rmlPreviewFile);
                }
            }
        }
    }
}
