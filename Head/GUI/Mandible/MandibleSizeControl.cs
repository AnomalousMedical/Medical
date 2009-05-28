using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    public partial class MandibleSizeControl : DockContent
    {
        public MandibleSizeControl()
        {
            InitializeComponent();
        }            

        public void sceneChanged()
        {
            layoutPanel.Controls.Clear();
            Dictionary<String, BoneManipulator> manipulators = BoneManipulatorController.getManipulators();
            foreach (BoneManipulator manipulator in manipulators.Values)
            {
                BoneManipulatorSlider slider = new BoneManipulatorSlider();
                slider.initialize(manipulator);
                this.layoutPanel.Controls.Add(slider);
            }
        }

        internal void sceneUnloading()
        {
            foreach (Control control in layoutPanel.Controls)
            {
                control.Dispose();
            }
            layoutPanel.Controls.Clear();
        }
    }
}
