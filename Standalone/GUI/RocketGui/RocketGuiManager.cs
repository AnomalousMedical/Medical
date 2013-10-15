﻿using System;
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
        private static RocketRawOgreFilesystemArchiveFactory rawFilesystemArchives = new RocketRawOgreFilesystemArchiveFactory();
        private static RocketRenderSystemListener rocketRenderSystemListener;
        private static OgreResourceProviderArchiveFactory resourceProviderArchiveFactory = new OgreResourceProviderArchiveFactory();

        public RocketGuiManager()
        {
            
        }

        public void Dispose()
        {
            RocketOgreTextureManager.shutdown();
            if (eventListenerInstancer != null)
            {
                eventListenerInstancer.Dispose();
            }
        }

        /// <summary>
        /// This is one final dispose step, but this has to be done AFTER ogre is shutdown.
        /// </summary>
        public void destroyOgreCustomArchive()
        {
            if (resourceProviderArchiveFactory != null)
            {
                resourceProviderArchiveFactory.Dispose();
                resourceProviderArchiveFactory = null;
            }
            if (rawFilesystemArchives != null)
            {
                rawFilesystemArchives.Dispose();
                rawFilesystemArchives = null;
            }
            if (rocketRenderSystemListener != null)
            {
                rocketRenderSystemListener.Dispose();
            }
        }

        public void initialize(PluginManager pluginManager, EventManager eventManager, UpdateTimer mainTimer)
        {
            //Create a rocket group in ogre
            Root.getSingleton().addArchiveFactory(rawFilesystemArchives);
            Root.getSingleton().addArchiveFactory(resourceProviderArchiveFactory);

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

            RocketOgreTextureManager.startup();
            //Debugger.Initialise(context);
        }

        public static void clearAllCaches()
        {
            Factory.ClearStyleSheetCache();
            TemplateCache.ClearTemplateCache();
        }
    }
}
