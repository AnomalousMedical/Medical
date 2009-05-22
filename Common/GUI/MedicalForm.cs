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
    public partial class MedicalForm : Form, OSWindow
    {
        private List<OSWindowListener> listeners = new List<OSWindowListener>();
        private MedicalController controller;
        private FileTracker fileTracker = new FileTracker("*.sim.xml|*.sim.xml");
        private String windowDefaultText;
        private const String TITLE_FORMAT = "{0} - {1}";

        public MedicalForm()
        {
            InitializeComponent();
            windowDefaultText = this.Text;
        }

        public void initialize(MedicalController controller)
        {
            this.controller = controller;
        }

        public void addToolStrip(ToolStrip toolStrip)
        {
            toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip);
        }

        public void removeToolStrip(ToolStrip toolStrip)
        {
            if (toolStrip.Parent != null)
            {
                toolStrip.Parent.Controls.Remove(toolStrip);
            }
        }

        public void showDockContent(DockContent content)
        {
            content.Show(dockPanel);
        }

        public void hideDockContent(DockContent content)
        {
            content.DockHandler.Hide();
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
                for (int index = dockPanel.Contents.Count - 1; index >= 0; index--)
                {
                    if (dockPanel.Contents[index] is IDockContent)
                    {
                        IDockContent content = (IDockContent)dockPanel.Contents[index];
                        content.DockHandler.Close();
                    }
                }
                dockPanel.LoadFromXml(filename, callback);
            }
            return restore;
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (controller != null)
            {
                controller.shutdown();
            }
            base.OnFormClosing(e);
        }

        private void oneWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void twoWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void threeWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void fourWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.createNewScene();
            clearWindowTitle();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String filename = fileTracker.openFile(this);
            if (fileTracker.lastDialogAccepted())
            {
                controller.openScene(filename);
                updateWindowTitle(filename);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String filename = fileTracker.saveFile(this);
            if (fileTracker.lastDialogAccepted())
            {
                controller.saveScene(filename);
                updateWindowTitle(filename);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String filename = fileTracker.saveFileAs(this);
            if (fileTracker.lastDialogAccepted())
            {
                controller.saveScene(filename);
                updateWindowTitle(filename);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.shutdown();
            this.Close();
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
    }
}
