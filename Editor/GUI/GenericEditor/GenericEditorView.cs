using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI
{
    class GenericEditorView : MyGUIView
    {
        public GenericEditorView(String name, EditInterface editInterface, bool horizontalAlignment = false)
            : base(name)
        {
            this.EditInterface = editInterface;
            this.HorizontalAlignment = horizontalAlignment;
            this.ViewLocation = Controller.AnomalousMvc.ViewLocations.Right;
        }

        public EditInterface EditInterface { get; set; }

        public bool HorizontalAlignment { get; set; }

        protected GenericEditorView(LoadInfo info)
            :base(info)
        {

        }
    }
}
