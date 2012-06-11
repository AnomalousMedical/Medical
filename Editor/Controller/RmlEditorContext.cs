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
    class RmlEditorContext
    {
        public event Action<RmlEditorContext> Shutdown;

        enum Events
        {
            Save
        }

        private TextEditorComponent textEditorComponent;
        private RmlWidgetComponent rmlComponent;
        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private RmlTypeController rmlTypeController;

        public RmlEditorContext(String rmlText, String file, RmlTypeController rmlTypeController)
        {
            this.rmlTypeController = rmlTypeController;
            this.currentFile = file;

            mvcContext = new AnomalousMvcContext();
            TextEditorView textEditorView = new TextEditorView("RmlEditor", rmlText, wordWrap: false);
            textEditorView.ViewLocation = ViewLocations.Floating;
            textEditorView.ComponentCreated += (view, component) =>
            {
                textEditorComponent = component;
            };
            mvcContext.Views.add(textEditorView);
            RmlView rmlView = new RmlView("RmlView");
            rmlView.ViewLocation = ViewLocations.Right;
            rmlView.RmlFile = file;
            rmlView.ComponentCreated += (view, component) =>
            {
                rmlComponent = component;
            };
            mvcContext.Views.add(rmlView);
            EditorInfoBarView infoBar = new EditorInfoBarView("InfoBar", String.Format("{0} - Rml", file), "Editor/Close");
            infoBar.addAction(new EditorInfoBarAction("Close Rml File", "File", "Editor/CloseCurrentFile"));
            infoBar.addAction(new EditorInfoBarAction("Save Rml File", "File", "Editor/Save"));
            infoBar.addAction(new EditorInfoBarAction("Cut", "Edit", "Editor/Cut"));
            infoBar.addAction(new EditorInfoBarAction("Copy", "Edit", "Editor/Copy"));
            infoBar.addAction(new EditorInfoBarAction("Paste", "Edit", "Editor/Paste"));
            infoBar.addAction(new EditorInfoBarAction("Select All", "Edit", "Editor/SelectAll"));
            mvcContext.Views.add(infoBar);
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
                rmlTypeController.saveFile(textEditorComponent.Text, currentFile);
                if (rmlComponent != null)
                {
                    rmlComponent.reloadDocument(currentFile);
                }
            }
        }
    }
}
