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
using ComponentFactory.Krypton.Workspace;

namespace Medical.GUI
{
    public partial class KryptonDrawingWindowHost : UserControl, DrawingWindowHost
    {
        private DrawingWindowController controller;
        private DrawingWindow drawingWindow;
        private KryptonPage page;
        private KryptonDockingManager dockingManager;
        private KryptonWorkspaceSequence horizontalSequence;
        private KryptonWorkspaceSequence verticalSequence;

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
            horizontalSequence = new KryptonWorkspaceSequence(Orientation.Horizontal);
            verticalSequence = new KryptonWorkspaceSequence(Orientation.Vertical);
            verticalSequence.Children.Add(horizontalSequence);
            KryptonWorkspaceCell cell = new KryptonWorkspaceCell();
            cell.Pages.Add(page);
            horizontalSequence.Children.Add(cell);
        }

        public void ShowWindow(DrawingWindowHost parent, DrawingWindowPosition position)
        {
            KryptonDrawingWindowHost parentHost = (KryptonDrawingWindowHost)parent;

            switch (position)
            {
                case DrawingWindowPosition.Bottom:
                    parentHost.verticalSequence.Children.Add(verticalSequence);
                    break;
                case DrawingWindowPosition.Top:
                    parentHost.verticalSequence.Children.Insert(0, verticalSequence);
                    break;
                case DrawingWindowPosition.Left:
                    parentHost.horizontalSequence.Children.Insert(0, verticalSequence);
                    break;
                case DrawingWindowPosition.Right:
                    parentHost.horizontalSequence.Children.Add(verticalSequence);
                    break;
            }
        }

        public void ShowWindow()
        {
            KryptonDockingWorkspace ws = dockingManager.FindDockingWorkspace("Workspace");
            ws.DockableWorkspaceControl.Root.Children.Add(verticalSequence);
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
            page.TextTitle = this.Text;
            page.TextDescription = "";
        }
    }
}
