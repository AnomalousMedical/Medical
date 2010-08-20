using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Logging;

namespace Medical
{
    public class WxPlatformPlugin : PlatformPlugin
    {
        public static WxPlatformPlugin Instance { get; private set; }

        public WxPlatformPlugin()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("Can only create PCPlatformPlugin one time.");
            }
        }

        public void Dispose()
        {

        }

        public SystemTimer createTimer()
        {
            return new WxSystemTimer();
        }

        public void destroyTimer(SystemTimer timer)
        {
            WxPlatformPlugin pcTimer = timer as WxPlatformPlugin;
            if (pcTimer != null)
            {
                pcTimer.Dispose();
            }
            else
            {
                Log.Error("Attempted to delete a SystemTimer that was not a PCSystemTimer in PCPlatformPlugin. Are you mixing platform plugins?");
            }
        }

        public InputHandler createInputHandler(OSWindow window, bool foreground, bool exclusive, bool noWinKey)
        {
            return new WxInputHandler((WxOSWindow)window);
        }

        public void destroyInputHandler(InputHandler handler)
        {
            WxInputHandler pcInput = handler as WxInputHandler;
            if (pcInput != null)
            {
                pcInput.Dispose();
            }
            else
            {
                Log.Error("Attempted to delete a InputHandler that was not a PCInputHandler in PCPlatformPlugin. Are you mixing platform plugins?");
            }
        }

        public void initialize(PluginManager pluginManager)
        {
            pluginManager.setPlatformPlugin(this);
        }

        public void setPlatformInfo(UpdateTimer mainTimer, EventManager eventManager)
        {

        }

        public string getName()
        {
            return "PCPlatform";
        }

        public DebugInterface getDebugInterface()
        {
            return null;
        }

        public void createDebugCommands(List<CommandManager> commands)
        {

        }
    }
}
