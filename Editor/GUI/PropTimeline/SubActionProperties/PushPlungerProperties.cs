using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PushPlungerProperties : TimelineDataPanel
    {
        private NumericEdit pushPlungerEdit;
        private PushPlungerAction pushPlunger;

        public PushPlungerProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.PropTimeline.SubActionProperties.PushPlungerProperties.layout")
        {
            pushPlungerEdit = new NumericEdit(mainWidget.findWidget("PlungeAmountEdit") as EditBox);
            pushPlungerEdit.MinValue = 0.0f;
            pushPlungerEdit.MaxValue = 1.0f;
            pushPlungerEdit.Increment = 0.1f;
            pushPlungerEdit.ValueChanged += new MyGUIEvent(pushPlungerEdit_ValueChanged);
        }

        public override void setCurrentData(TimelineData data)
        {
            PropTimelineData propData = (PropTimelineData)data;
            pushPlunger = (PushPlungerAction)propData.Action;
            pushPlungerEdit.FloatValue = pushPlunger.PlungePercentage;
        }

        void pushPlungerEdit_ValueChanged(Widget source, EventArgs e)
        {
            pushPlunger.PlungePercentage = pushPlungerEdit.FloatValue;
        }
    }
}
