using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using Engine.Platform;
using Engine.ObjectManagement;
using Medical.Controller;
using ComponentFactory.Krypton.Ribbon;
using System.Drawing;
using ComponentFactory.Krypton.Navigator;

namespace Medical.GUI
{
    [Flags]
    public enum DockLocations
    {
        Left = 0,
        Right = 2,
        Bottom = 4,
        Top = 8,
        Float = 16,
    }

    public class GUIElement : UserControl
    {
        private GUIElementController controller;
        private bool updating = false;
        private Keys shortcutKey = Keys.None;
        private KryptonRibbonGroupButton button;
        private KryptonPage page;

        public GUIElement()
        {
            InitializeComponent();
            ToolStripName = "Default";

            page = new KryptonPage();
            this.Dock = DockStyle.Fill;
            page.Controls.Add(this);
        }

        public void shortcutKeyPressed(ShortcutEventCommand shortcut)
        {
            button_Click(null, null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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

        internal void _setController(GUIElementController controller, KryptonRibbonGroupButton button)
        {
            this.button = button;
            button.Click += button_Click;
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

        private void button_Click(object sender, EventArgs e)
        {
            if (controller.isVisible(this))
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
            this.Name = "GUIElement";
            this.ResumeLayout(false);
        }

        public Icon Icon { get; set; }

        public DockLocations DockAreas { get; set; }

        public DockLocations ShowHint { get; set; }

        public KryptonPage Page
        {
            get
            {
                return page;
            }
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

        public String ButtonText { get; set; }

        public new String Text
        {
            get
            {
                return page.Text;
            }
            set
            {
                page.Text = value;
                page.TextDescription = value;
            }
        }
    }
}
