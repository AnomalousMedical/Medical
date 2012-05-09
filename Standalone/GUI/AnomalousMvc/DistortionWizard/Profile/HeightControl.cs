using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public class HeightControl
    {
        private bool allowUpdates = true;
        private HeightComboSlider leftHeightSlider;
        private HeightComboSlider rightHeightSlider;
        private MinMaxScroll bothSidesSlider;

        public HeightControl(ScrollBar leftHeightScroll, ScrollBar rightHeightScroll, ScrollBar bothSidesScroll)
        {
            leftHeightSlider = new HeightComboSlider(leftHeightScroll);
            rightHeightSlider = new HeightComboSlider(rightHeightScroll);
            bothSidesSlider = new MinMaxScroll(bothSidesScroll);

            bothSidesSlider.ScrollChangePosition += new MyGUIEvent(bothSidesSlider_ScrollChangePosition);
            bothSidesSlider.Minimum = 0;
            bothSidesSlider.Maximum = 1000;
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

        public void getPositionFromScene()
        {
            leftHeightSlider.getPositionFromScene();
            rightHeightSlider.getPositionFromScene();
            syncBothSidesSlider();
        }

        public void setToDefault()
        {
            leftHeightSlider.setToDefault();
            rightHeightSlider.setToDefault();
            syncBothSidesSlider();
        }

        void bothSidesSlider_ScrollChangePosition(Widget source, EventArgs e)
        {
            if (allowUpdates)
            {
                leftHeightSlider.Value = (float)bothSidesSlider.Value / bothSidesSlider.Maximum;
                rightHeightSlider.Value = (float)bothSidesSlider.Value / bothSidesSlider.Maximum;
            }
        }

        private void syncBothSidesSlider()
        {
            allowUpdates = false;
            int newVal = (int)((leftHeightSlider.Value + rightHeightSlider.Value) / 2.0 * bothSidesSlider.Maximum);
            if (newVal > bothSidesSlider.Maximum)
            {
                bothSidesSlider.Value = bothSidesSlider.Maximum;
            }
            else if (newVal < bothSidesSlider.Minimum)
            {
                bothSidesSlider.Value = bothSidesSlider.Minimum;
            }
            else
            {
                bothSidesSlider.Value = newVal;
            }
            allowUpdates = true;
        }
    }
}
