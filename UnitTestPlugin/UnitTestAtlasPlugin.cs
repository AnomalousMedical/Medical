using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using UnitTestPlugin.GUI;
using Medical.GUI;

namespace UnitTestPlugin
{
    class UnitTestAtlasPlugin : AtlasPlugin
    {
        TestImageAtlas testImageAtlas;

        public UnitTestAtlasPlugin()
        {
            
        }

        public void Dispose()
        {
            testImageAtlas.Dispose();
        }

        public void loadGUIResources()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;

            testImageAtlas = new TestImageAtlas();
            guiManager.addManagedDialog(testImageAtlas);
            testImageAtlas.Visible = true;
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
                return -1;
            }
        }

        public string PluginName
        {
            get
            {
                return "UnitTestPlugin";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "Developer/BrandingImage";
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
