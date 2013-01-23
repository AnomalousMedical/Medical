using Lecture.GUI;
using Medical;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture
{
    class LecturePlugin : AtlasPlugin
    {
        private SlideshowExplorer slideshowExplorer;
        private EditorController slideshowEditorController;
        private SlideshowEditController slideshowEditController;
        private TimelineController editorTimelineController;
        private EditorUICallback editorUICallback;
        private PropEditController propEditController;
        private SimObjectMover propMover;

        public LecturePlugin()
        {

        }

        public void Dispose()
        {
            slideshowExplorer.Dispose();
            slideshowEditorController.Dispose();
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Lecture.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;

            editorTimelineController = new TimelineController(standaloneController);
            guiManager.giveGUIsToTimelineController(editorTimelineController);

            slideshowEditorController = new EditorController(standaloneController, editorTimelineController);
            standaloneController.DocumentController.addDocumentHandler(new SlideshowDocumentHandler(slideshowEditorController));

            //Prop Mover
            MedicalController medicalController = standaloneController.MedicalController;
            propMover = new SimObjectMover("Props", medicalController.PluginManager, medicalController.EventManager);
            medicalController.FixedLoopUpdate += propMover.update;

            propEditController = new PropEditController(propMover);

            editorUICallback = new EditorUICallback(standaloneController, slideshowEditorController, propEditController);

            slideshowEditController = new SlideshowEditController(standaloneController, editorUICallback, this.propEditController, slideshowEditorController);
            slideshowExplorer = new SlideshowExplorer(slideshowEditorController, slideshowEditController);
            slideshowExplorer.RunContext = (context) =>
            {
                standaloneController.TimelineController.setResourceProvider(slideshowEditorController.ResourceProvider);
                standaloneController.MvcCore.startRunningContext(context);
            };
            guiManager.addManagedDialog(slideshowExplorer);

            TaskController taskController = standaloneController.TaskController;
            taskController.addTask(new MDIDialogOpenTask(slideshowExplorer, "Medical.SlideshowExplorer", "Slideshow Editor", CommonResources.NoIcon, TaskMenuCategories.Editor));

            CommonEditorResources.initialize(standaloneController);
        }

        public void sceneLoaded(Engine.ObjectManagement.SimScene scene)
        {
            
        }

        public void sceneUnloading(Engine.ObjectManagement.SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void sceneRevealed()
        {
            
        }

        public long PluginId
        {
            get
            {
                return -1;
            }
        }

        public string PluginName
        {
            get
            {
                return "Lecture Tools";
            }
        }

        public string BrandingImageKey
        {
            get
            {
                return "Lecture.BrandingImage";
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
