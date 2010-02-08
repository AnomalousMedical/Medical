using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI.BoneManipulator
{
    public partial class HeightControl : UserControl
    {
        public HeightControl()
        {
            InitializeComponent();
        }

        public void sceneChanged()
        {
            AnimationManipulator ramus = MandibleController.Mandible.getAnimationManipulator("leftRamusHeightMandible");
            AnimationManipulator condyle = MandibleController.Mandible.getAnimationManipulator("leftCondyleHeightMandible");
            leftHeightSlider.initialize(condyle, ramus);

            ramus = MandibleController.Mandible.getAnimationManipulator("rightRamusHeightMandible");
            condyle = MandibleController.Mandible.getAnimationManipulator("rightCondyleHeightMandible");
            rightHeightSlider.initialize(condyle, ramus);

            bothSidesSlider.ValueChanged += new EventHandler(bothSidesSlider_ValueChanged);
        }

        public void getPositionFromScene()
        {
            leftHeightSlider.getPositionFromScene();
            rightHeightSlider.getPositionFromScene();
        }

        void bothSidesSlider_ValueChanged(object sender, EventArgs e)
        {
            leftHeightSlider.Value = (float)bothSidesSlider.Value / bothSidesSlider.Maximum;
            rightHeightSlider.Value = (float)bothSidesSlider.Value / bothSidesSlider.Maximum;
        }
    }
}
