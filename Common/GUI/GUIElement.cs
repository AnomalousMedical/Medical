using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Platform;
using Engine.ObjectManagement;
using Medical.Controller;

namespace Medical.GUI
{
    public class GUIElement : DockContent
    {
        private ToolStripButton button = new ToolStripButton();
        private GUIElementController controller;
        private bool updating = false;
        private Keys shortcutKey = Keys.None;

        public GUIElement()
        {
            InitializeComponent();
            ToolStripName = "Default";
            this.VisibleChanged += new EventHandler(content_VisibleChanged);
            button.DisplayStyle = ToolStripItemDisplayStyle.Image;
            button.Click += new EventHandler(button_Click);
        }

        public Keys ShortcutKey
        {
            get
            {
                return shortcutKey;
            }
            set
            {
                shortcutKey = value;
            }
        }

        public String ButtonText
        {
            get
            {
                return button.Text;
            }
            set
            {
                button.Text = value;
            }
        }

        public int ButtonImageIndex
        {
            get
            {
                return button.ImageIndex;
            }
            set
            {
                button.ImageIndex = value;
            }
        }

        public void shortcutKeyPressed(ShortcutEventCommand shortcut)
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                controller.showDockContent(this);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (button.Image != null)
            {
                button.Image.Dispose();
            }
            button.Dispose();
        }

        internal void callSceneUnloading()
        {
            sceneUnloading();
        }

        protected virtual void sceneUnloading()
        {

        }

        internal void callSceneLoaded(SimScene scene)
        {
            sceneLoaded(scene);
        }

        protected virtual void sceneLoaded(SimScene scene)
        {

        }

        internal void callFixedLoopUpdate(Clock time)
        {
            this.fixedLoopUpdate(time);
        }

        protected virtual void fixedLoopUpdate(Clock time)
        {

        }

        internal bool matchesPersistString(String persistString)
        {
            return persistString == this.GetType().ToString();
        }

        internal void _setController(GUIElementController controller)
        {
            if (updating && controller != null)
            {
                this.controller.removeUpdatingElement(this);
            }
            this.controller = controller;
            if (updating)
            {
                this.controller.addUpdatingElement(this);
            }
        }

        protected void subscribeToUpdates()
        {
            if (!updating)
            {
                if (controller != null)
                {
                    controller.addUpdatingElement(this);
                }
                updating = true;
            }
        }

        protected void unsubscribeFromUpdates()
        {
            if (updating)
            {
                if (controller != null)
                {
                    controller.removeUpdatingElement(this);
                }
                updating = false;
            }
        }

        private void content_VisibleChanged(object sender, EventArgs e)
        {
            button.Checked = this.Visible;
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                controller.hideDockContent(this);
            }
            else
            {
                controller.showDockContent(this);
            }
        }

        public String ToolStripName { get; set; }

        //protected override void OnTextChanged(EventArgs e)
        //{
        //    base.OnTextChanged(e);
        //    button.Text = this.Text;
        //    //if (button.Image != null)
        //    //{
        //    //    button.Image.Dispose();
        //    //}
        //    //button.Image = this.Icon.ToBitmap();
        //}

        internal ToolStripButton Button
        {
            get
            {
                return button;
            }
        }

        public MedicalController MedicalController
        {
            get
            {
                return controller.MedicalController;
            }
        }

        /// <summary>
        /// This will be true if the window was hidden using hideWindows because
        /// it was visible when the function was called. Allows
        /// restoreHiddenWindows to work properly.
        /// </summary>
        internal bool RestoreFromHidden { get; set; }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GUIElement
            // 
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "GUIElement";
            this.ResumeLayout(false);
        }
    }
}
