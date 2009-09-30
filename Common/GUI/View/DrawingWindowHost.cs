using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine;

namespace Medical
{
    public partial class DrawingWindowHost : DockContent
    {
        private const String PERSIST_STRING = "{0}: {1}: {2}: {3}";
        private static readonly char[] SEP = { ':' };

        /// <summary>
        /// This function will attempt to restore a DrawingWindowHost from a
        /// given persist string. If it is valid the funciton will return true
        /// and pass the window properties back in the out variables.
        /// </summary>
        /// <param name="persistString">The PersistString to restore from.</param>
        /// <param name="name"></param>
        /// <param name="translation"></param>
        /// <param name="lookAt"></param>
        /// <returns>True if the window was sucessfully restored.</returns>
        public static bool RestoreFromString(String persistString, out String name, out Vector3 translation, out Vector3 lookAt)
        {
            String[] parsed = persistString.Split(SEP);
            if (parsed.Length == 4 && parsed[0] == typeof(DrawingWindowHost).ToString())
            {
                name = parsed[1].Trim();
                translation = new Vector3(parsed[2]);
                lookAt = new Vector3(parsed[3]);
                return true;
            }
            else
            {
                name = null;
                translation = Vector3.Zero;
                lookAt = Vector3.Zero;
                return false;
            }
        }

        private List<Control> savedControls = new List<Control>();
        private bool notClosing = true;
        private DrawingWindowController controller;

        public DrawingWindowHost(String name, DrawingWindowController controller)
        {
            InitializeComponent();
            this.Text = name;
            this.Name = name;
            this.controller = controller;
        }

        public DrawingWindow DrawingWindow
        {
            get
            {
                return drawingWindow;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            controller._alertCameraDestroyed(this);
            notClosing = false;
            drawingWindow.destroyCamera();
            base.OnClosing(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (notClosing)
            {
                savedControls.Clear();
                foreach (Control control in Controls)
                {
                    savedControls.Add(control);
                }
                this.Controls.Clear();
            }
            base.OnHandleDestroyed(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            foreach (Control control in savedControls)
            {
                Controls.Add(control);
            }
            savedControls.Clear();
            base.OnHandleCreated(e);
        }

        private void changeBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                drawingWindow.BackColor = colorDialog.Color;
            }
        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawingWindow.setRenderingMode(Engine.RenderingMode.Solid);
        }

        private void wireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawingWindow.setRenderingMode(Engine.RenderingMode.Wireframe);
        }

        private void pointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawingWindow.setRenderingMode(Engine.RenderingMode.Points);
        }

        private void showStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawingWindow.showStats(true);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.F6)
            {
                drawingWindow.setRenderingMode(Engine.RenderingMode.Solid);
            }
            if (e.KeyCode == Keys.F7)
            {
                drawingWindow.setRenderingMode(Engine.RenderingMode.Wireframe);
            }
            if (e.KeyCode == Keys.F8)
            {
                drawingWindow.setRenderingMode(Engine.RenderingMode.Points);
            }
        }

        protected override string GetPersistString()
        {
            return String.Format(PERSIST_STRING, typeof(DrawingWindowHost).ToString(), drawingWindow.CameraName, drawingWindow.Translation, drawingWindow.LookAt);
        }

        public virtual bool IsClone
        {
            get
            {
                return false;
            }
        }
    }
}
