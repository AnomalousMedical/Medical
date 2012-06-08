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
        public const String Icon = "TimelineEditorIcon";

        public event TimelineTypeEvent TimelineChanged;

        private EditorController editorController;
        private TimelineEditorContext timelineEditorContext;

        public TimelineTypeController(EditorController editorController)
            :base(".tl", editorController)
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
        }

        public override void openFile(string path)
        {
            Timeline timeline = (Timeline)loadObject(path);
            if (TimelineChanged != null)
            {
                TimelineChanged.Invoke(this, timeline);
            }
            timelineEditorContext = new TimelineEditorContext(timeline, path, this);
            timelineEditorContext.Shutdown += new Action<TimelineEditorContext>(timelineEditorContext_Shutdown);

            editorController.runEditorContext(timelineEditorContext.MvcContext);
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

        public void saveTimeline(Timeline timeline, String filename)
        {
            try
            {
                saveObject(filename, timeline);
                editorController.NotificationManager.showNotification(String.Format("{0} saved.", filename), Icon, 2);
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

        public void closeTimeline()
        {
            if (timelineEditorContext != null)
            {
                timelineEditorContext.close();
            }
            closeCurrentCachedResource();
        }

        void timelineEditorContext_Shutdown(TimelineEditorContext obj)
        {
            if (obj == timelineEditorContext)
            {
                timelineEditorContext = null;
            }
        }
    }
}
