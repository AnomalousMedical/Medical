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

namespace Medical.GUI
{
    public partial class MedicalForm : Form, OSWindow
    {
        private List<OSWindowListener> listeners = new List<OSWindowListener>();
        private MedicalController controller;
        private LayersControl layersControl = new LayersControl();
        private PictureControl pictureControl = new PictureControl();
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
            layersControl.VisibleChanged += new EventHandler(layersControl_VisibleChanged);
            pictureControl.VisibleChanged += new EventHandler(pictureControl_VisibleChanged);
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
            //temp
            layersControl.setupLayers();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String filename = fileTracker.openFile(this);
            if (fileTracker.lastDialogAccepted())
            {
                controller.openScene(filename);
                updateWindowTitle(filename);
            }
            //temp
            layersControl.setupLayers();
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

        private void layersButton_Click(object sender, EventArgs e)
        {
            if (layersControl.Visible)
            {
                layersControl.DockHandler.Hide();
            }
            else
            {
                layersControl.Show(dockPanel);
            }
        }

        void layersControl_VisibleChanged(object sender, EventArgs e)
        {
            layersButton.Checked = layersControl.Visible;
        }

        private void pictureButton_Click(object sender, EventArgs e)
        {
            if (pictureControl.Visible)
            {
                pictureControl.DockHandler.Hide();
            }
            else
            {
                pictureControl.Show(dockPanel); 
            }
        }

        void pictureControl_VisibleChanged(object sender, EventArgs e)
        {
            pictureButton.Checked = pictureControl.Visible;
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
