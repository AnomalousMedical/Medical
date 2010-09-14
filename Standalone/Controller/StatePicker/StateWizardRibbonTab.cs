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

        public StateWizardRibbonTab(Widget ribbonWidget, StateWizardController stateWizardController, PiperJBOGUI basicGUI)
        {
            this.basicGUI = basicGUI;

            ScrollView distortionTab = ribbonWidget.findWidget("DistortionsTab/ScrollView") as ScrollView;

            Widget anatomyDistortionPanel = ribbonWidget.findWidget("AnatomyDistortionPanel");
            Widget examDistortionPanel = ribbonWidget.findWidget("ExamDistortionPanel");

            int anatomyPosition = 3;
            int examPosition = 7;
            foreach (StateWizard wizard in stateWizardController.WizardEnum)
            {
                String caption = wizard.TextLine1;
                if (wizard.TextLine2 != null)
                {
                    caption += "\n" + wizard.TextLine2;
                }
                Button wizardButton;
                if (wizard.WizardType == WizardType.Anatomy)
                {
                    wizardButton = anatomyDistortionPanel.createWidgetT("Button", "RibbonButton", anatomyPosition, 2, 78, 68, Align.Default, wizard.Name) as Button;
                }
                else
                {
                    wizardButton = examDistortionPanel.createWidgetT("Button", "RibbonButton", examPosition, 2, 78, 68, Align.Default, wizard.Name) as Button;
                }
                wizardButton.Caption = caption;
                int buttonWidth = (int)wizardButton.getTextSize().Width + 10;
                if (buttonWidth < 38)
                {
                    buttonWidth = 38;
                }
                wizardButton.setSize(buttonWidth, wizardButton.Height);
                wizardButton.UserObject = wizard;
                wizardButton.StaticImage.setItemResource(wizard.ImageKey);
                wizardButton.MouseButtonClick += new MyGUIEvent(wizardButton_MouseButtonClick);
                if (wizard.WizardType == WizardType.Anatomy)
                {
                    anatomyPosition += buttonWidth + 3;
                }
                else
                {
                    examPosition += buttonWidth + 3;
                }
            }
            anatomyPosition -= 3;
            examPosition -= 3;
            anatomyDistortionPanel.setSize(anatomyPosition, anatomyDistortionPanel.Height);
            examDistortionPanel.setSize(examPosition, examDistortionPanel.Height);
            examDistortionPanel.setPosition(anatomyDistortionPanel.Right, examDistortionPanel.Top);

            Size2 scrollSize = distortionTab.CanvasSize;
            scrollSize.Width = examDistortionPanel.Right;
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
