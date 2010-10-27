using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class MoveCameraProperties : TimelineDataPanel
    {
        private MoveCameraAction moveAction;
        private Edit translationEdit;
        private Edit lookAtEdit;
        private Color defaultColor;
        private StaticText cameraText;

        public MoveCameraProperties(Widget parent)
            :base(parent, "Medical.GUI.Timeline.ActionProperties.MoveCameraProperties.layout")
        {
            translationEdit = mainWidget.findWidget("TranslationEdit") as Edit;
            translationEdit.EventEditTextChange += new MyGUIEvent(translationEdit_EventEditTextChange);
            lookAtEdit = mainWidget.findWidget("LookAtEdit") as Edit;
            lookAtEdit.EventEditTextChange += new MyGUIEvent(lookAtEdit_EventEditTextChange);
            defaultColor = translationEdit.TextColor;

            Button useCurrentButton = mainWidget.findWidget("UseCurrentButton") as Button;
            useCurrentButton.MouseButtonClick += new MyGUIEvent(useCurrentButton_MouseButtonClick);

            Button previewButton = mainWidget.findWidget("PreviewButton") as Button;
            previewButton.MouseButtonClick += new MyGUIEvent(previewButton_MouseButtonClick);

            cameraText = mainWidget.findWidget("CameraText") as StaticText;
        }

        public override void setCurrentData(TimelineData data)
        {
            moveAction = (MoveCameraAction)((TimelineActionData)data).Action;
            if (moveAction != null)
            {
                translationEdit.Caption = moveAction.Translation.ToString();
                lookAtEdit.Caption = moveAction.LookAt.ToString();
                cameraText.Caption = moveAction.CameraName;
            }
        }

        void useCurrentButton_MouseButtonClick(Widget source, EventArgs e)
        {
            moveAction.capture();
            translationEdit.Caption = moveAction.Translation.ToString();
            lookAtEdit.Caption = moveAction.LookAt.ToString();
            cameraText.Caption = moveAction.CameraName;
        }

        void previewButton_MouseButtonClick(Widget source, EventArgs e)
        {
            moveAction.preview();
        }

        void translationEdit_EventEditTextChange(Widget source, EventArgs e)
        {
            Vector3 newValue = new Vector3();
            if (newValue.setValue(translationEdit.Caption))
            {
                moveAction.Translation = newValue;
                translationEdit.TextColor = defaultColor;
            }
            else
            {
                translationEdit.TextColor = Color.Red;
            }
        }

        void lookAtEdit_EventEditTextChange(Widget source, EventArgs e)
        {
            Vector3 newValue = new Vector3();
            if (newValue.setValue(lookAtEdit.Caption))
            {
                moveAction.LookAt = newValue;
                lookAtEdit.TextColor = defaultColor;
            }
            else
            {
                lookAtEdit.TextColor = Color.Red;
            }
        }
    }
}
