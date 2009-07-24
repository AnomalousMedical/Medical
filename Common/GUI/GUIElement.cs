﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using Engine.Platform;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public class GUIElement : DockContent
    {
        private ToolStripButton button;
        private GUIElementController controller;

        public GUIElement()
        {
            InitializeComponent();
            ToolStripName = "Default";
            this.VisibleChanged += new EventHandler(content_VisibleChanged);
            button = new ToolStripButton();
            button.DisplayStyle = ToolStripItemDisplayStyle.Text;
            button.Click += new EventHandler(button_Click);
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
            this.controller = controller;
        }

        protected void subscribeToUpdates()
        {
            controller.addUpdatingElement(this);
        }

        protected void unsubscribeFromUpdates()
        {
            controller.removeUpdatingElement(this);
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

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            button.Text = this.Text;
        }

        internal ToolStripButton Button
        {
            get
            {
                return button;
            }
        }

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
