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
using Medical.Controller;

namespace Medical
{
    class XmlEditorContext
    {
        public event Action<XmlEditorContext> Focus;
        public event Action<XmlEditorContext> Blur;

        enum Events
        {
            Save
        }

        private TextEditorComponent textEditorComponent;
        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private XmlTypeController typeController;
        private String rmlPreviewFile;

        public XmlEditorContext(String file, String rmlPreviewFile, XmlTypeController typeController)
        {
            this.typeController = typeController;
            this.currentFile = file;
            this.rmlPreviewFile = rmlPreviewFile;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            TextEditorView textEditorView = new TextEditorView("RmlEditor", () => this.typeController.loadText(currentFile), wordWrap: false, textHighlighter:XmlTextHighlighter.Instance);
            textEditorView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left);
            textEditorView.IsWindow = true;
            textEditorView.ComponentCreated += (view, component) =>
            {
                textEditorComponent = component;
            };
            mvcContext.Views.add(textEditorView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "CommonToolstrip/Save", "File", "Editor/Save", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "Edit", "Editor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "Edit", "Editor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "Edit", "Editor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "Edit", "Editor/SelectAll", mvcContext));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor", 
                new RunCommandsAction("Show",
                    new ShowViewCommand("RmlEditor"),
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
                saveAll();
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

        private void saveAll()
        {
            typeController.updateCachedText(currentFile, CurrentText);
            typeController.EditorController.saveAllCachedResources();
        }

        private void save()
        {
            if (textEditorComponent != null)
            {
                typeController.saveFile(textEditorComponent.Text, currentFile);
            }
        }
    }
}
