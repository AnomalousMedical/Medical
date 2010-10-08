using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class MoveCameraProperties : ActionPropertiesPanel
    {
        private MoveCameraAction moveAction;
        private Edit translationEdit;
        private Edit lookAtEdit;

        public MoveCameraProperties(Widget parent)
            :base(parent, "Medical.GUI.Timeline.ActionProperties.MoveCameraProperties.layout")
        {
            translationEdit = mainWidget.findWidget("TranslationEdit") as Edit;
            lookAtEdit = mainWidget.findWidget("LookAtEdit") as Edit;

            Button useCurrentButton = mainWidget.findWidget("UseCurrentButton") as Button;
            useCurrentButton.MouseButtonClick += new MyGUIEvent(useCurrentButton_MouseButtonClick);
        }

        public override TimelineAction CurrentAction
        {
            get
            {
                return moveAction;
            }
            set
            {
                moveAction = (MoveCameraAction)value;
                if (moveAction != null)
                {
                    translationEdit.Caption = moveAction.Translation.ToString();
                    lookAtEdit.Caption = moveAction.LookAt.ToString();
                }
            }
        }

        void useCurrentButton_MouseButtonClick(Widget source, EventArgs e)
        {
            moveAction.captureFromScene();
            translationEdit.Caption = moveAction.Translation.ToString();
            lookAtEdit.Caption = moveAction.LookAt.ToString();
        }
    }
}
