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
        private ToolStripButton playbackButton;
        private ToolStripButton animationButton;
        private LayersControl layersControl = new LayersControl();
        private PictureControl pictureControl = new PictureControl();
        private PlaybackGUI playbackGUI = new PlaybackGUI();
        private CommonController controller;
        private BlendEditor blendEditor = new BlendEditor();
        private StateEditor stateEditor = new StateEditor();

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

            playbackButton = new ToolStripButton("Playback");
            playbackButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            playbackButton.Click += new EventHandler(playbackButton_Click);
            this.Items.Add(playbackButton);

            animationButton = new ToolStripButton("Animation");
            animationButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            animationButton.Click += new EventHandler(animationButton_Click);
            this.Items.Add(animationButton);

            layersControl.VisibleChanged += new EventHandler(layersControl_VisibleChanged);
            pictureControl.VisibleChanged += new EventHandler(pictureControl_VisibleChanged);
            playbackGUI.VisibleChanged += new EventHandler(playbackGUI_VisibleChanged);
            blendEditor.VisibleChanged += new EventHandler(blendEditor_VisibleChanged);

            playbackGUI.initialize(medicalController);
            blendEditor.initialize(medicalController);
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
            if (persistString == playbackGUI.GetType().ToString())
            {
                return playbackGUI;
            }
            if (persistString == blendEditor.GetType().ToString())
            {
                return blendEditor;
            }
            if (persistString == stateEditor.GetType().ToString())
            {
                return stateEditor;
            }
            return null;
        }

        public void sceneChanged()
        {
            layersControl.sceneLoaded();
            playbackGUI.sceneLoaded();
            blendEditor.sceneLoaded();
        }

        public void sceneUnloading()
        {
            layersControl.sceneUnloading();
            playbackGUI.sceneUnloading();
            blendEditor.sceneUnloading();
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

        void playbackButton_Click(object sender, EventArgs e)
        {
            if (playbackGUI.Visible)
            {
                controller.removeControl(playbackGUI);
            }
            else
            {
                controller.addControlToUI(playbackGUI);
            }
        }

        void playbackGUI_VisibleChanged(object sender, EventArgs e)
        {
            playbackButton.Checked = playbackGUI.Visible;
        }

        void animationButton_Click(object sender, EventArgs e)
        {
            if (blendEditor.Visible)
            {
                controller.removeControl(blendEditor);
                controller.removeControl(stateEditor);
            }
            else
            {
                controller.addControlToUI(blendEditor);
                if (stateEditor.DockPanel == null)
                {
                    stateEditor.Show(blendEditor.Pane, DockAlignment.Left, (double)stateEditor.Width / blendEditor.Width);
                }
                else
                {
                    controller.addControlToUI(stateEditor);
                }
            }
        }

        void blendEditor_VisibleChanged(object sender, EventArgs e)
        {
            animationButton.Checked = blendEditor.Visible;
        }
    }
}
