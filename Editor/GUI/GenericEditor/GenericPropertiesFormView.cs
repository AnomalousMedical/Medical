using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Editing;
using Engine.Saving;

namespace Medical.GUI
{
    class GenericPropertiesFormView : MyGUIView
    {
        public GenericPropertiesFormView(String name, EditInterface editInterface, bool horizontalAlignment = false)
            : base(name)
        {
            this.EditInterface = editInterface;
            this.HorizontalAlignment = horizontalAlignment;
            this.ViewLocation = Controller.AnomalousMvc.ViewLocations.Right;
        }

        public EditInterface EditInterface { get; set; }

        public bool HorizontalAlignment { get; set; }

        protected GenericPropertiesFormView(LoadInfo info)
            : base(info)
        {

        }
    }
}
