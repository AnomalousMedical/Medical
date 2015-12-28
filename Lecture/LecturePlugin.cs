using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;
using Engine;
using Lecture.GUI;
using Medical;
using Medical.Editor;
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
        private EditorController editorController;
        private SlideshowEditController slideshowEditController;
        private TimelineController editorTimelineController;
        private LectureUICallback editorUICallback;
        private PropEditController propEditController;
        private SimObjectMover propMover;

        public LecturePlugin()
        {

        }

        public void Dispose()
        {
            slideshowExplorer.Dispose();
            editorController.Dispose();
            propMover.Dispose();
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Lecture.Resources.LecturePlugin_MyGUI_Skin.xml");
            ResourceManager.Instance.load("Lecture.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;

            editorTimelineController = new TimelineController(standaloneController);
            standaloneController.giveGUIsToTimelineController(editorTimelineController);

            editorController = new EditorController(standaloneController, editorTimelineController);
            editorController.ProjectTypes.addInfo(new ZipProjectType(".sl"));
            standaloneController.DocumentController.addDocumentHandler(new SlideshowDocumentHandler(editorController));

            //Prop Mover
            MedicalController medicalController = standaloneController.MedicalController;
            propMover = new SimObjectMover("LectureProps", medicalController.PluginManager.RendererPlugin, medicalController.EventManager, standaloneController.SceneViewController);

            propEditController = new PropEditController(propMover);

            editorUICallback = new LectureUICallback(standaloneController, editorController, propEditController);

            slideshowEditController = new SlideshowEditController(standaloneController, editorUICallback, this.propEditController, editorController, editorTimelineController);
            slideshowExplorer = new SlideshowExplorer(slideshowEditController);
            guiManager.addManagedDialog(slideshowExplorer);

            TaskController taskController = standaloneController.TaskController;
            taskController.addTask(new MDIDialogOpenTask(slideshowExplorer, "Medical.SlideshowExplorer", "Authoring Tools", "Lecture.Icon.SmartLectureIcon", TaskMenuCategories.Create));

            CommonEditorResources.initialize(standaloneController);
            standaloneController.ViewHostFactory.addFactory(new SlideTaskbarFactory());

            standaloneController.MedicalStateController.StateAdded += MedicalStateController_StateAdded;
            standaloneController.MedicalStateController.BlendingStopped += MedicalStateController_BlendingStopped;
        }

        public void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown)
        {
            standaloneController.MedicalStateController.StateAdded -= MedicalStateController_StateAdded;
            standaloneController.MedicalStateController.BlendingStopped -= MedicalStateController_BlendingStopped;
        }

        public void sceneLoaded(Engine.ObjectManagement.SimScene scene)
        {
            propMover.sceneLoaded(scene);
            slideshowEditController.sceneLoading(scene);
        }

        public void sceneUnloading(Engine.ObjectManagement.SimScene scene)
        {
            propMover.sceneUnloading(scene);
            slideshowEditController.sceneUnloading(scene);
        }

        public long PluginId
        {
            get
            {
                return 28;
            }
        }

        public string PluginName
        {
            get
            {
                return "Authoring Tools";
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

        public bool AllowUninstall
        {
            get
            {
                return true;
            }
        }

        public bool AllowRuntimeUninstall
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
            }
        }

        private void MedicalStateController_BlendingStopped(MedicalStateController controller)
        {
            slideshowEditController.AllowSlideSceneSetup = true;
        }

        private void MedicalStateController_StateAdded(MedicalStateController controller, MedicalState state)
        {
            slideshowEditController.AllowSlideSceneSetup = false;
        }
    }
}
