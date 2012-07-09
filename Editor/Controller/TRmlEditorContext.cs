﻿using System;
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
    class TRmlEditorContext
    {
        public event Action<TRmlEditorContext> Focus;
        public event Action<TRmlEditorContext> Blur;

        enum Events
        {
            Save
        }

        private TextEditorComponent textEditorComponent;
        private RmlWidgetComponent rmlComponent;
        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private TRmlTypeController trmlTypeController;
        private String rmlPreviewFile;

        public TRmlEditorContext(String file, String rmlPreviewFile, TRmlTypeController trmlTypeController)
        {
            this.trmlTypeController = trmlTypeController;
            this.currentFile = file;
            this.rmlPreviewFile = rmlPreviewFile;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            TextEditorView textEditorView = new TextEditorView("RmlEditor", () => this.trmlTypeController.loadText(currentFile), wordWrap: false);
            textEditorView.ViewLocation = ViewLocations.Left;
            textEditorView.IsWindow = true;
            textEditorView.ComponentCreated += (view, component) =>
            {
                textEditorComponent = component;
            };
            mvcContext.Views.add(textEditorView);

            RmlView rmlView = new RmlView("RmlView");
            rmlView.ViewLocation = ViewLocations.Left;
            rmlView.IsWindow = true;
            rmlView.RmlFile = rmlPreviewFile;
            rmlView.ComponentCreated += (view, component) =>
            {
                rmlComponent = component;
            };
            mvcContext.Views.add(rmlView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "FileToolstrip/Save", "File", "Editor/Save", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "Edit", "Editor/Cut", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "Edit", "Editor/Copy", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "Edit", "Editor/Paste", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "Edit", "Editor/SelectAll", mvcContext));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor",
                new RunCommandsAction("Show",
                    new ShowViewCommand("RmlView"),
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
            if (textEditorComponent != null)
            {
                trmlTypeController.saveFile(textEditorComponent.Text, currentFile);
                if (rmlComponent != null)
                {
                    rmlComponent.reloadDocument(rmlPreviewFile);
                }
            }
        }
    }
}