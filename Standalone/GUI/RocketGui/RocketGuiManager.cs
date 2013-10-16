using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using OgreWrapper;
using OgrePlugin;
using Engine;
using Engine.Platform;
using MyGUIPlugin;
using System.Reflection;

namespace Medical.GUI
{
    public class RocketGuiManager : IDisposable
    {
        private EventListenerInstancer eventListenerInstancer;
        private static RocketRenderSystemListener rocketRenderSystemListener;

        public RocketGuiManager()
        {
            
        }

        public void Dispose()
        {
            if (eventListenerInstancer != null)
            {
                eventListenerInstancer.Dispose();
            }
            if (rocketRenderSystemListener != null)
            {
                Root.getSingleton().getRenderSystem().removeListener(rocketRenderSystemListener);
                rocketRenderSystemListener.Dispose();
            }
        }

        public void initialize(PluginManager pluginManager, EventManager eventManager, UpdateTimer mainTimer)
        {
            rocketRenderSystemListener = new RocketRenderSystemListener();
            Root.getSingleton().getRenderSystem().addListener(rocketRenderSystemListener);

            eventListenerInstancer = new RocketEventListenerInstancer();
            Factory.RegisterEventListenerInstancer(eventListenerInstancer);

            RocketInterface.Instance.FileInterface.addExtension(new RocketAssemblyResourceLoader(typeof(RocketInterface).Assembly));
            RocketInterface.Instance.FileInterface.addExtension(new RocketAssemblyResourceLoader(typeof(MyGUIInterface).Assembly));

            FontDatabase.LoadFontFace("MyGUIPlugin_DejaVuSans.ttf", "DejaVuSans", Font.Style.STYLE_NORMAL, Font.Weight.WEIGHT_NORMAL);
            FontDatabase.LoadFontFace("MyGUIPlugin.Resources.MyGUIPlugin_DejaVuSans-Bold.ttf", "DejaVuSans", Font.Style.STYLE_NORMAL, Font.Weight.WEIGHT_BOLD);
            FontDatabase.LoadFontFace("MyGUIPlugin.Resources.MyGUIPlugin_DejaVuSans-BoldOblique.ttf", "DejaVuSans", Font.Style.STYLE_ITALIC, Font.Weight.WEIGHT_BOLD);
            FontDatabase.LoadFontFace("MyGUIPlugin.Resources.MyGUIPlugin_DejaVuSans-Oblique.ttf", "DejaVuSans", Font.Style.STYLE_ITALIC, Font.Weight.WEIGHT_NORMAL);

            //Debugger.Initialise(context);
        }

        public static void clearAllCaches()
        {
            Factory.ClearStyleSheetCache();
            TemplateCache.ClearTemplateCache();
        }
    }
}
