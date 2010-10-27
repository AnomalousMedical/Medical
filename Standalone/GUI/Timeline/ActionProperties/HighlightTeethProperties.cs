using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class HighlightTeethProperties : TimelineDataPanel
    {
        private HighlightTeethAction highlightTeeth;
        private CheckButton enableHighlight;

        public HighlightTeethProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.HighlightTeethProperties.layout")
        {
            enableHighlight = new CheckButton(mainWidget.findWidget("EnableHighlight") as Button);
            enableHighlight.CheckedChanged += new MyGUIEvent(enableHighlight_CheckedChanged);
        }

        public override void setCurrentData(TimelineData data)
        {
            highlightTeeth = (HighlightTeethAction)((TimelineActionData)data).Action;
            if (highlightTeeth != null)
            {
                enableHighlight.Checked = highlightTeeth.EnableHighlight;
            }   
        }

        void enableHighlight_CheckedChanged(Widget source, EventArgs e)
        {
            highlightTeeth.EnableHighlight = enableHighlight.Checked;
        }
    }
}
