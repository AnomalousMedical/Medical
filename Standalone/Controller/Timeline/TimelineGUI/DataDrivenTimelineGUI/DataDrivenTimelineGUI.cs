using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine;

namespace Medical
{
    class DataDrivenTimelineGUI : GenericTimelineGUI<DataDrivenTimelineGUIData>
    {
        private DataControl topLevelDataControl;

        public DataDrivenTimelineGUI()
            :base("Medical.Controller.Timeline.TimelineGUI.DataDrivenTimelineGUI.DataDrivenTimelineGUI.layout")
        {
            
        }

        protected override void onShown()
        {
            topLevelDataControl = GUIData.createControls(widget);
            topLevelDataControl.WorkingSize = new Size2(widget.Width, widget.Height);
            topLevelDataControl.Location = new Vector2(0, 0);
            topLevelDataControl.layout();
        }
    }
}
