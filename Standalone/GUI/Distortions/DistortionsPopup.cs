using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class DistortionsPopup : PopupContainer
    {
        private GUIManager guiManager;

        private List<Button> buttons = new List<Button>();

        public DistortionsPopup(StateWizardController stateWizardController, GUIManager guiManager)
            :base("Medical.GUI.Distortions.DistortionsPopup.layout")
        {
            this.guiManager = guiManager;

            Widget anatomyDistortionPanel = widget.findWidget("AnatomyDistortionPanel");
            Widget examDistortionPanel = widget.findWidget("ExamDistortionPanel");

            int anatomyPosition = 3;
            int examPosition = 3;
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
            //anatomyPosition -= 3;
            //examPosition -= 3;
            anatomyDistortionPanel.setSize(anatomyPosition, anatomyDistortionPanel.Height);
            examDistortionPanel.setSize(examPosition, examDistortionPanel.Height);

            Size2 size = new Size2(widget.Width, widget.Height);
            if (examDistortionPanel.Right > anatomyDistortionPanel.Right)
            {
                size.Width = examDistortionPanel.Right;
            }
            else
            {
                size.Width = anatomyDistortionPanel.Right;
            }
            widget.setSize((int)size.Width, (int)size.Height);
        }

        public override void Dispose()
        {
            foreach (Button button in buttons)
            {
                Gui.Instance.destroyWidget(button);
            }
            buttons.Clear();
            base.Dispose();
        }

        void wizardButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
            guiManager.startWizard(source.UserObject as StateWizard);
        }
    }
}
