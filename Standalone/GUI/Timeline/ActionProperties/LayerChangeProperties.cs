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

            Button preview = mainWidget.findWidget("Preview") as Button;
            preview.MouseButtonClick += new MyGUIEvent(preview_MouseButtonClick);

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

        void preview_MouseButtonClick(Widget source, EventArgs e)
        {
            layerChange.preview();
        }

        void capture_MouseButtonClick(Widget source, EventArgs e)
        {
            layerChange.capture();
            windowText.Caption = layerChange.TransparencyState;
        }
    }
}
