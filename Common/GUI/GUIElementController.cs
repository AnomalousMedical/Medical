using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;

namespace Medical.GUI
{
    public class GUIElementController
    {
        private List<GUIElement> guiElements = new List<GUIElement>();
        private DockPanel dock;
        private ToolStripContainer toolStripContainer;
        private Dictionary<String, ToolStrip> toolStrips = new Dictionary<string, ToolStrip>();

        public GUIElementController(DockPanel dock, ToolStripContainer toolStrip)
        {
            this.dock = dock;
            this.toolStripContainer = toolStrip;
        }

        public void addGUIElement(GUIElement element)
        {
            guiElements.Add(element);
            ToolStrip toolStrip;
            if (!toolStrips.ContainsKey(element.ToolStripName))
            {
                toolStrip = new ToolStrip();
                toolStrips.Add(element.ToolStripName, toolStrip);
                toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip);
            }
            else
            {
                toolStrip = toolStrips[element.ToolStripName];
            }
            toolStrip.Items.Add(element.Button);
            element._setController(this);
        }

        public void removeGUIElement(GUIElement element)
        {
            guiElements.Remove(element);
            ToolStrip toolStrip = toolStrips[element.ToolStripName];
            toolStrip.Items.Remove(element.Button);
            if (toolStrip.Items.Count == 0)
            {
                toolStripContainer.TopToolStripPanel.Controls.Remove(toolStrip);
            }
            element._setController(null);
        }

        public DockContent restoreWindow(String persistString)
        {
            foreach (GUIElement element in guiElements)
            {
                if (element.matchesPersistString(persistString))
                {
                    return element;
                }
            }
            return null;
        }

        public void showDockContent(DockContent content)
        {
            content.Show(dock);
        }

        public void hideDockContent(DockContent content)
        {
            content.Hide();
        }

        public void alertGUISceneUnloading()
        {
            foreach (GUIElement element in guiElements)
            {
                element.sceneUnloading();
            }
        }

        public void alertGUISceneLoaded()
        {
            foreach (GUIElement element in guiElements)
            {
                element.sceneLoaded();
            }
        }
    }
}
