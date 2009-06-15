using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.GUI;
using WeifenLuo.WinFormsUI.Docking;
using Medical.GUI;

namespace Medical
{
    class CommonToolStrip : ToolStrip
    {
        private ToolStripButton layersButton;
        private ToolStripButton pictureButton;
        private ToolStripButton animationButton;
        private LayersControl layersControl = new LayersControl();
        private PictureControl pictureControl = new PictureControl();
        private AnimationGUI animationGUI = new AnimationGUI();
        private CommonController controller;

        public CommonToolStrip(CommonController controller, MedicalController medicalController)
        {
            this.controller = controller;
            layersButton = new ToolStripButton("Layers");
            layersButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            layersButton.Click += new EventHandler(layersButton_Click);
            this.Items.Add(layersButton);

            pictureButton = new ToolStripButton("Picture");
            pictureButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            pictureButton.Click += new EventHandler(pictureButton_Click);
            this.Items.Add(pictureButton);

            animationButton = new ToolStripButton("Animation");
            animationButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            animationButton.Click += new EventHandler(animationButton_Click);
            this.Items.Add(animationButton);

            layersControl.VisibleChanged += new EventHandler(layersControl_VisibleChanged);
            pictureControl.VisibleChanged += new EventHandler(pictureControl_VisibleChanged);
            animationGUI.VisibleChanged += new EventHandler(animationGUI_VisibleChanged);

            animationGUI.initialize(medicalController);
        }

        /// <summary>
        /// Used when restoring window positions. Return the window matching the
        /// persistString or null if no match is found.
        /// </summary>
        /// <param name="persistString">A string describing the window.</param>
        /// <returns>The matching DockContent or null if none is found.</returns>
        public DockContent getDockContent(String persistString)
        {
            if (persistString == layersControl.GetType().ToString())
            {
                return layersControl;
            }
            if (persistString == pictureControl.GetType().ToString())
            {
                return pictureControl;
            }
            if (persistString == animationGUI.GetType().ToString())
            {
                return animationGUI;
            }
            return null;
        }

        public void sceneChanged()
        {
            layersControl.sceneLoaded();
            animationGUI.sceneLoaded();
        }

        public void sceneUnloading()
        {
            layersControl.sceneUnloading();
            animationGUI.sceneUnloading();
        }

        private void layersButton_Click(object sender, EventArgs e)
        {
            if (layersControl.Visible)
            {
                controller.removeControl(layersControl);
            }
            else
            {
                controller.addControlToUI(layersControl);
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
                controller.removeControl(pictureControl);
            }
            else
            {
                controller.addControlToUI(pictureControl);
            }
        }

        void pictureControl_VisibleChanged(object sender, EventArgs e)
        {
            pictureButton.Checked = pictureControl.Visible;
        }

        void animationButton_Click(object sender, EventArgs e)
        {
            if (animationGUI.Visible)
            {
                controller.removeControl(animationGUI);
            }
            else
            {
                controller.addControlToUI(animationGUI);
            }
        }

        void animationGUI_VisibleChanged(object sender, EventArgs e)
        {
            animationButton.Checked = animationGUI.Visible;
        }
    }
}
