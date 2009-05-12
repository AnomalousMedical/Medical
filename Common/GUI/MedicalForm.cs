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
using Medical.GUI.View;

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
            leftPanel.AutoSize = true;
            rightPanel.AutoSize = true;
            bottomPanel.AutoSize = true;
            layersControl.ParentChanged += new EventHandler(layersControl_ParentChanged);
            pictureControl.ParentChanged += new EventHandler(pictureControl_ParentChanged);
            layersControl.addLayerSection("Bones");
            layersControl.addLayerSection("Teeth");
        }

        public DrawingSplitHost DrawingHost
        {
            get
            {
                return drawingSplitHost;
            }
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

        public void addLeftControl(Control control)
        {
            leftPanel.Controls.Clear();
            leftPanel.Controls.Add(control);
        }

        public void removeControl(Control control)
        {
            if (control.Parent != null)
            {
                control.Parent.Controls.Remove(control);
            }
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
            controller.SplitController.createOneWaySplit();
        }

        private void twoWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.SplitController.createTwoWaySplit();
        }

        private void threeWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.SplitController.createThreeWayUpperSplit();
        }

        private void fourWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.SplitController.createFourWaySplit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.shutdown();
            this.Close();
        }

        private void layersButton_Click(object sender, EventArgs e)
        {
            if (layersControl.Parent == null)
            {
                rightPanel.Controls.Clear();
                rightPanel.Controls.Add(layersControl);
            }
            else
            {
                layersControl.Parent.Controls.Remove(layersControl);
            }
        }

        void layersControl_ParentChanged(object sender, EventArgs e)
        {
            layersButton.Checked = layersControl.Parent != null;
        }

        private void pictureButton_Click(object sender, EventArgs e)
        {
            if (pictureControl.Parent == null)
            {
                rightPanel.Controls.Clear();
                rightPanel.Controls.Add(pictureControl);
            }
            else
            {
                pictureControl.Parent.Controls.Clear();
            }
        }

        void pictureControl_ParentChanged(object sender, EventArgs e)
        {
            pictureButton.Checked = pictureControl.Parent != null;
        }
    }
}
