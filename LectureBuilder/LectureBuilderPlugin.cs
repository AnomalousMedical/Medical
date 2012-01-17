using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.GUI;

namespace LectureBuilder
{
    class LectureBuilderPlugin : AtlasPlugin
    {
        private LectureBuilderWindow lectureBuilderWindow = null;
        private TimelineController lectureTimelineController;

        public LectureBuilderPlugin()
        {

        }

        public void Dispose()
        {
            if (lectureBuilderWindow != null)
            {
                lectureBuilderWindow.Dispose();
            }
        }

        public void loadGUIResources()
        {
            Gui.Instance.load("LectureBuilder.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            lectureTimelineController = new TimelineController(standaloneController);

            GUIManager guiManager = standaloneController.GUIManager;

            lectureBuilderWindow = new LectureBuilderWindow(lectureTimelineController, standaloneController.TimelineController);
            guiManager.addManagedDialog(lectureBuilderWindow);

            TaskController taskController = standaloneController.TaskController;
            taskController.addTask(new MDIDialogOpenTask(lectureBuilderWindow, "LectureBuilder", "Lecture Builder", "", TaskMenuCategories.Editor));
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        public void sceneRevealed()
        {
            
        }

        public long PluginId
        {
            get
            {
                return 10;
            }
        }

        public string PluginName
        {
            get
            {
                return "Lecture Builder";
            }
        }

        public string BrandingImageKey
        {
            get
            {
                return "LectureBuilder/BrandingImage";
            }
        }

        public String Location
        {
            get
            {
                return GetType().Assembly.Location;
            }
        }

        public Version Version
        {
            get
            {
                return GetType().Assembly.GetName().Version;
            }
        }
    }
}
