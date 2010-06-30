using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class FossaPanel : StateWizardPanel
    {
        Fossa fossa;
        private bool allowUpdates = true;
        private String fossaName;
        private float openingState = 0.0f;

        MinMaxScroll eminanceSlider;

        public FossaPanel(String fossaName, String panelFile, StateWizardPanelController controller)
            : base(panelFile, controller)
        {
            eminanceSlider = new MinMaxScroll(mainWidget.findWidget("FlatnessSlider") as VScroll);
            eminanceSlider.Minimum = 0;
            eminanceSlider.Maximum = 1000;

            eminanceSlider.ScrollChangePosition += new MyGUIEvent(eminanceSlider_ValueChanged);
            this.fossaName = fossaName;
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            fossa = FossaController.get(fossaName);
            if (fossa != null)
            {
                allowUpdates = false;
                synchronize(fossa, fossa.getEminanceDistortion());
                allowUpdates = true;
            }
        }

        void eminanceSlider_ValueChanged(Widget sender, EventArgs e)
        {
            if (allowUpdates)
            {
                synchronize(eminanceSlider, (float)eminanceSlider.Value / eminanceSlider.Maximum);
            }
        }

        public override void applyToState(MedicalState state)
        {
            state.Fossa.addPosition(fossaName, fossa.getEminanceDistortion());
        }

        public override void setToDefault()
        {
            synchronize(this, 0f);
        }

        public override void recordOpeningState()
        {
            if (fossa != null)
            {
                openingState = fossa.getEminanceDistortion();
            }
        }

        public override void resetToOpeningState()
        {
            synchronize(this, openingState);
        }

        protected override void onPanelOpening()
        {
            synchronize(fossa, fossa.getEminanceDistortion());
        }

        public String FossaName
        {
            get
            {
                return fossaName;
            }
            set
            {
                fossaName = value;
            }
        }

        //public Image NormalImage
        //{
        //    get
        //    {
        //        return normalImagePanel.BackgroundImage;
        //    }
        //    set
        //    {
        //        normalImagePanel.BackgroundImage = value;
        //    }
        //}

        //public Image DistortedImage
        //{
        //    get
        //    {
        //        return distortedImagePanel.BackgroundImage;
        //    }
        //    set
        //    {
        //        distortedImagePanel.BackgroundImage = value;
        //    }
        //}

        private void synchronize(Object source, float value)
        {
            if (source != fossa && fossa != null)
            {
                fossa.setEminanceDistortion(value);
            }
            if (source != eminanceSlider)
            {
                int newVal = (int)(value * eminanceSlider.Maximum);
                if (newVal > eminanceSlider.Maximum)
                {
                    eminanceSlider.Value = eminanceSlider.Maximum;
                }
                else if (newVal < eminanceSlider.Minimum)
                {
                    eminanceSlider.Value = eminanceSlider.Minimum;
                }
                else
                {
                    eminanceSlider.Value = newVal;
                }
            }
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to undo the fossa to before the wizard was opened?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                resetToOpeningState();
            }
        }

        private void makeNormalButton_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to reset the fossa to normal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                setToDefault();
            }
        }
    }
}
