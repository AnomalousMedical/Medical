﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using System.Xml;
using MyGUIPlugin;
using Logging;
using Medical.Controller.AnomalousMvc;

namespace Medical
{
    delegate void TimelineTypeEvent(TimelineTypeController typeController, Timeline timeline);

    class TimelineTypeController : SaveableTypeController
    {
        public const String TIMELINE_WILDCARD = "Timelines (*.tl)|*.tl";

        private TimelineEditor editor;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private EditorController editorController;
        private String currentFile;

        public event TimelineTypeEvent TimelineChanged;

        public TimelineTypeController(TimelineEditor editor, EditorController editorController)
            :base(".tl", editorController)
        {
            this.editor = editor;
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
            editor.GotFocus += new EventHandler(editor_GotFocus);

            extensionActions.Add(new ExtensionAction("Close Timeline", "File", close));
            extensionActions.Add(new ExtensionAction("Save Timeline", "File", saveTimeline));
            extensionActions.Add(new ExtensionAction("Save Timeline As", "File", saveTimelineAs));
            extensionActions.Add(new ExtensionAction("Cut", "Edit", editor.cut));
            extensionActions.Add(new ExtensionAction("Copy", "Edit", editor.copy));
            extensionActions.Add(new ExtensionAction("Paste", "Edit", editor.paste));
            extensionActions.Add(new ExtensionAction("Select All", "Edit", editor.selectAll));
        }

        public override void openFile(string path)
        {
            editor.CurrentTimeline = (Timeline)loadObject(path);
            currentFile = path;
            editor.updateFileName(currentFile);
            //if (!editor.Visible)
            //{
            //    editor.Visible = true;
            //}
            editorController.ExtensionActions = extensionActions;
            //editor.bringToFront();
            if (TimelineChanged != null)
            {
                TimelineChanged.Invoke(this, editor.CurrentTimeline);
            }

            AnomalousMvcContext mvcContext = new AnomalousMvcContext();
            mvcContext.Views.add(new TimelineEditorView("TimelineEditor", editor.CurrentTimeline));
            mvcContext.Views.add(new GenericEditorView("TimelinePropertiesEditor", editor.CurrentTimeline.getEditInterface()));
            MvcController timelineEditorController = new MvcController("TimelineEditor");
            RunCommandsAction showAction = new RunCommandsAction("Show");
            showAction.addCommand(new ShowViewCommand("TimelineEditor"));
            showAction.addCommand(new ShowViewCommand("TimelinePropertiesEditor"));
            timelineEditorController.Actions.add(showAction);
            mvcContext.Controllers.add(timelineEditorController);
            MvcController common = new MvcController("Common");
            RunCommandsAction startup = new RunCommandsAction("Start");
            startup.addCommand(new RunActionCommand("TimelineEditor/Show"));
            common.Actions.add(startup);
            RunCommandsAction shutdown = new RunCommandsAction("Shutdown");
            common.Actions.add(shutdown);
            mvcContext.Controllers.add(common);

            editorController.runEditorContext(mvcContext);
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Timeline", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Timeline Name", "Enter a name for the timeline.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".tl");
                    if (editorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewTimeline(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewTimeline(filePath);
                    }
                    return true;
                });
            }));
        }

        void createNewTimeline(String filePath)
        {
            Timeline timeline = new Timeline();
            creatingNewFile(filePath);
            saveObject(filePath, timeline);
            openFile(filePath);
        }

        void editor_GotFocus(object sender, EventArgs e)
        {
            editorController.ExtensionActions = extensionActions;
        }

        private void saveTimeline()
        {
            if (editor.CurrentTimeline != null)
            {
                saveTimeline(editor.CurrentTimeline, currentFile);
            }
            else
            {
                saveTimelineAs();
            }
        }

        private void saveTimelineAs()
        {
            using (FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Save Timeline", "", "", TIMELINE_WILDCARD))
            {
                if (saveDialog.showModal() == NativeDialogResult.OK)
                {
                    saveTimeline(editor.CurrentTimeline, saveDialog.Path);
                }
            }
        }

        private void saveTimeline(Timeline timeline, String filename)
        {
            try
            {
                saveObject(filename, timeline);
                currentFile = filename;
                editor.updateFileName(currentFile);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your timeline to\n'{0}'\nPlease make sure that destination is valid.", filename), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Could not save timeline. {0}", ex.Message);
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            close();
        }

        private void close()
        {
            editor.CurrentTimeline = null;
            editor.updateFileName(null);
            closeCurrentCachedResource();
        }
    }
}
