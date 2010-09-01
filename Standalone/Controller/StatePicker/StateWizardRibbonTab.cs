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
        private PiperJBOGUI basicGUI;

        private List<Button> buttons = new List<Button>();

        public StateWizardRibbonTab(Gui gui, StateWizardController stateWizardController, PiperJBOGUI basicGUI)
        {
            this.basicGUI = basicGUI;

            ScrollView distortionTab = gui.findWidgetT("DistortionsTab/ScrollView") as ScrollView;
            int currentPosition = 3;
            foreach (StateWizard wizard in stateWizardController.WizardEnum)
            {
                String caption = wizard.TextLine1;
                if (wizard.TextLine2 != null)
                {
                    caption += "\n" + wizard.TextLine2;
                }
                Button wizardButton = distortionTab.createWidgetT("Button", "RibbonButton", currentPosition, 6, 78, 64, Align.Default, wizard.Name) as Button;
                wizardButton.Caption = caption;
                int buttonWidth = (int)wizardButton.getTextSize().Width + 10;
                wizardButton.setSize(buttonWidth, 64);
                wizardButton.UserObject = wizard;
                wizardButton.StaticImage.setItemResource(wizard.ImageKey);
                wizardButton.MouseButtonClick += new MyGUIEvent(wizardButton_MouseButtonClick);
                currentPosition += buttonWidth + 3;
            }
            Size2 scrollSize = distortionTab.CanvasSize;
            scrollSize.Width = currentPosition;
            distortionTab.CanvasSize = scrollSize;
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
            basicGUI.startWizard(source.UserObject as StateWizard);
        }
    }
}
