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
        enum Events
        {
            Save
        }

        private TextEditorComponent textEditorComponent;
        private RmlWidgetComponent rmlComponent;
        private EditorController editorController;
        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;

        public RmlEditorContext(EditorController editorController, String file)
        {
            this.editorController = editorController;
            this.currentFile = file;

            String rmlText = null;
            using (StreamReader streamReader = new StreamReader(editorController.ResourceProvider.openFile(file)))
            {
                rmlText = streamReader.ReadToEnd();
            }

            mvcContext = new AnomalousMvcContext();
            TextEditorView textEditorView = new TextEditorView("RmlEditor", rmlText, wordWrap: false);
            textEditorView.ViewLocation = ViewLocations.Left;
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
            infoBar.addAction(new EditorInfoBarAction("Save Rml File As", "File", "Editor/SaveAs"));
            //infoBar.addAction(new EditorInfoBarAction("Cut", "Edit", "Editor/Cut"));
            //infoBar.addAction(new EditorInfoBarAction("Copy", "Edit", "Editor/Copy"));
            //infoBar.addAction(new EditorInfoBarAction("Paste", "Edit", "Editor/Paste"));
            //infoBar.addAction(new EditorInfoBarAction("Select All", "Edit", "Editor/SelectAll"));
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
            timelineEditorController.Actions.add(new CallbackAction("SaveAs", context =>
            {
                saveAs();
            }));
            //timelineEditorController.Actions.add(new CutAction());
            //timelineEditorController.Actions.add(new CopyAction());
            //timelineEditorController.Actions.add(new PasteAction());
            //timelineEditorController.Actions.add(new SelectAllAction());
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

        public void save()
        {
            if (textEditorComponent != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(editorController.ResourceProvider.openWriteStream(currentFile)))
                {
                    streamWriter.Write(textEditorComponent.Text);
                }
                if (rmlComponent != null)
                {
                    rmlComponent.reloadDocument(currentFile);
                }
            }
            else
            {
                saveAs();
            }
        }

        public void saveAs()
        {
            //if (currentEditor != null)
            //{
            //    using (FileSaveDialog fileDialog = new FileSaveDialog(MainWindow.Instance, "Save a MVC Context", "", "", WILDCARD))
            //    {
            //        if (fileDialog.showModal() == NativeDialogResult.OK)
            //        {
            //            try
            //            {
            //                using (StreamWriter streamWriter = new StreamWriter(fileDialog.Path))
            //                {
            //                    streamWriter.Write(currentEditor.Text);
            //                }
            //                currentEditor.CurrentFile = fileDialog.Path;
            //                currentEditor.Caption = String.Format("{0} - Rml Text Editor", currentEditor.CurrentFile);
            //            }
            //            catch (Exception e)
            //            {
            //                MessageBox.show("Save error", String.Format("Exception saving RML File to {0}:\n{1}.", fileDialog.Path, e.Message), MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            //            }
            //        }
            //    }
            //}
        }

        public AnomalousMvcContext MvcContext
        {
            get
            {
                return mvcContext;
            }
        }

        private void close()
        {
            //if (currentEditor != null)
            //{
            //    currentEditor.Visible = false;
            //}
        }
    }
}
