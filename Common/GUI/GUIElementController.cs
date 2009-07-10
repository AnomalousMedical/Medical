using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using Engine.Platform;

namespace Medical.GUI
{
    public class GUIElementController
    {
        private delegate void CallFixedUpdate(Clock time);

        private event CallFixedUpdate callFixedUpdate;

        private List<GUIElement> guiElements = new List<GUIElement>();
        private List<GUIElement> updatingGUIElements = new List<GUIElement>();
        private DockPanel dock;
        private ToolStripContainer toolStripContainer;
        private Dictionary<String, ToolStrip> toolStrips = new Dictionary<string, ToolStrip>();

        public GUIElementController(DockPanel dock, ToolStripContainer toolStrip, MedicalController controller)
        {
            this.dock = dock;
            this.toolStripContainer = toolStrip;
            controller.FixedLoopUpdate += fixedLoopUpdate;
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
                element.callSceneUnloading();
            }
        }

        public void alertGUISceneLoaded()
        {
            foreach (GUIElement element in guiElements)
            {
                element.callSceneLoaded();
            }
        }

        internal void addUpdatingElement(GUIElement element)
        {
            callFixedUpdate += element.callFixedLoopUpdate;
        }

        internal void removeUpdatingElement(GUIElement element)
        {
            callFixedUpdate -= element.callFixedLoopUpdate;
        }

        void fixedLoopUpdate(Clock time)
        {
            if (callFixedUpdate != null)
            {
                callFixedUpdate.Invoke(time);
            }
        }
    }
}
