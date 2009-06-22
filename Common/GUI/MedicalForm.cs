using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Platform;
using Medical;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;

namespace Medical.GUI
{
    public class MedicalForm : Form, OSWindow
    {
        private List<OSWindowListener> listeners = new List<OSWindowListener>();
        protected FileTracker fileTracker = new FileTracker("*.sim.xml|*.sim.xml");
        private String windowDefaultText;
        private const String TITLE_FORMAT = "{0} - {1}";
        private DockPanel dockPanel;
        private ToolStripContainer toolStrip;

        public MedicalForm()
        {

        }

        protected void initialize(DockPanel dockPanel, ToolStripContainer toolStrip)
        {
            this.dockPanel = dockPanel;
            this.toolStrip = toolStrip;
        }

        public void saveWindows(String filename)
        {
            dockPanel.SaveAsXml(filename);
        }

        public bool restoreWindows(String filename, DeserializeDockContent callback)
        {
            bool restore = File.Exists(filename);
            if (restore)
            {
                //Close all windows
                for (int index = dockPanel.Contents.Count - 1; index >= 0; index--)
                {
                    IDockContent content = dockPanel.Contents[index] as IDockContent;
                    if (content != null)
                    {
                        content.DockHandler.Close();
                    }
                }
                //Load the file
                dockPanel.LoadFromXml(filename, callback);
            }
            return restore;
        }

        public IDockContent ActiveDocument
        {
            get
            {
                return dockPanel.ActiveDocument;
            }
        }

        internal void showDockContent(DockContent content)
        {
            content.Show(dockPanel);
        }

        internal void hideDockContent(DockContent content)
        {
            content.DockHandler.Hide();
        }

        #region OSWindow Members

        public IntPtr WindowHandle
        {
            get
            {
                return this.Handle;
            }
        }

        public int WindowWidth
        {
            get
            {
                return this.Width;
            }
        }

        public int WindowHeight
        {
            get
            {
                return this.Height;
            }
        }

        public void addListener(OSWindowListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(OSWindowListener listener)
        {
            listeners.Remove(listener);
        }

        #endregion

        protected override void OnResize(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.resized(this);
            }
            base.OnResize(e);
        }

        protected override void OnMove(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.moved(this);
            }
            base.OnMove(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.closing(this);
            }
            base.OnHandleDestroyed(e);
        }
        
        /// <summary>
        /// Update the title of the window to reflect a current filename or other info.
        /// </summary>
        /// <param name="subName">A name to place as a secondary name in the title.</param>
        private void updateWindowTitle(String subName)
        {
            Text = String.Format(TITLE_FORMAT, windowDefaultText, subName);
        }

        /// <summary>
        /// Clear the window title back to the default text.
        /// </summary>
        private void clearWindowTitle()
        {
            Text = windowDefaultText;
        }

        public DockPanel DockPanel
        {
            get
            {
                return dockPanel;
            }
        }

        public ToolStripContainer ToolStrip
        {
            get
            {
                return toolStrip;
            }
        }
    }
}
