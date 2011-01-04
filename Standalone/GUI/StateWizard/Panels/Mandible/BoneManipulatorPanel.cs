using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.ObjectManagement;
using Logging;

namespace Medical.GUI
{
    public class BoneManipulatorPanel : StateWizardPanel
    {
        private List<BoneManipulatorSlider> openingValues = new List<BoneManipulatorSlider>();

        public BoneManipulatorPanel(String panelFile, StateWizardPanelController controller)
            : base(panelFile, controller)
        {
            
        }

        protected void addBoneManipulator(BoneManipulatorSlider slider)
        {
            slider.OpeningValue = 0.0f;
            openingValues.Add(slider);
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            sceneLoaded(medicalController.CurrentScene);
        }

        public void sceneLoaded(SimScene scene)
        {
            foreach (BoneManipulatorSlider slider in openingValues)
            {
                if (slider != null)
                {
                    if (slider.BoneManipulator != null)
                    {
                        AnimationManipulator manipulator = MandibleController.Mandible.getAnimationManipulator(slider.BoneManipulator);
                        if (manipulator != null)
                        {
                            slider.initialize(manipulator);
                        }
                        else
                        {
                            Log.Default.sendMessage("Could not find manipulator named {0}.", LogLevel.Warning, "Head", slider.BoneManipulator);
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
            foreach (BoneManipulatorSlider slider in openingValues)
            {
                if (slider != null && slider.BoneManipulator != null)
                {
                    state.BoneManipulator.addPosition(slider.createStateEntry());
                }
            }
        }

        public override void setToDefault()
        {
            foreach (BoneManipulatorSlider slider in openingValues)
            {
                if (slider != null && slider.BoneManipulator != null)
                {
                    slider.setToDefault();
                }
            }
        }

        public override void recordOpeningState()
        {
            foreach (BoneManipulatorSlider slider in openingValues)
            {
                if (slider != null && slider.BoneManipulator != null)
                {
                    slider.OpeningValue = slider.Value;
                }
            }
        }

        public override void resetToOpeningState()
        {
            foreach (BoneManipulatorSlider slider in openingValues)
            {
                if (slider != null && slider.BoneManipulator != null)
                {
                    slider.Value = slider.OpeningValue;
                }
            }
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            foreach (BoneManipulatorSlider slider in openingValues)
            {
                if (slider != null && slider.BoneManipulator != null)
                {
                    slider.updateFromScene();
                }
            }
        }
    }
}
