using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using Medical.GUI;
using MyGUIPlugin;

namespace DentalSim
{
    class DentalSimPlugin : AtlasPlugin
    {
        public DentalSimPlugin()
        {

        }

        public void Dispose()
        {

        }

        public void createMenuBar(NativeMenuBar menu)
        {

        }

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;
            Gui.Instance.load("Medical.Resources.EditorImagesets.xml");
        }

        public void sceneLoaded(SimScene scene)
        {

        }

        public void sceneRevealed()
        {

        }

        public void sceneUnloading(Engine.ObjectManagement.SimScene scene)
        {

        }

        public void setMainInterfaceEnabled(bool enabled)
        {

        }

        public string BrandingImageKey
        {
            get
            {
                return "DentalSim/BrandingImage";
            }
        }

        public long PluginId
        {
            get
            {
                return 2;
            }
        }

        public string PluginName
        {
            get
            {
                return "Dental Simulation";
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
