using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Engine.ObjectManagement;

namespace Medical
{
    public class ToolManager : UpdateListener
    {
        private Tool currentTool;
        private List<Tool> tools = new List<Tool>();
        private EventManager eventManager;
        private bool enabled = true;

        public ToolManager(EventManager eventManager)
        {
            this.eventManager = eventManager;
        }

        public void addTool(Tool tool)
        {
            tools.Add(tool);
        }

        public void removeTool(Tool tool)
        {
            tools.Remove(tool);
        }

        public void createSceneElements(SimSubScene subScene, PluginManager pluginManager)
        {
            foreach (Tool tool in tools)
            {
                tool.createSceneElement(subScene, pluginManager);
            }
        }

        public void destroySceneElements(SimSubScene subScene, PluginManager pluginManager)
        {
            foreach (Tool tool in tools)
            {
                tool.destroySceneElement(subScene, pluginManager);
            }
        }

        public void enableTool(Tool tool)
        {
            foreach (Tool currentTool in tools)
            {
                if (currentTool != tool)
                {
                    currentTool.setEnabled(false);
                }
            }
            tool.setEnabled(enabled);
            this.currentTool = tool;
        }

        public void setEnabled(bool enabled)
        {
            if (currentTool != null)
            {
                currentTool.setEnabled(enabled);
            }
            this.enabled = enabled;
        }

        public void setToolTranslation(Vector3 translation)
        {
            foreach (Tool tool in tools)
            {
                tool.setTranslation(translation);
            }
        }

        #region UpdateListener Members

        public void sendUpdate(Clock clock)
        {
            if (enabled && currentTool != null)
            {
                currentTool.update(eventManager);
            }
        }

        public void loopStarting()
        {
            
        }

        public void exceededMaxDelta()
        {
            
        }

        #endregion
    }
}
