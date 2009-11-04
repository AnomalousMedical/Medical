using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Docking;

namespace Medical
{
    [Serializable]
    [Flags]
    public enum DockAreas
    {
        Float = 1,
        DockLeft = 2,
        DockRight = 4,
        DockTop = 8,
        DockBottom = 16,
        Document = 32,
    }

    public enum DockState
    {
        Unknown = 0,
        Float = 1,
        DockTopAutoHide = 2,
        DockLeftAutoHide = 3,
        DockBottomAutoHide = 4,
        DockRightAutoHide = 5,
        Document = 6,
        DockTop = 7,
        DockLeft = 8,
        DockBottom = 9,
        DockRight = 10,
        Hidden = 11,
    }

    public enum DockAlignment
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public class DockContent : UserControl
    {
        private KryptonPage page;
        private DockArea dockArea;

        public DockContent()
        {
            page = new KryptonPage();
            this.Dock = DockStyle.Fill;
            page.Controls.Add(this);
        }

        public void Close()
        {
            if (dockArea != null)
            {
                dockArea.close(this);
                dockArea = null;
            }
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            
        }

        protected virtual String GetPersistString()
        {
            return "";
        }

        public void Show(DockArea panel)
        {
            this.dockArea = panel;
            panel.show(this);
        }

        //public void Show(DockArea panel, DockingEdge edge)
        //{

        //}

        public Icon Icon { get; set; }

        public DockAreas DockAreas { get; set; }

        public MenuStrip MainMenuStrip { get; set; }

        public DockState ShowHint { get; set; }

        public bool HideOnClose { get; set; }

        public bool CloseButton { get; set; }

        public bool CloseButtonVisible { get; set; }

        public override String Text
        {
            get
            {
                return page.Text;
            }
            set
            {
                page.Text = value;
                page.TextTitle = value;
            }
        }

        internal KryptonPage Page
        {
            get
            {
                return page;
            }
        }
    }
}
