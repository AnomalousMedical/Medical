using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    class OpenPropManagerView : MyGUIView
    {
        public event Action<OpenPropManagerView, OpenPropManager> ComponentCreated;

        public OpenPropManagerView(String name)
            : base(name)
        {
            IsWindow = true;
            ViewLocation = ViewLocations.Floating;
        }

        internal void _fireComponentCreated(OpenPropManager propManager)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, propManager);
            }
        }

        public OpenPropManagerView(LoadInfo info)
            :base(info)
        {

        }
    }
}
