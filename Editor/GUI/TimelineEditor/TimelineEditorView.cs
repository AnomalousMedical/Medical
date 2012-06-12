using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    class TimelineEditorView : MyGUIView
    {
        public TimelineEditorView(String name, Timeline timeline)
            :base(name)
        {
            ViewLocation = Controller.AnomalousMvc.ViewLocations.Bottom;
            IsWindow = true;
            this.Timeline = timeline;
        }

        public Timeline Timeline { get; set; }

        protected TimelineEditorView(LoadInfo info)
            :base(info)
        {

        }
    }
}
