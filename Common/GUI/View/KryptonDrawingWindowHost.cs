using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Docking;

namespace Medical.GUI
{
    public partial class KryptonDrawingWindowHost : UserControl, DrawingWindowHost
    {
        private DrawingWindowController controller;
        private DrawingWindow drawingWindow;
        private KryptonPage page;
        private KryptonDockingManager dockingManager;

        public KryptonDrawingWindowHost(String name, DrawingWindowController controller, KryptonDockingManager dockingManager)
        {
            InitializeComponent();

            this.Text = name;
            this.Name = name;
            this.controller = controller;
            this.dockingManager = dockingManager;

            // Create the drawing window here since it breaks the designer otherwise
            drawingWindow = new Medical.DrawingWindow();
            drawingWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            drawingWindow.Location = new System.Drawing.Point(0, 0);
            drawingWindow.Name = "drawingWindow";
            drawingWindow.Size = new System.Drawing.Size(284, 264);
            drawingWindow.TabIndex = 0;
            Controls.Add(this.drawingWindow);
            drawingWindow.SubTextChanged += new DrawingWindowEvent(drawingWindow_SubTextChanged);

            //Create krypton page
            page = new KryptonPage(name, name);
            this.Dock = DockStyle.Fill;
            page.Controls.Add(this);
        }

        public void ShowWindow(DrawingWindowHost parent, DrawingWindowPosition position)
        {
            dockingManager.AddToWorkspace("Workspace", new KryptonPage[] { page });
        }

        public void ShowWindow()
        {
            dockingManager.AddToWorkspace("Workspace", new KryptonPage[] { page });
        }

        public void Close()
        {
            dockingManager.CloseRequest(new String[] { page.UniqueName });
        }

        public DrawingWindow DrawingWindow
        {
            get
            {
                return drawingWindow;
            }
        }

        internal void alertClosing()
        {
            drawingWindow.destroyCamera();
            controller._alertCameraDestroyed(this);
        }

        void drawingWindow_SubTextChanged(DrawingWindow window)
        {
            String subText = window.SubText;
            if (subText != null)
            {
                this.Text = String.Format("{0} - {1}", Name, window.SubText);
            }
            else
            {
                this.Text = this.Name;
            }
            page.Text = this.Text;
        }
    }
}
