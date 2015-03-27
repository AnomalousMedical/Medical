using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.ObjectManagement;
using Logging;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class BoneManipulatorGUI<WizardViewType> : WizardComponent<WizardViewType>
        where WizardViewType : WizardView
    {
        private List<BoneManipulatorSlider> openingValues = new List<BoneManipulatorSlider>();

        public BoneManipulatorGUI(String panelFile, WizardViewType view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base(panelFile, view, context, viewHost)
        {
            
        }

        protected void addBoneManipulator(BoneManipulatorSlider slider)
        {
            slider.OpeningValue = 0.0f;
            openingValues.Add(slider);
        }

        public override void opening()
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
                            slider.OpeningValue = slider.Value;
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

        protected void setToDefault()
        {
            foreach (BoneManipulatorSlider slider in openingValues)
            {
                if (slider != null && slider.BoneManipulator != null)
                {
                    slider.setToDefault();
                }
            }
        }

        protected void resetToOpeningState()
        {
            foreach (BoneManipulatorSlider slider in openingValues)
            {
                if (slider != null && slider.BoneManipulator != null)
                {
                    slider.Value = slider.OpeningValue;
                }
            }
        }
    }
}
