using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Logging;

namespace Medical.GUI
{
    public partial class BoneManipulatorPanel : StatePickerPanel
    {
        private Dictionary<BoneManipulatorSlider, float> openingValues = new Dictionary<BoneManipulatorSlider, float>();

        /// <summary>
        /// Do not call this constructor, it exists to make the designer work
        /// </summary>
        protected BoneManipulatorPanel()
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                throw new NotImplementedException();
            }
        }

        public BoneManipulatorPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            foreach (Control control in Controls)
            {
                BoneManipulatorSlider slider = control as BoneManipulatorSlider;
                if (slider != null && slider.Tag != null)
                {
                    openingValues.Add(slider, 0);
                }
            }
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            sceneLoaded(medicalController.CurrentScene);
        }

        public void sceneLoaded(SimScene scene)
        {
            foreach (Control control in Controls)
            {
                BoneManipulatorSlider slider = control as BoneManipulatorSlider;
                if (slider != null)
                {
                    if (slider.Tag != null)
                    {
                        AnimationManipulator manipulator = MandibleController.Mandible.getAnimationManipulator(slider.Tag.ToString());
                        if (manipulator != null)
                        {
                            slider.initialize(manipulator);
                        }
                        else
                        {
                            Log.Default.sendMessage("Could not find manipulator named {0}.", LogLevel.Warning, "Head", slider.Tag.ToString());
                        }
                    }
                    else
                    {
                        Log.Default.sendMessage("No tag set on slider. Cannot search for manipulator.", LogLevel.Warning, "Head");
                    }
                }
            }
        }

        public override void applyToState(MedicalState state)
        {
            foreach (Control control in Controls)
            {
                BoneManipulatorSlider slider = control as BoneManipulatorSlider;
                if (slider != null && slider.Tag != null)
                {
                    state.BoneManipulator.addPosition(slider.createStateEntry());
                }
            }
        }

        public override void setToDefault()
        {
            foreach (Control control in Controls)
            {
                BoneManipulatorSlider slider = control as BoneManipulatorSlider;
                if (slider != null && slider.Tag != null)
                {
                    slider.setToDefault();
                }
            }
        }

        public override void recordOpeningState()
        {
            foreach (Control control in Controls)
            {
                BoneManipulatorSlider slider = control as BoneManipulatorSlider;
                if (slider != null && slider.Tag != null)
                {
                    openingValues[slider] = slider.Value;
                }
            }
        }

        public override void resetToOpeningState()
        {
            foreach (Control control in Controls)
            {
                BoneManipulatorSlider slider = control as BoneManipulatorSlider;
                if (slider != null && slider.Tag != null)
                {
                    slider.Value = openingValues[slider];
                }
            }
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            foreach (Control control in Controls)
            {
                BoneManipulatorSlider slider = control as BoneManipulatorSlider;
                if (slider != null && slider.Tag != null)
                {
                    slider.updateFromScene();
                }
            }
        }
    }
}
