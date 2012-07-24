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
        public event Action<RmlEditorContext> Focus;
        public event Action<RmlEditorContext> Blur;

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
        private EditorUICallback uiCallback;

        public RmlEditorContext(String file, RmlTypeController rmlTypeController, EditorUICallback uiCallback, AnomalousMvcContext editingMvcContext)
        {
            this.rmlTypeController = rmlTypeController;
            this.currentFile = file;
            this.uiCallback = uiCallback;

            rmlTypeController.loadText(currentFile);

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            TextEditorView textEditorView = new TextEditorView("RmlEditor", () => rmlComponent.CurrentRml, wordWrap: false);
            textEditorView.ViewLocation = ViewLocations.Left;
            textEditorView.IsWindow = true;
            textEditorView.Buttons.add(new CloseButtonDefinition("Close", "RmlTextEditor/Close"));
            textEditorView.ComponentCreated += (view, component) =>
            {
                textEditorComponent = component;
            };
            mvcContext.Views.add(textEditorView);
            RmlWysiwygView rmlView = new RmlWysiwygView("RmlView", uiCallback, uiCallback);
            rmlView.ViewLocation = ViewLocations.Left;
            rmlView.IsWindow = true;
            rmlView.RmlFile = file;
            rmlView.ComponentCreated += (view, component) =>
            {
                rmlComponent = component;
                rmlComponent.RmlEdited += rmlEditor =>
                {
                    if (textEditorComponent != null)
                    {
                        textEditorComponent.Text = rmlEditor.CurrentRml;
                    }
                };
            };
            mvcContext.Views.add(rmlView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            {
                saveAll();
            }));
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
            taskbar.addTask(new RunMvcContextActionTask("RmlEditor", "Edit Rml", RmlTypeController.Icon, "Edit", "RmlTextEditor/Show", mvcContext));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("RmlTextEditor",
                new RunCommandsAction("Show",
                    new ShowViewIfNotOpenCommand("RmlEditor")),
                new RunCommandsAction("Close",
                    new CloseViewCommand(),
                    new CallbackCommand(context =>
                        {
                            textEditorComponent = null;
                        }))
                    ));

            mvcContext.Controllers.add(new MvcController("Editor",
                new RunCommandsAction("Show",
                    new ShowViewCommand("RmlView"),
                    new ShowViewCommand("InfoBar")),
                new RunCommandsAction("Close", new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                    {
                        save();
                    }),
                new CallbackAction("Cut", context =>
                    {
                        textEditorComponent.cut();
                    }),
                new CallbackAction("Copy", context =>
                    {
                        textEditorComponent.copy();
                    }),
                new CallbackAction("Paste", context =>
                    {
                        textEditorComponent.paste();
                    }),
                new CallbackAction("SelectAll", context =>
                    {
                        textEditorComponent.selectAll();
                    }),
                new CallbackAction("Paragraph", context =>
                    {
                        rmlComponent.insertParagraph();
                    }),
                new CallbackAction("Header", context =>
                    {
                        rmlComponent.insertHeader1();
                    }),
                new CallbackAction("ActionLink", context =>
                    {
                        BrowserWindow<String>.GetInput(uiCallback.createActionBrowser(), true, delegate(String result, ref string errorPrompt)
                        {
                            rmlComponent.insertLink(result);
                            return true;
                        });
                    }),
                new CallbackAction("Button", context =>
                    {
                        BrowserWindow<String>.GetInput(uiCallback.createActionBrowser(), true, delegate(String result, ref string errorPrompt)
                        {
                            //textEditorComponent.insertText(String.Format("<input type=\"submit\" onclick=\"{0}\">Empty Button</input>", result));
                            rmlComponent.insertButton(result);
                            return true;
                        });
                    }),
                new CallbackAction("Image", context =>
                    {
                        BrowserWindow<String>.GetInput(uiCallback.createFileBrowser("*.png", "Image Files"), true, delegate(String result, ref string errorPrompt)
                        {
                            //textEditorComponent.insertText(String.Format("<input type=\"submit\" onclick=\"{0}\">Empty Button</input>", result));
                            rmlComponent.insertImage(result);
                            return true;
                        });
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
            MessageEvent saveEvent = new MessageEvent(Events.Save);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                save();
            };
            eventContext.addEvent(saveEvent);

            if (editingMvcContext != null)
            {
                String controllerName = PathExtensions.RemoveExtension(file);
                if(editingMvcContext.Controllers.hasItem(controllerName))
                {
                    MvcController viewController = editingMvcContext.Controllers[controllerName];

                    GenericPropertiesFormView genericPropertiesView = new GenericPropertiesFormView("MvcContext", viewController.getEditInterface(), true);
                    genericPropertiesView.ViewLocation = ViewLocations.Left;
                    genericPropertiesView.IsWindow = true;
                    genericPropertiesView.Buttons.add(new CloseButtonDefinition("Close", "MvcEditor/Close"));
                    mvcContext.Views.add(genericPropertiesView);

                    taskbar.addTask(new RunMvcContextActionTask("EditActions", "Edit Actions", "MvcContextEditor/ControllerIcon", "Edit", "MvcEditor/Show", mvcContext));

                    mvcContext.Controllers.add(new MvcController("MvcEditor",
                    new RunCommandsAction("Show",
                        new ShowViewIfNotOpenCommand("MvcContext")),
                    new RunCommandsAction("Close",
                        new CloseViewCommand())
                    ));
                }
            }
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
                //If the text editor is open it is the master, if it is not then
                //use the rml wysiwyg
                if (textEditorComponent != null)
                {
                    return textEditorComponent.Text;
                }
                else
                {
                    return rmlComponent.CurrentRml;
                }
            }
        }

        public string CurrentFile
        {
            get
            {
                return currentFile;
            }
        }

        private void saveAll()
        {
            if (rmlComponent != null)
            {
                rmlComponent.aboutToSaveRml();
            }
            rmlTypeController.updateCachedText(currentFile, CurrentText);
            rmlTypeController.EditorController.saveAllCachedResources();
            if (rmlComponent != null)
            {
                rmlComponent.reloadDocument(currentFile);
            }
        }

        private void save()
        {
            if (rmlComponent != null)
            {
                rmlComponent.aboutToSaveRml();
            }
            rmlTypeController.saveFile(CurrentText, currentFile);
            if (rmlComponent != null)
            {
                rmlComponent.reloadDocument(currentFile);
            }
        }
    }
}
