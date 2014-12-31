using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using System.Diagnostics;
using Medical.GUI;
using Engine;
using Medical.Movement.GUI;
using Anomalous.GuiFramework;

namespace Medical.Movement
{
    public class MovementBodyAtlasPlugin : AtlasPlugin
    {
        private PoseController poseController;

        private MovementDialog movementDialog;

        public MovementBodyAtlasPlugin()
        {
            
        }

        public void Dispose()
        {
            movementDialog.Dispose();
        }

        public void loadGUIResources()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            poseController = new PoseController(standaloneController);

            GUIManager guiManager = standaloneController.GUIManager;
            var resources = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            movementDialog = new MovementDialog(standaloneController.MusclePositionController, poseController, standaloneController.MedicalController);
            guiManager.addManagedDialog(movementDialog);

            var taskController = standaloneController.TaskController;
            var movementDialogTask = new PinableMDIDialogOpenTask(movementDialog, "Medical.Movement.MovementDialogTask", "Movement", CommonResources.NoIcon, TaskMenuCategories.Explore);
            taskController.addTask(movementDialogTask);
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public long PluginId
        {
            get
            {
                return 48;
            }
        }

        public String PluginName
        {
            get
            {
                return "Movement Simulation";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return CommonResources.NoIcon;
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

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
            }
        }
    }
}
