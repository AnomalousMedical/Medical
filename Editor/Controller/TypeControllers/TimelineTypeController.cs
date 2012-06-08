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
using Medical.Platform;
using Engine.Platform;

namespace Medical
{
    delegate void TimelineTypeEvent(TimelineTypeController typeController, Timeline timeline);

    class TimelineTypeController : SaveableTypeController
    {
        enum Events
        {
            Save,
            TogglePlay,
            Cut,
            Copy,
            Paste,
            SelectAll
        }

        public const String TIMELINE_WILDCARD = "Timelines (*.tl)|*.tl";
        public const String Icon = "TimelineEditorIcon";

        private EditorController editorController;
        private String currentFile;
        private Timeline currentTimeline;
        private TimelineEditorComponent timelineEditorComponent = null;

        public event TimelineTypeEvent TimelineChanged;

        private EventContext eventContext;

        public TimelineTypeController(EditorController editorController)
            :base(".tl", editorController)
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
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
            TimelineEditorView timelineEditorView = new TimelineEditorView("TimelineEditor", currentTimeline);
            timelineEditorView.ComponentCreated += (view, component) =>
            {
                timelineEditorComponent = component;
            };
            mvcContext.Views.add(timelineEditorView);
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
            timelineEditorController.Actions.add(new CallbackAction("Cut", context =>
            {
                timelineEditorComponent.cut();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Copy", context =>
            {
                timelineEditorComponent.copy();
            }));
            timelineEditorController.Actions.add(new CallbackAction("Paste", context =>
            {
                timelineEditorComponent.paste();
            }));
            timelineEditorController.Actions.add(new CallbackAction("SelectAll", context =>
            {
                timelineEditorComponent.selectAll();
            }));
            mvcContext.Controllers.add(timelineEditorController);
            MvcController common = new MvcController("Common");
            RunCommandsAction startup = new RunCommandsAction("Start");
            startup.addCommand(new RunActionCommand("TimelineEditor/Show"));
            startup.addCommand(new CallbackCommand(context =>
            {
                GlobalContextEventHandler.setEventContext(eventContext);
            }));
            common.Actions.add(startup);
            CallbackAction shutdown = new CallbackAction("Shutdown", context =>
            {
                timelineEditorComponent = null;
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
                saveTimeline();
            };
            eventContext.addEvent(saveEvent);
            MessageEvent togglePlayEvent = new MessageEvent(Events.TogglePlay);
            togglePlayEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            togglePlayEvent.addButton(KeyboardButtonCode.KC_SPACE);
            togglePlayEvent.FirstFrameUpEvent += eventManager =>
            {
                timelineEditorComponent.togglePlayPreview();
            };
            eventContext.addEvent(togglePlayEvent);

            MessageEvent cut = new MessageEvent(Events.Cut);
            cut.addButton(KeyboardButtonCode.KC_LCONTROL);
            cut.addButton(KeyboardButtonCode.KC_X);
            cut.FirstFrameUpEvent += eventManager =>
            {
                timelineEditorComponent.cut();
            };
            eventContext.addEvent(cut);

            MessageEvent copy = new MessageEvent(Events.Copy);
            copy.addButton(KeyboardButtonCode.KC_LCONTROL);
            copy.addButton(KeyboardButtonCode.KC_C);
            copy.FirstFrameUpEvent += eventManager =>
            {
                timelineEditorComponent.copy();
            };
            eventContext.addEvent(copy);

            MessageEvent paste = new MessageEvent(Events.Paste);
            paste.addButton(KeyboardButtonCode.KC_LCONTROL);
            paste.addButton(KeyboardButtonCode.KC_V);
            paste.FirstFrameUpEvent += eventManager =>
            {
                timelineEditorComponent.paste();
            };
            eventContext.addEvent(paste);

            MessageEvent selectAll = new MessageEvent(Events.SelectAll);
            selectAll.addButton(KeyboardButtonCode.KC_LCONTROL);
            selectAll.addButton(KeyboardButtonCode.KC_A);
            selectAll.FirstFrameUpEvent += eventManager =>
            {
                timelineEditorComponent.selectAll();
            };
            eventContext.addEvent(selectAll);

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
                editorController.NotificationManager.showNotification(String.Format("{0} saved.", currentFile), Icon, 2);
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
            closeCurrentCachedResource();
        }
    }
}
