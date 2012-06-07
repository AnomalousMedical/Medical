using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Medical.GUI.AnomalousMvc;

namespace Medical.GUI
{
    class EditorInfoBarView : MyGUIView
    {
        public EditorInfoBarView(String name, String caption, String closeAction = null)
            :base(name)
        {
            this.Caption = caption;
            this.ViewLocation = Controller.AnomalousMvc.ViewLocations.Top;
            this.CloseAction = closeAction;
        }

        public String Caption { get; set; }

        public String CloseAction { get; set; }

        protected EditorInfoBarView(LoadInfo info)
            : base(info)
        {

        }
    }
}
