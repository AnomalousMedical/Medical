using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medical.GUI;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical
{
    class CommonToolStrip : ToolStrip
    {
        private ToolStripButton layersButton;
        private ToolStripButton pictureButton;
        private ToolStripButton stateButton;
        private ToolStripButton camerasButton;
        private LayersControl layersControl = new LayersControl();
        private PictureControl pictureControl = new PictureControl();
        private MedicalStateGUI medicalState = new MedicalStateGUI();
        private SavedCameraGUI savedCameras = new SavedCameraGUI();
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

            stateButton = new ToolStripButton("State");
            stateButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            stateButton.Click += new EventHandler(stateButton_Click);
            this.Items.Add(stateButton);

            camerasButton = new ToolStripButton("Cameras");
            camerasButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            camerasButton.Click += new EventHandler(camerasButton_Click);
            this.Items.Add(camerasButton);

            layersControl.VisibleChanged += new EventHandler(layersControl_VisibleChanged);
            pictureControl.VisibleChanged += new EventHandler(pictureControl_VisibleChanged);
            medicalState.VisibleChanged += new EventHandler(medicalState_VisibleChanged);
            savedCameras.VisibleChanged += new EventHandler(savedCameras_VisibleChanged);

            medicalState.initialize(medicalController);
            savedCameras.initialize(medicalController);
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
            if (persistString == medicalState.GetType().ToString())
            {
                return medicalState;
            }
            if (persistString == savedCameras.GetType().ToString())
            {
                return savedCameras;
            }
            return null;
        }

        public void sceneChanged()
        {
            layersControl.sceneLoaded();
            medicalState.sceneLoaded();
        }

        public void sceneUnloading()
        {
            layersControl.sceneUnloading();
            medicalState.sceneUnloading();
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

        void stateButton_Click(object sender, EventArgs e)
        {
            if (medicalState.Visible)
            {
                controller.removeControl(medicalState);
            }
            else
            {
                controller.addControlToUI(medicalState);
            }
        }

        void medicalState_VisibleChanged(object sender, EventArgs e)
        {
            stateButton.Checked = medicalState.Visible;
        }

        void camerasButton_Click(object sender, EventArgs e)
        {
            if (savedCameras.Visible)
            {
                controller.removeControl(savedCameras);
            }
            else
            {
                controller.addControlToUI(savedCameras);
            }
        }

        void savedCameras_VisibleChanged(object sender, EventArgs e)
        {
            camerasButton.Checked = savedCameras.Visible;
        }
    }
}
