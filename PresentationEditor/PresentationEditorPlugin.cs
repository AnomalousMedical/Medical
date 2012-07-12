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

namespace PresentationEditor
{
    public class PresentationEditorPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;

        public PresentationEditorPlugin()
        {
            Log.Info("Presentation Editor GUI Loaded");
        }

        public void Dispose()
        {
            
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("PresentationEditor.Resources.PresentationEditorImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
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
