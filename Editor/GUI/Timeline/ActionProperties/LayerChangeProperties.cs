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
        private TextBox windowText;
        private CheckButton useSystemDuration;

        public LayerChangeProperties(Widget parentWidget)
            : base(parentWidget, "Medical.GUI.Timeline.ActionProperties.LayerChangeProperties.layout")
        {
            Button capture = mainWidget.findWidget("Capture") as Button;
            capture.MouseButtonClick += new MyGUIEvent(capture_MouseButtonClick);

            windowText = mainWidget.findWidget("WindowText") as TextBox;

            useSystemDuration = new CheckButton(mainWidget.findWidget("UseSystemDuration") as Button);
            useSystemDuration.CheckedChanged += new MyGUIEvent(useSystemDuration_CheckedChanged);
        }

        public override void setCurrentData(TimelineData data)
        {
            layerChange = (LayerChangeAction)((TimelineActionData)data).Action;
            if (layerChange != null)
            {
                windowText.Caption = layerChange.TransparencyState;
                useSystemDuration.Checked = layerChange.UseSystemLayerTransitionTime;
            }
        }

        void capture_MouseButtonClick(Widget source, EventArgs e)
        {
            layerChange.capture();
            windowText.Caption = layerChange.TransparencyState;
        }

        void useSystemDuration_CheckedChanged(Widget source, EventArgs e)
        {
            layerChange.UseSystemLayerTransitionTime = useSystemDuration.Checked;
        }
    }
}
