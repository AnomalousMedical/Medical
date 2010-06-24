using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ToothPanel : StateWizardPanel
    {
        private List<ToothButton> toothButtons = new List<ToothButton>();

        public ToothPanel(String toothPanelFile, Button button, BasicGUI basicGUI)
            :base(toothPanelFile, button, basicGUI)
        {
            uint numChildren = mainWidget.getChildCount();
            for (uint i = 0; i < numChildren; ++i)
            {
                Button toothGUIButton = mainWidget.getChildAt(i) as Button;
                if (toothGUIButton != null)
                {
                    toothButtons.Add(new ToothButton(toothGUIButton));
                }
            }
        }
    }
}
