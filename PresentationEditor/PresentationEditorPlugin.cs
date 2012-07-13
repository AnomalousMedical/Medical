using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using Medical.Editor;
using Medical;
using PresentationEditor.GUI;

namespace PresentationEditor
{
    public class PresentationEditorPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;

        private EditorController editorController;
        private TimelineController editorTimelineController;
        private MedicalUICallback medicalUICallback;
        private PresentationEditor presentationController;

        //Editor Contexts
        RmlSlideEditorContext rmlSlideEditorContext;

        //Guis
        private SlideIndex slideIndex;

        public PresentationEditorPlugin()
        {
            Log.Info("Presentation Editor GUI Loaded");
        }

        public void Dispose()
        {
            slideIndex.Dispose();
            editorController.Dispose();
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("PresentationEditor.Resources.PresentationEditorImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;

            editorTimelineController = new TimelineController(standaloneController);
            editorController = new EditorController(standaloneController, editorTimelineController);
            standaloneController.DocumentController.addDocumentHandler(new PresentationDocumentHandler(editorController));
            editorController.ProjectChanged += editorController_ProjectChanged;
            presentationController = new PresentationEditor(editorController);

            medicalUICallback = new MedicalUICallback();

            GUIManager guiManager = standaloneController.GUIManager;

            slideIndex = new SlideIndex(presentationController);
            guiManager.addManagedDialog(slideIndex);

            //Tasks
            TaskController taskController = standaloneController.TaskController;
            taskController.addTask(new MDIDialogOpenTask(slideIndex, "PresentationEditor.SlideIndex", "Presentation Editor", "StandaloneIcons/NoIcon", TaskMenuCategories.Editor));

            editorController.addTypeController(new PresentationTypeController(editorController, standaloneController));

            standaloneController.initializeEditorCore();

            //Rml Type Controller
            RmlTypeController rmlTypeController = new RmlTypeController(editorController);
            rmlTypeController.OpenEditor += file =>
                {
                    rmlSlideEditorContext = new RmlSlideEditorContext(file, presentationController, rmlTypeController, medicalUICallback);
                    rmlSlideEditorContext.Focus += obj =>
                        {
                            rmlSlideEditorContext = obj;
                        };
                    rmlSlideEditorContext.Blur += obj =>
                        {
                            rmlTypeController.updateCachedText(obj.CurrentFile, obj.CurrentText);
                            if (obj == rmlSlideEditorContext)
                            {
                                rmlSlideEditorContext = null;
                            }
                        };
                    editorController.runEditorContext(rmlSlideEditorContext.MvcContext);
                };
            editorController.addTypeController(rmlTypeController);

            //TEMP
            standaloneController.ViewHostFactory.addFactory(new EditorTaskbarFactory(editorController));
        }

        void editorController_ProjectChanged(EditorController editorController, String defaultFile)
        {
            if (rmlSlideEditorContext != null)
            {
                rmlSlideEditorContext.close();
            }

            if (editorController.ResourceProvider != null)
            {
                if (!slideIndex.Visible)
                {
                    slideIndex.Visible = true;
                }
            }
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

        public long PluginId
        {
            get
            {
                return 26;
            }
        }

        public String PluginName
        {
            get
            {
                return "Presentation Editor";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "PresentationEditor/BrandingImage";
            }
        }

        public void createMenuBar(NativeMenuBar menu)
        {

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

        public void sceneRevealed()
        {

        }
    }
}
