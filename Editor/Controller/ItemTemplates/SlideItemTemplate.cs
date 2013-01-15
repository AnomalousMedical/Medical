using Engine.Editing;
using Logging;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    class SlideItemTemplate : ProjectItemTemplate
    {
        private RmlTypeController rmlTypeController;
        private MvcTypeController mvcTypeController;
        private SceneViewController sceneViewController;
        private MedicalStateController medicalStateController;

        public SlideItemTemplate(RmlTypeController rmlTypeController, MvcTypeController mvcTypeController, SceneViewController sceneViewController, MedicalStateController medicalStateController)
        {
            this.rmlTypeController = rmlTypeController;
            this.mvcTypeController = mvcTypeController;
            this.sceneViewController = sceneViewController;
            this.medicalStateController = medicalStateController;
        }

        public void createItem(string path, EditorController editorController)
        {
            String slideFolder = Path.Combine(path, PathExtensions.RemoveExtension(Name));
            if (!editorController.ResourceProvider.exists(slideFolder))
            {
                editorController.ResourceProvider.createDirectory(path, Name);
            }
            String leftViewFileName = Path.Combine(slideFolder, "Left.rml");
            rmlTypeController.createRmlFileSafely(leftViewFileName);
            AnomalousMvcContext mvcContext = mvcTypeController.CurrentObject;
            if (mvcContext != null)
            {
                String leftViewName = PathExtensions.RemoveExtension(leftViewFileName);
                if (mvcContext.Views.hasItem(leftViewName))
                {
                    MessageBox.show(String.Format("A view named '{0}' already exists. Would you like to overwrite it?", leftViewName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, (result) =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            mvcContext.Views.remove(mvcContext.Views[leftViewName]);
                            createView(mvcContext, leftViewName);
                        }
                    });
                }
                else
                {
                    createView(mvcContext, leftViewName);
                }

                String controllerName = slideFolder;
                if (mvcContext.Controllers.hasItem(controllerName))
                {
                    MessageBox.show(String.Format("A controller named '{0}' already exists. Would you like to overwrite it?", controllerName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, (result) =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            mvcContext.Controllers.remove(mvcContext.Controllers[controllerName]);
                            createController(mvcContext, controllerName, leftViewName);
                        }
                    });
                }
                else
                {
                    createController(mvcContext, controllerName, leftViewName);
                }

                MvcModel model;
                if (mvcContext.Models.tryGetValue(EmbeddedTemplateNames.SlideshowMvcContext.NavigationModel, out model))
                {
                    NavigationModel navModel = model as NavigationModel;
                    if (navModel != null)
                    {
                        navModel.addNavigationLink(new NavigationLink(name: controllerName, action: controllerName + "/Show"));
                    }
                    else
                    {
                        Log.Error("Found a model named '{0}', but it was not a NavigationModel.", EmbeddedTemplateNames.SlideshowMvcContext.NavigationModel);
                    }
                }
                else
                {
                    Log.Error("Cannot find navigation model '{0}' while creating slideshow, cannot navigate to state.", EmbeddedTemplateNames.SlideshowMvcContext.NavigationModel);
                }

                editorController.saveAllCachedResources();
            }
        }

        public string TypeName
        {
            get
            {
                return "Slide";
            }
        }

        public string ImageName
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        public string Group
        {
            get
            {
                return "Slideshow";
            }
        }

        [Editable]
        public String Name { get; set; }

        public bool isValid(out string errorMessage)
        {
            if (String.IsNullOrEmpty(Name))
            {
                errorMessage = "Please enter a name.";
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }

        public void reset()
        {
            Name = null;
        }

        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
                }
                return editInterface;
            }
        }

        private void createController(AnomalousMvcContext mvcContext, String controllerName, String viewName)
        {
            MvcController controller = new MvcController(controllerName);
            
            RunCommandsAction show = new RunCommandsAction("Show");
            show.addCommand(new ShowViewCommand(viewName));
            CameraPosition cameraPos = new CameraPosition();
            if (sceneViewController.ActiveWindow != null)
            {
                SceneViewWindow window = sceneViewController.ActiveWindow;
                cameraPos.Translation = window.Translation;
                cameraPos.LookAt = window.LookAt;
                cameraPos.calculateIncludePoint(window);
            }
            show.addCommand(new MoveCameraCommand()
            {
                CameraPosition = cameraPos
            });
            ChangeLayersCommand layers = new ChangeLayersCommand();
            layers.Layers.captureState();
            show.addCommand(layers);
            show.addCommand(new ChangeMedicalStateCommand()
            {
                PresetState = medicalStateController.createPresetState("")
            });
            controller.Actions.add(show);
            show.addCommand(new SetMusclePositionCommand());
            
            RunCommandsAction close = new RunCommandsAction("Close");
            close.addCommand(new CloseViewCommand());
            controller.Actions.add(close);
            mvcContext.Controllers.add(controller);
        }

        private static void createView(AnomalousMvcContext mvcContext, String name)
        {
            RmlView view = new RmlView(name);
            mvcContext.Views.add(view);
        }
    }
}
