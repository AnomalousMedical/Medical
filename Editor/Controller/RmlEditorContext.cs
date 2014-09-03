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
using Medical.Controller;

namespace Medical
{
    class RmlEditorContext
    {
        public event Action<RmlEditorContext> Focus;
        public event Action<RmlEditorContext> Blur;

        enum Events
        {
            Save,
            Undo,
            Redo
        }

        private TextEditorComponent textEditorComponent;
        private RmlWysiwygComponent rmlComponent;
        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private RmlTypeController rmlTypeController;
        private EditorUICallback uiCallback;
        private UndoRedoBuffer undoBuffer;

        public RmlEditorContext(String file, RmlTypeController rmlTypeController, AnomalousMvcContext editingMvcContext, EditorController editorController, EditorUICallback uiCallback)
        {
            this.rmlTypeController = rmlTypeController;
            this.currentFile = file;
            this.uiCallback = uiCallback;

            undoBuffer = new UndoRedoBuffer(50);

            rmlTypeController.loadText(currentFile);

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            TextEditorView textEditorView = new TextEditorView("RmlEditor", () => rmlComponent.CurrentRml, wordWrap: false, textHighlighter: RmlTextHighlighter.Instance);
            textEditorView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left);
            textEditorView.Buttons.add(new CloseButtonDefinition("Close", "RmlTextEditor/Close"));
            textEditorView.ComponentCreated += (view, component) =>
            {
                textEditorComponent = component;
            };
            mvcContext.Views.add(textEditorView);
            
            RmlWysiwygView rmlView = new RmlWysiwygView("RmlView", uiCallback, undoBuffer);
            rmlView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left);
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

            DragAndDropView<WysiwygDragDropItem> htmlDragDrop = new DragAndDropView<WysiwygDragDropItem>("HtmlDragDrop",
                new WysiwygDragDropItem("Heading", "Editor/HeaderIcon", "<h1>Heading</h1>"),
                new WysiwygDragDropItem("Paragraph", "Editor/ParagraphsIcon", "<p>Add paragraph text here.</p>"),
                new WysiwygDragDropItem("Image", "Editor/ImageIcon", String.Format("<img src=\"{0}\" style=\"width:200px;\"></img>", RmlWysiwygComponent.DefaultImage)),
                new WysiwygDragDropItem("Link", "Editor/LinksIcon", "<a onclick=\"None\">Link</a>"),
                new WysiwygDragDropItem("Button", "Editor/AddButtonIcon", "<input type=\"submit\" onclick=\"None\">Button</input>"),
                new WysiwygDragDropItem("Separator", CommonResources.NoIcon, "<x-separator/>"),
                new WysiwygDragDropItem("Two Columns", CommonResources.NoIcon, "<div class=\"TwoColumn\"><div class=\"Column\"><p>Column 1 text goes here.</p></div><div class=\"Column\"><p>Column 2 text goes here.</p></div></div>"),
                new WysiwygDragDropItem("Heading and Paragraph", CommonResources.NoIcon, "<h1>Heading For Paragraph.</h1><p>Paragraph for heading.</p>", "div"),
                new WysiwygDragDropItem("Left Image and Paragraph", CommonResources.NoIcon, String.Format("<div class=\"ImageParagraphLeft\"><img src=\"{0}\" style=\"width:200px;\"/><p>Add paragraph text here.</p></div>", RmlWysiwygComponent.DefaultImage)),
                new WysiwygDragDropItem("Right Image and Paragraph", CommonResources.NoIcon, String.Format("<div class=\"ImageParagraphRight\"><img src=\"{0}\" style=\"width:200px;\"/><p>Add paragraph text here.</p></div>", RmlWysiwygComponent.DefaultImage))
                );
            htmlDragDrop.Dragging += (item, position) =>
                {
                    rmlComponent.setPreviewElement(position, item.PreviewMarkup, item.PreviewTagType);
                };
            htmlDragDrop.DragEnded += (item, position) =>
                {
                    if (rmlComponent.contains(position))
                    {
                        rmlComponent.insertRml(item.createDocumentMarkup());
                    }
                    else
                    {
                        rmlComponent.cancelAndHideEditor();
                        rmlComponent.clearPreviewElement(false);
                    }
                };
            htmlDragDrop.ItemActivated += (item) =>
                {
                    rmlComponent.insertRml(item.createDocumentMarkup());
                };
            htmlDragDrop.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left);
            mvcContext.Views.add(htmlDragDrop);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "CommonToolstrip/Save", "File", "Editor/Save", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Undo", "Undo", CommonResources.NoIcon, "Edit", "Editor/Undo", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Redo", "Redo", CommonResources.NoIcon, "Edit", "Editor/Redo", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "Edit", "Editor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "Edit", "Editor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "Edit", "Editor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "Edit", "Editor/SelectAll", mvcContext));
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
                new CallbackAction("Undo", context =>
                    {
                        undoBuffer.undo();
                    }),
                new CallbackAction("Redo", context =>
                    {
                        undoBuffer.execute();
                    })
                ));

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
            MessageEvent saveEvent = new MessageEvent(Events.Save, EventLayers.Gui);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                saveAll();
            };
            eventContext.addEvent(saveEvent);

            MessageEvent undoEvent = new MessageEvent(Events.Undo, EventLayers.Gui);
            undoEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            undoEvent.addButton(KeyboardButtonCode.KC_Z);
            undoEvent.FirstFrameUpEvent += eventManager =>
            {
                undoBuffer.undo();
            };
            eventContext.addEvent(undoEvent);

            MessageEvent redoEvent = new MessageEvent(Events.Redo, EventLayers.Gui);
            redoEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            redoEvent.addButton(KeyboardButtonCode.KC_Y);
            redoEvent.FirstFrameUpEvent += eventManager =>
            {
                undoBuffer.execute();
            };
            eventContext.addEvent(redoEvent);

            if (editingMvcContext != null)
            {
                String controllerName = PathExtensions.RemoveExtension(file);
                if(editingMvcContext.Controllers.hasItem(controllerName))
                {
                    MvcController viewController = editingMvcContext.Controllers[controllerName];

                    GenericPropertiesFormView genericPropertiesView = new GenericPropertiesFormView("MvcContext", viewController.getEditInterface(), editorController, uiCallback, true);
                    genericPropertiesView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left);
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
                        GenericPropertiesFormView genericPropertiesView = new GenericPropertiesFormView("MvcView", view.getEditInterface(), editorController, uiCallback, true);
                        genericPropertiesView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left);
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
            preSave();
            rmlTypeController.updateCachedText(currentFile, CurrentText);
            rmlTypeController.EditorController.saveAllCachedResources();
            postSave();
        }

        private void save()
        {
            preSave();
            rmlTypeController.saveFile(CurrentText, currentFile);
            postSave();
        }

        private void preSave()
        {
            if (rmlComponent != null)
            {
                rmlComponent.aboutToSaveRml();
            }
        }

        private void postSave()
        {
            if (textEditorComponent != null)
            {
                if (textEditorComponent.ChangesMade)
                {
                    if (rmlComponent != null)
                    {
                        String undoRml = rmlComponent.UnformattedRml;
                        rmlComponent.reloadDocument();
                        rmlComponent.updateUndoStatus(undoRml, true);
                    }
                    textEditorComponent.resetChangesMade();
                }
            }
        }
    }
}
