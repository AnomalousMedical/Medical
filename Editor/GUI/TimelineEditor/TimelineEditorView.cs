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
        public event Action<TimelineEditorView, TimelineEditorComponent> ComponentCreated;

        public TimelineEditorView(String name, Timeline timeline)
            :base(name)
        {
            ViewLocation = Controller.AnomalousMvc.ViewLocations.Bottom;
            IsWindow = true;
            this.Timeline = timeline;
        }

        public Timeline Timeline { get; set; }

        internal void _fireComponentCreated(TimelineEditorComponent component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        protected TimelineEditorView(LoadInfo info)
            :base(info)
        {

        }
    }
}
