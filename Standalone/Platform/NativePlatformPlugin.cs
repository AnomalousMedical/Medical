using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Logging;

namespace Medical
{
    public class NativePlatformPlugin : PlatformPlugin
    {
        public static NativePlatformPlugin Instance { get; private set; }

        public NativePlatformPlugin()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("Can only create NativePlatformPlugin one time.");
            }
        }

        public void Dispose()
        {

        }

        public SystemTimer createTimer()
        {
            return new NativeSystemTimer();
        }

        public void destroyTimer(SystemTimer timer)
        {
            NativeSystemTimer pcTimer = timer as NativeSystemTimer;
            if (pcTimer != null)
            {
                pcTimer.Dispose();
            }
            else
            {
                Log.Error("Attempted to delete a SystemTimer that was not a PCSystemTimer in NativePlatformPlugin. Are you mixing platform plugins?");
            }
        }

        public InputHandler createInputHandler(OSWindow window, bool foreground, bool exclusive, bool noWinKey)
        {
            return new NativeInputHandler((NativeOSWindow)window);
        }

        public void destroyInputHandler(InputHandler handler)
        {
            NativeInputHandler pcInput = handler as NativeInputHandler;
            if (pcInput != null)
            {
                pcInput.Dispose();
            }
            else
            {
                Log.Error("Attempted to delete a InputHandler that was not a PCInputHandler in NativePlatformPlugin. Are you mixing platform plugins?");
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
            return "NativePlatform";
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
