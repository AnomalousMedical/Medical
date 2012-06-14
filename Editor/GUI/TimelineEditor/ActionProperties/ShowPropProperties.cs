using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowPropProperties : TimelineDataPanel
    {
        private ShowPropAction showProp;
        private PropEditController propEditController;

        public ShowPropProperties(Widget parentWidget, PropEditController propEditController)
            :base(parentWidget, "Medical.GUI.TimelineEditor.ActionProperties.ShowPropProperties.layout")
        {
            this.propEditController = propEditController;
        }

        public override void Dispose()
        {
            unsubscribeActionEvents();
            base.Dispose();
        }

        public override void setCurrentData(TimelineData data)
        {
            unsubscribeActionEvents();
            TimelineActionData actionData = ((TimelineActionData)data);
            showProp = (ShowPropAction)actionData.Action;
            if (showProp != null)
            {
                showProp.DurationChanged += actionData_DurationChanged;
                showProp.PropTypeChanged += showProp_PropTypeChanged;
            }

            propEditController.CurrentShowPropAction = showProp;
        }

        public override void editingCompleted()
        {
            unsubscribeActionEvents();
            showProp = null;
            propEditController.CurrentShowPropAction = null;
        }

        void actionData_DurationChanged(TimelineAction action)
        {
            propEditController.Duration = action.Duration;
        }

        void showProp_PropTypeChanged(ShowPropAction obj)
        {
            propEditController.CurrentShowPropAction = showProp;
        }

        private void unsubscribeActionEvents()
        {
            if (showProp != null)
            {
                showProp.DurationChanged -= actionData_DurationChanged;
                showProp.PropTypeChanged -= showProp_PropTypeChanged;
            }
        }
    }
}
