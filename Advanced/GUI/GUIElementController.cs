using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Platform;
using Engine.ObjectManagement;
using System.Drawing;
using System.IO;
using Medical.Controller;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;

namespace Medical.GUI
{
    class RibbonEntry
    {
        private KryptonRibbonGroup group;
        private KryptonRibbonGroupTriple currentTriple;

        public RibbonEntry(String name)
        {
            group = new KryptonRibbonGroup();
            group.TextLine1 = name;
        }

        public KryptonRibbonGroup Group
        {
            get
            {
                return group;
            }
        }

        public KryptonRibbonGroupTriple CurrentTriple
        {
            get
            {
                if (currentTriple == null || currentTriple.Items.Count > 2)
                {
                    currentTriple = new KryptonRibbonGroupTriple();
                    group.Items.Add(currentTriple);
                }
                return currentTriple;
            }
        }
    }

    public class GUIElementController
    {
        private delegate void CallFixedUpdate(Clock time);

        private event CallFixedUpdate callFixedUpdate;

        private List<GUIElement> guiElements = new List<GUIElement>();
        private List<GUIElement> updatingGUIElements = new List<GUIElement>();
        private KryptonDockingManager dock;
        private KryptonRibbonTab tab;
        private SortedDictionary<String, RibbonEntry> tabs = new SortedDictionary<string, RibbonEntry>();
        private bool enabled = false;
        private MedicalController medicalController;

        public GUIElementController(KryptonDockingManager dock, KryptonRibbonTab tab, MedicalController controller)
        {
            this.dock = dock;
            this.tab = tab;
            controller.FixedLoopUpdate += fixedLoopUpdate;
            this.medicalController = controller;
        }

        public void setupShortcuts(ShortcutGroup shortcuts)
        {
            foreach (GUIElement element in guiElements)
            {
                if (element.ShortcutKey != Keys.None)
                {
                    ShortcutEventCommand elementCommand = new ShortcutEventCommand(element.Text, element.ShortcutKey, false);
                    elementCommand.Execute += element.shortcutKeyPressed;
                    shortcuts.addShortcut(elementCommand);
                }
            }
        }

        /// <summary>
        /// Save the windows to a file.
        /// </summary>
        /// <param name="filename"></param>
        public void saveWindowFile(String filename)
        {
            //dock.SaveAsXml(filename);
        }

        /// <summary>
        /// Restore the windows from a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool restoreWindowFile(String filename)
        {
            return false;
            //bool restore = File.Exists(filename);
            //if (restore)
            //{
            //    //Close all windows
            //    for (int index = dock.Contents.Count - 1; index >= 0; index--)
            //    {
            //        IDockContent content = dock.Contents[index] as IDockContent;
            //        if (content != null)
            //        {
            //            content.DockHandler.Close();
            //        }
            //    }
            //    //Load the file
            //    dock.LoadFromXml(filename, callback);
            //}
            //return restore;
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
                    showDockContent(element);
                }
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
            RibbonEntry toolStrip;
            if (!tabs.ContainsKey(element.ToolStripName))
            {
                toolStrip = new RibbonEntry(element.ToolStripName);
                tabs.Add(element.ToolStripName, toolStrip);
                if (enabled)
                {
                    tab.Groups.Add(toolStrip.Group);
                }
            }
            else
            {
                toolStrip = tabs[element.ToolStripName];
            }
            KryptonRibbonGroupButton button = new KryptonRibbonGroupButton();
            button.TextLine1 = element.ButtonText;
            if (element.Icon != null)
            {
                button.ImageLarge = element.Icon.ToBitmap();
            }
            toolStrip.CurrentTriple.Items.Add(button);
            element._setController(this, button);
        }

        //public void removeGUIElement(GUIElement element)
        //{
        //    guiElements.Remove(element);
        //    RibbonEntry toolStrip = tabs[element.ToolStripName];
        //    toolStrip.Items.Remove(element.Button);
        //    if (toolStrip.Items.Count == 0)
        //    {
        //        ribbon.TopToolStripPanel.Controls.Remove(toolStrip);
        //    }
        //    element._setController(null);
        //}

        public void showDockContent(GUIElement content)
        {
            if (content.ShowHint == DockLocations.Right)
            {
                dock.AddDockspace("Control", DockingEdge.Right, new KryptonPage[] { content.Page });
            }
            else if (content.ShowHint == DockLocations.Bottom)
            {
                dock.AddDockspace("Control", DockingEdge.Bottom, new KryptonPage[] { content.Page });
            }
            else
            {
                dock.AddDockspace("Control", DockingEdge.Left, new KryptonPage[] { content.Page });
            }
        }

        public void hideDockContent(GUIElement content)
        {
            dock.RemovePage(content.Page, false);
        }

        public bool isVisible(GUIElement element)
        {
            return dock.ContainsPage(element.Page);
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
                        foreach (RibbonEntry toolStrip in tabs.Values)
                        {
                            tab.Groups.Add(toolStrip.Group);
                        }
                    }
                    else
                    {
                        foreach (RibbonEntry toolStrip in tabs.Values)
                        {
                            tab.Groups.Remove(toolStrip.Group);
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
