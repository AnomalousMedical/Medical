using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    public class RawRmlView : MyGUIView
    {
        public event Action<RawRmlView, RmlWidgetComponent> ComponentCreated;

        public RawRmlView(String name)
            : base(name)
        {
            Rml = null;
        }

        [Editable]
        public String Rml { get; set; }

        internal void _fireComponentCreated(RmlWidgetComponent component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        public RawRmlView(LoadInfo info)
            : base(info)
        {
            
        }
    }
}
