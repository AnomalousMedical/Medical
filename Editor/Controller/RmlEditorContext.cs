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

            TextEditorView textEditorView = new TextEditorView("RmlEditor", () => rmlComponent.CurrentRml, wordWrap: false, textHighlighter: RmlTextHighlighter.Instance);
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

            DragAndDropView<HtmlDragDropItem> htmlDragDrop = new DragAndDropView<HtmlDragDropItem>("HtmlDragDrop",
                new HtmlDragDropItem("Heading", CommonResources.NoIcon, "<h1>Heading</h1>"),
                new HtmlDragDropItem("Paragraph", CommonResources.NoIcon, "<p>Add paragraph text here.</p>"),
                new HtmlDragDropItem("Image", CommonResources.NoIcon, "<img src=\"\"></img>"),
                new HtmlDragDropItem("Broken", CommonResources.NoIcon, "<yea this will be fucked/\""),
                new HtmlDragDropItem("Multi", CommonResources.NoIcon, "<h1>Heading For Paragraph.</h1><p>Paragraph for heading.</p>")
                );
            htmlDragDrop.Dragging += (item, position) =>
                {
                    rmlComponent.changeSelectedElement(position);
                };
            htmlDragDrop.DragEnded += (item, position) =>
                {
                    if (rmlComponent.Widget.contains(position.x, position.y))
                    {
                        rmlComponent.insertRawRml(item.Markup);
                    }
                };
            htmlDragDrop.ViewLocation = ViewLocations.Left;
            htmlDragDrop.IsWindow = true;
            mvcContext.Views.add(htmlDragDrop);

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

            mvcContext.Controllers.add(new MvcController("HtmlDragDrop",
                new RunCommandsAction("Show",
                    new ShowViewIfNotOpenCommand("HtmlDragDrop")),
                new RunCommandsAction("Close",
                    new CloseViewCommand())
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
                        if (textEditorComponent != null)
                        {
                            textEditorComponent.cut();
                        }
                    }),
                new CallbackAction("Copy", context =>
                    {
                        if (textEditorComponent != null)
                        {
                            textEditorComponent.copy();
                        }
                    }),
                new CallbackAction("Paste", context =>
                    {
                        if (textEditorComponent != null)
                        {
                            textEditorComponent.paste();
                        }
                    }),
                new CallbackAction("SelectAll", context =>
                    {
                        if (textEditorComponent != null)
                        {
                            textEditorComponent.selectAll();
                        }
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
                            rmlComponent.insertButton(result);
                            return true;
                        });
                    }),
                new CallbackAction("Image", context =>
                    {
                        BrowserWindow<String>.GetInput(uiCallback.createFileBrowser(new String[] { "*.png", "*.jpg", "*jpeg", "*.gif", "*.bmp" }, "Image Files"), true, delegate(String result, ref string errorPrompt)
                        {
                            rmlComponent.insertImage(result);
                            return true;
                        });
                    })));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start", new RunActionCommand("HtmlDragDrop/Show"), new RunActionCommand("Editor/Show")),
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
                saveAll();
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

                if (editingMvcContext.Views.hasItem(controllerName))
                {
                    RmlView view = editingMvcContext.Views[controllerName] as RmlView;
                    if (view != null && view.RmlFile == file)
                    {
                        GenericPropertiesFormView genericPropertiesView = new GenericPropertiesFormView("MvcView", view.getEditInterface(), true);
                        genericPropertiesView.ViewLocation = ViewLocations.Left;
                        genericPropertiesView.IsWindow = true;
                        genericPropertiesView.Buttons.add(new CloseButtonDefinition("Close", "MvcViewEditor/Close"));
                        mvcContext.Views.add(genericPropertiesView);

                        taskbar.addTask(new RunMvcContextActionTask("EditView", "Edit View", "MvcContextEditor/IndividualViewIcon", "Edit", "MvcViewEditor/Show", mvcContext));

                        mvcContext.Controllers.add(new MvcController("MvcViewEditor",
                        new RunCommandsAction("Show",
                            new ShowViewIfNotOpenCommand("MvcView")),
                        new RunCommandsAction("Close",
                            new CloseViewCommand())
                        ));
                    }
                }

                taskbar.addTask(new CallbackTask("PreviewMvc", "Preview", "MvcContextEditor/MVCcomIcon", "", 0, true, (item) =>
                {
                    uiCallback.previewMvcContext(editingMvcContext);
                }));
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
