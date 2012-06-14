using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowPropActionData : TimelineActionData
    {
        private ShowPropAction showProp;
        private PropEditController propEditController;

        public ShowPropActionData(ShowPropAction showPropAction, PropEditController propEditController)
            :base(showPropAction)
        {
            this.propEditController = propEditController;
            this.showProp = showPropAction;
        }

        public override void Dispose()
        {
            unsubscribeActionEvents();
            base.Dispose();
        }

        public override void editingStarted()
        {
            base.editingStarted();
            showProp.DurationChanged += showProp_DurationChanged;
            showProp.PropTypeChanged += showProp_PropTypeChanged;
            propEditController.CurrentShowPropAction = showProp;
        }

        public override void editingCompleted()
        {
            unsubscribeActionEvents();
            propEditController.CurrentShowPropAction = null;
            base.editingCompleted();
        }

        void showProp_DurationChanged(TimelineAction action)
        {
            propEditController.Duration = action.Duration;
        }

        void showProp_PropTypeChanged(ShowPropAction obj)
        {
            propEditController.CurrentShowPropAction = showProp;
        }

        private void unsubscribeActionEvents()
        {
            showProp.DurationChanged -= showProp_DurationChanged;
            showProp.PropTypeChanged -= showProp_PropTypeChanged;
        }
    }
}
