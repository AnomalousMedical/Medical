using System;
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

        private EditorController editorController;
        private String currentFile;
        private Timeline currentTimeline;

        public event TimelineTypeEvent TimelineChanged;

        public TimelineTypeController(EditorController editorController)
            :base(".tl", editorController)
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);

            //extensionActions.Add(new ExtensionAction("Close Timeline", "File", close));
            //extensionActions.Add(new ExtensionAction("Save Timeline", "File", saveTimeline));
            //extensionActions.Add(new ExtensionAction("Save Timeline As", "File", saveTimelineAs));
            //extensionActions.Add(new ExtensionAction("Cut", "Edit", editor.cut));
            //extensionActions.Add(new ExtensionAction("Copy", "Edit", editor.copy));
            //extensionActions.Add(new ExtensionAction("Paste", "Edit", editor.paste));
            //extensionActions.Add(new ExtensionAction("Select All", "Edit", editor.selectAll));
            //Also need to handle synchronizing the timeline editor marker with the prop timeline editor marker

            //CURRENT WORKING
            //Make it so you can add items to the EditorInfoBarView menus
            //Add the timeline extension actions above to that menu, link it back to callback actions in the controller
            //Create an interface and some actions for cut, copy, paste, add these to the context
            //make the timeline editor extend this interface and add itself to the model, this way the actions can read
            //from the model and run the cut, copy, paste, select all functions
        }

        public override void openFile(string path)
        {
            currentTimeline = (Timeline)loadObject(path);
            currentFile = path;
            if (TimelineChanged != null)
            {
                TimelineChanged.Invoke(this, currentTimeline);
            }

            AnomalousMvcContext mvcContext = new AnomalousMvcContext();
            mvcContext.Views.add(new TimelineEditorView("TimelineEditor", currentTimeline));
            mvcContext.Views.add(new GenericEditorView("TimelinePropertiesEditor", currentTimeline.getEditInterface()));
            EditorInfoBarView infoBar = new EditorInfoBarView("TimelineInfoBar", String.Format("{0} - Timeline", currentFile), "TimelineEditor/Close");
            infoBar.addAction(new EditorInfoBarAction("Close Timeline", "File", "TimelineEditor/CloseTimeline"));
            infoBar.addAction(new EditorInfoBarAction("Save Timeline", "File", "TimelineEditor/Save"));
            infoBar.addAction(new EditorInfoBarAction("Save Timeline As", "File", "TimelineEditor/SaveAs"));
            infoBar.addAction(new EditorInfoBarAction("Cut", "Edit", "TimelineEditor/Cut"));
            infoBar.addAction(new EditorInfoBarAction("Copy", "Edit", "TimelineEditor/Copy"));
            infoBar.addAction(new EditorInfoBarAction("Paste", "Edit", "TimelineEditor/Paste"));
            infoBar.addAction(new EditorInfoBarAction("Select All", "Edit", "TimelineEditor/SelectAll"));
            mvcContext.Views.add(infoBar);
            MvcController timelineEditorController = new MvcController("TimelineEditor");
            RunCommandsAction showAction = new RunCommandsAction("Show");
            showAction.addCommand(new ShowViewCommand("TimelineEditor"));
            showAction.addCommand(new ShowViewCommand("TimelinePropertiesEditor"));
            showAction.addCommand(new ShowViewCommand("TimelineInfoBar"));
            timelineEditorController.Actions.add(showAction);
            RunCommandsAction closeAction = new RunCommandsAction("Close");
            closeAction.addCommand(new CloseAllViewsCommand());
            timelineEditorController.Actions.add(closeAction);
            timelineEditorController.Actions.add(new CallbackAction("CloseTimeline", context =>
            {
                closeTimeline();
                context.runAction("TimelineEditor/Close");
            }));
            timelineEditorController.Actions.add(new CallbackAction("Save", context =>
            {
                saveTimeline();
            }));
            timelineEditorController.Actions.add(new CallbackAction("SaveAs", context =>
            {
                saveTimelineAs();
            }));
            timelineEditorController.Actions.add(new CutAction());
            timelineEditorController.Actions.add(new CopyAction());
            timelineEditorController.Actions.add(new PasteAction());
            timelineEditorController.Actions.add(new SelectAllAction());
            mvcContext.Controllers.add(timelineEditorController);
            MvcController common = new MvcController("Common");
            RunCommandsAction startup = new RunCommandsAction("Start");
            startup.addCommand(new RunActionCommand("TimelineEditor/Show"));
            common.Actions.add(startup);
            CallbackAction shutdown = new CallbackAction("Shutdown", context =>
            {
                
            });
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

        private void saveTimeline()
        {
            if (currentTimeline != null)
            {
                saveTimeline(currentTimeline, currentFile);
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
                    saveTimeline(currentTimeline, saveDialog.Path);
                }
            }
        }

        private void saveTimeline(Timeline timeline, String filename)
        {
            try
            {
                saveObject(filename, timeline);
                currentFile = filename;
                //editor.updateFileName(currentFile);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your timeline to\n'{0}'\nPlease make sure that destination is valid.", filename), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Could not save timeline. {0}", ex.Message);
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            closeTimeline();
        }

        private void closeTimeline()
        {
            currentTimeline = null;
            //editor.updateFileName(null);
            closeCurrentCachedResource();
        }
    }
}
