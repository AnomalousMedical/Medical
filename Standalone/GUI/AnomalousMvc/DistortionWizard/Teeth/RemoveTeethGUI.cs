using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class RemoveTeethGUI : WizardPanel<WizardView>
    {
        private List<ToothButton> toothButtons = new List<ToothButton>();

        public RemoveTeethGUI(String layoutFile, WizardView wizardView, AnomalousMvcContext context)
            : base(layoutFile, wizardView, context)
        {
            Widget toothPanel = widget.findWidget("ToothPanel");
            uint numChildren = toothPanel.ChildCount;
            for (uint i = 0; i < numChildren; ++i)
            {
                Button toothGUIButton = toothPanel.getChildAt(i) as Button;
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
            ToothButton button = (ToothButton)sender;
            Tooth tooth = TeethController.getTooth(button.ToothName);
            if (tooth != null)
            {
                tooth.Extracted = button.Extracted;
            }
        }
    }
}
