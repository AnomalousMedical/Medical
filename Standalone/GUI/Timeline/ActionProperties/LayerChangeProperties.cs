using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class LayerChangeProperties : TimelineDataPanel
    {
        private LayerChangeAction layerChange;
        private StaticText windowText;

        public LayerChangeProperties(Widget parentWidget)
            : base(parentWidget, "Medical.GUI.Timeline.ActionProperties.LayerChangeProperties.layout")
        {
            Button capture = mainWidget.findWidget("Capture") as Button;
            capture.MouseButtonClick += new MyGUIEvent(capture_MouseButtonClick);

            windowText = mainWidget.findWidget("WindowText") as StaticText;
        }

        public override void setCurrentData(TimelineData data)
        {
            layerChange = (LayerChangeAction)((TimelineActionData)data).Action;
            if (layerChange != null)
            {
                windowText.Caption = layerChange.TransparencyState;
            }
        }

        void capture_MouseButtonClick(Widget source, EventArgs e)
        {
            layerChange.capture();
            windowText.Caption = layerChange.TransparencyState;
        }
    }
}
