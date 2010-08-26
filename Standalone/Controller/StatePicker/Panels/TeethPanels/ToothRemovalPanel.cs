using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ToothRemovalPanel : StateWizardPanel
    {
        private Dictionary<ToothButton, bool> openCheckStatus = new Dictionary<ToothButton, bool>();
        private List<ToothButton> toothButtons = new List<ToothButton>();

        public ToothRemovalPanel(String toothPanelFile, StateWizardPanelController controller)
            : base(toothPanelFile, controller)
        {
            uint numChildren = mainWidget.getChildCount();
            for (uint i = 0; i < numChildren; ++i)
            {
                Button toothGUIButton = mainWidget.getChildAt(i) as Button;
                if (toothGUIButton != null)
                {
                    ToothButton toothButton = new ToothButton(toothGUIButton);
                    toothButtons.Add(toothButton);
                    toothButton.ExtractedStatusChanged += new EventHandler(toothButton_ExtractedStatusChanged);
                }
            }
        }

        void toothButton_ExtractedStatusChanged(object sender, EventArgs e)
        {
            showChanges(true);
        }

        public override void applyToState(MedicalState state)
        {
            TeethState teethState = state.Teeth;
            foreach (ToothButton button in toothButtons)
            {
                teethState.addPosition(new ToothState(button.ToothName, button.Extracted, Vector3.Zero, Quaternion.Identity));
            }
        }

        public override void setToDefault()
        {
            foreach (ToothButton button in toothButtons)
            {
                button.Extracted = false;
            }
        }

        protected override void onPanelOpening()
        {
            //allowUpdates = false;
            foreach (ToothButton button in toothButtons)
            {
                button.Extracted = TeethController.getTooth(button.ToothName).Extracted;
            }
            //allowUpdates = true;
        }

        public override void recordOpeningState()
        {
            foreach (ToothButton button in toothButtons)
            {
                openCheckStatus[button] = TeethController.getTooth(button.ToothName).Extracted;
            }
        }

        public override void resetToOpeningState()
        {
            foreach (ToothButton button in toothButtons)
            {
                button.Extracted = openCheckStatus[button];
            }
        }
    }
}
