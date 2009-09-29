using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using Engine.Platform;
using Engine.ObjectManagement;
using System.Drawing;
using System.IO;

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
        private SortedDictionary<String, ToolStrip> toolStrips = new SortedDictionary<string, ToolStrip>();
        private bool enabled = false;
        private MedicalController medicalController;

        public GUIElementController(DockPanel dock, ToolStripContainer toolStrip, MedicalController controller)
        {
            this.dock = dock;
            this.toolStripContainer = toolStrip;
            controller.FixedLoopUpdate += fixedLoopUpdate;
            toolStrip.Size = new Size(toolStrip.Size.Width, 40);
            this.medicalController = controller;
        }

        /// <summary>
        /// Save the windows to a file.
        /// </summary>
        /// <param name="filename"></param>
        public void saveWindowFile(String filename)
        {
            dock.SaveAsXml(filename);
        }

        /// <summary>
        /// Restore the windows from a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool restoreWindowFile(String filename, DeserializeDockContent callback)
        {
            bool restore = File.Exists(filename);
            if (restore)
            {
                //Close all windows
                for (int index = dock.Contents.Count - 1; index >= 0; index--)
                {
                    IDockContent content = dock.Contents[index] as IDockContent;
                    if (content != null)
                    {
                        content.DockHandler.Close();
                    }
                }
                //Load the file
                dock.LoadFromXml(filename, callback);
            }
            return restore;
        }

        /// <summary>
        /// Hide the GUIElements controlled by this controller.
        /// </summary>
        public void hideWindows()
        {
            foreach (GUIElement element in guiElements)
            {
                element.RestoreFromHidden = element.Visible;
                element.Hide();
            }
        }

        /// <summary>
        /// Restore all elements hidden by hideWindows().
        /// </summary>
        public void restoreHiddenWindows()
        {
            foreach (GUIElement element in guiElements)
            {
                if (element.RestoreFromHidden)
                {
                    element.Show(dock);
                }
            }
        }

        public IDockContent ActiveDocument
        {
            get
            {
                return dock.ActiveDocument;
            }
        }

        public DockPanel DockPanel
        {
            get
            {
                return dock;
            }
        }

        public ToolStripContainer ToolStrip
        {
            get
            {
                return toolStripContainer;
            }
        }

        public MedicalController MedicalController
        {
            get
            {
                return medicalController;
            }
        }

        public void addGUIElement(GUIElement element)
        {
            guiElements.Add(element);
            ToolStrip toolStrip;
            if (!toolStrips.ContainsKey(element.ToolStripName))
            {
                toolStrip = new ToolStrip();
                toolStrip.ImageScalingSize = new Size(32, 32);
                toolStrips.Add(element.ToolStripName, toolStrip);
                if (enabled)
                {
                    toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip);
                }
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

        public void alertGUISceneLoaded(SimScene scene)
        {
            foreach (GUIElement element in guiElements)
            {
                element.callSceneLoaded(scene);
            }
        }

        public bool EnableToolbars
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (enabled)
                    {
                        foreach (ToolStrip toolStrip in toolStrips.Values)
                        {
                            toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip);
                        }
                    }
                    else
                    {
                        foreach (ToolStrip toolStrip in toolStrips.Values)
                        {
                            toolStripContainer.TopToolStripPanel.Controls.Remove(toolStrip);
                        }
                    }
                }
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
