using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ChangeMedicalStateProperties : ActionPropertiesPanel
    {
        private ChangeMedicalStateAction changeStateAction;

        public ChangeMedicalStateProperties(Widget parentWidget)
            : base(parentWidget, "Medical.GUI.Timeline.ActionProperties.ChangeMedicalStateProperties.layout")
        {
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);

            Button preview = mainWidget.findWidget("Preview") as Button;
            preview.MouseButtonClick += new MyGUIEvent(preview_MouseButtonClick);
        }

        public override TimelineAction CurrentAction
        {
            get
            {
                return changeStateAction;
            }
            set
            {
                changeStateAction = (ChangeMedicalStateAction)value;
            }
        }

        void preview_MouseButtonClick(Widget source, EventArgs e)
        {
            changeStateAction.preview();
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            changeStateAction.captureCurrent();
        }
    }
}
