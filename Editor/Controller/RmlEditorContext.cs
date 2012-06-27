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
using Medical.Editor;

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
        private RmlWysiwygComponent rmlComponent;
        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private RmlTypeController rmlTypeController;

        public RmlEditorContext(String rmlText, String file, RmlTypeController rmlTypeController)
        {
            this.rmlTypeController = rmlTypeController;
            this.currentFile = file;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.ShutdownAction = "Common/Shutdown";

            TextEditorView textEditorView = new TextEditorView("RmlEditor", rmlText, wordWrap: false);
            textEditorView.ViewLocation = ViewLocations.Floating;
            textEditorView.IsWindow = true;
            textEditorView.ComponentCreated += (view, component) =>
            {
                textEditorComponent = component;
            };
            mvcContext.Views.add(textEditorView);
            RmlWysiwygView rmlView = new RmlWysiwygView("RmlView");
            rmlView.ViewLocation = ViewLocations.Left;
            rmlView.IsWindow = true;
            rmlView.RmlFile = file;
            rmlView.ComponentCreated += (view, component) =>
            {
                rmlComponent = component;
                rmlComponent.RmlEdited += rmlEditor =>
                {
                    textEditorComponent.Text = rmlEditor.CurrentRml;
                };
            };
            mvcContext.Views.add(rmlView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            //taskbar.addTask(new RunMvcContextActionTask("Close", "Close Rml File", "NoIcon", "File", "Editor/CloseCurrentFile", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "FileToolstrip/Save", "File", "Editor/Save", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "Edit", "Editor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "Edit", "Editor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "Edit", "Editor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "Edit", "Editor/SelectAll", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paragraph", "Paragraph", "Editor/ParagraphsIcon", "Edit", "Editor/Paragraph", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Header", "Header", "Editor/HeaderIcon", "Edit", "Editor/Header", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("ActionLink", "Action Link", "Editor/LinksIcon", "Edit", "Editor/ActionLink", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Button", "Button", "Editor/AddButtonIcon", "Edit", "Editor/Button", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Image", "Image", "Editor/ImageIcon", "Edit", "Editor/Image", mvcContext));
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
            timelineEditorController.Actions.add(new CallbackAction("Paragraph", context =>
            {
                //textEditorComponent.insertText("<p>Empty Paragraph</p>");
                rmlComponent.insertParagraph();

            }));
            timelineEditorController.Actions.add(new CallbackAction("Header", context =>
            {
                //textEditorComponent.insertText("<h1>Empty Header</h1>\n");
                rmlComponent.insertHeader1();
            }));
            timelineEditorController.Actions.add(new CallbackAction("ActionLink", context =>
            {
                BrowserWindow<String>.GetInput(BrowserWindowController.createActionBrowser(), true, delegate(String result, ref string errorPrompt)
                {
                    rmlComponent.insertLink(result);
                    //textEditorComponent.insertText(String.Format("<a onclick=\"{0}\">Empty Link</a>", result));
                    return true;
                });
            }));
            timelineEditorController.Actions.add(new CallbackAction("Button", context =>
            {
                BrowserWindow<String>.GetInput(BrowserWindowController.createActionBrowser(), true, delegate(String result, ref string errorPrompt)
                {
                    //textEditorComponent.insertText(String.Format("<input type=\"submit\" onclick=\"{0}\">Empty Button</input>", result));
                    rmlComponent.insertButton(result);
                    return true;
                });
            }));
            timelineEditorController.Actions.add(new CallbackAction("Image", context =>
            {
                BrowserWindow<String>.GetInput(BrowserWindowController.createFileBrowser("*.png", "Image Files"), true, delegate(String result, ref string errorPrompt)
                {
                    //textEditorComponent.insertText(String.Format("<input type=\"submit\" onclick=\"{0}\">Empty Button</input>", result));
                    rmlComponent.insertImage(result);
                    return true;
                });
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

        public string CurrentText
        {
            get
            {
                return textEditorComponent.Text;
            }
        }

        public string CurrentFile
        {
            get
            {
                return currentFile;
            }
        }

        private void save()
        {
            if (rmlComponent != null)
            {
                rmlComponent.aboutToSaveRml();
            }
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
