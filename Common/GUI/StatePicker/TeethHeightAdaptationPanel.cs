using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class TeethHeightAdaptationPanel : TeethAdaptationPanel
    {
        public TeethHeightAdaptationPanel()
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
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            leftHeightSlider.getPositionFromScene();
            rightHeightSlider.getPositionFromScene();
        }
    }
}
