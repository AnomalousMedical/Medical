using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using MyGUIPlugin;
using GraphicsUser.GUI;
using Medical.GUI;

namespace GraphicsUser
{
    class GraphicsUserPlugin : AtlasPlugin
    {
        private RenderPropertiesDialog renderDialog;

        public GraphicsUserPlugin()
        {

        }

        public void Dispose()
        {
            renderDialog.Dispose();
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        public void loadGUIResources()
        {
            Gui.Instance.load("GraphicsUser.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            //Dialogs
            GUIManager guiManager = standaloneController.GUIManager;

            renderDialog = new RenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer);
            guiManager.addManagedDialog(renderDialog);

            //Tasks
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(renderDialog, "GraphicsUser.Render", "Render", "RenderIcon", TaskMenuCategories.Tools));
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneRevealed()
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }
        
        public string BrandingImageKey
        {
            get
            {
                return "GraphicsUser/BrandingImage";
            }
        }

        public string Location
        {
            get
            {
                return GetType().Assembly.Location;
            }
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
                return "Graphics User";
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
