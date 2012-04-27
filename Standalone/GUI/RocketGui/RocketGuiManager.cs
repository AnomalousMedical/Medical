using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using OgreWrapper;
using OgrePlugin;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    class RocketGuiManager : IDisposable
    {
        private EventListenerInstancer eventListenerInstancer;
        private RocketWidget rocketWidget;

        public RocketGuiManager()
        {

        }

        public void Dispose()
        {
            if (rocketWidget != null)
            {
                rocketWidget.Dispose();
            }
            if (eventListenerInstancer != null)
            {
                eventListenerInstancer.Dispose();
            }
        }

        public void initialize(PluginManager pluginManager, EventManager eventManager, UpdateTimer mainTimer)
        {
            //Create a rocket group in ogre
            OgreResourceGroupManager.getInstance().createResourceGroup("Rocket");

            eventListenerInstancer = new TestEventListenerInstancer();
            Factory.RegisterEventListenerInstancer(eventListenerInstancer);

            String sample_path = "S:/Junk/librocket/playing/";//"S:/dependencies/libRocket/src/Samples/";
            VirtualFileSystem.Instance.addArchive(sample_path);
            OgreResourceGroupManager.getInstance().addResourceLocation("assets", "EngineArchive", "Rocket", false);

            FontDatabase.LoadFontFace("assets/Delicious-Roman.otf");
            FontDatabase.LoadFontFace("assets/Delicious-Bold.otf");
            FontDatabase.LoadFontFace("assets/Delicious-Italic.otf");
            FontDatabase.LoadFontFace("assets/Delicious-BoldItalic.otf");

            //Debugger.Initialise(context);
            String name = "Test";
            MyGUIPlugin.ImageBox imageBox = (MyGUIPlugin.ImageBox)MyGUIPlugin.Gui.Instance.createWidgetT("ImageBox", "ImageBox", 100, 100, 800, 600, MyGUIPlugin.Align.Default, "Overlapped", name);
            rocketWidget = new RocketWidget(name, imageBox);
        }
    }
}
