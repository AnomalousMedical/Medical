using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Platform;
using Medical.Controller;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    public partial class MedicalForm : Form, OSWindow
    {
        private List<OSWindowListener> listeners = new List<OSWindowListener>();
        private MedicalController controller;
        private LayersControl layersControl = new LayersControl();
        private PictureControl pictureControl = new PictureControl();

        public MedicalForm()
        {
            InitializeComponent();
        }

        public void initialize(MedicalController controller)
        {
            this.controller = controller;
            layersControl.VisibleChanged += new EventHandler(layersControl_VisibleChanged);
            pictureControl.VisibleChanged += new EventHandler(pictureControl_VisibleChanged);

            //temp
            layersControl.addLayerSection("Bones");
            layersControl.addLayerSection("Teeth");
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

        public void addDockContent(DockContent content)
        {
            content.Show(dockPanel);
        }

        public void removeDockContent(DockContent content)
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
    }
}
