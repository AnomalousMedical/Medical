using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;

namespace Medical.GUI
{
    class StateWizardRibbonTab : IDisposable
    {
        private BasicGUI basicGUI;

        private List<Button> buttons = new List<Button>();

        public StateWizardRibbonTab(Gui gui, StateWizardController stateWizardController, BasicGUI basicGUI)
        {
            this.basicGUI = basicGUI;

            Widget distortionTab = gui.findWidgetT("DistortionsTab");
            int currentPosition = 3;
            foreach (StateWizard wizard in stateWizardController.WizardEnum)
            {
                String caption = wizard.TextLine1;
                if (wizard.TextLine2 != null)
                {
                    caption += "\n" + wizard.TextLine2;
                }
                Button wizardButton = distortionTab.createWidgetT("Button", "RibbonButton", currentPosition, 6, 78, 64, Align.Default, wizard.Name) as Button;
                int buttonWidth = (int)FontManager.Instance.measureStringWidth(wizardButton.Font, caption) + 45;
                wizardButton.setSize(buttonWidth, 64);
                wizardButton.Caption = caption;
                wizardButton.UserObject = wizard.Name;
                wizardButton.MouseButtonClick += new MyGUIEvent(wizardButton_MouseButtonClick);
                currentPosition += buttonWidth + 3;
            }
        }

        public void Dispose()
        {
            foreach (Button button in buttons)
            {
                Gui.Instance.destroyWidget(button);
            }
            buttons.Clear();
        }

        void wizardButton_MouseButtonClick(Widget source, EventArgs e)
        {
            basicGUI.startWizard(source.UserObject.ToString());
        }
    }
}
