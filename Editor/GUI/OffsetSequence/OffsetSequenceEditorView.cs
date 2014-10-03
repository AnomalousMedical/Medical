using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Medical.Muscles;
using Medical.Controller;

namespace Medical.GUI
{
    public class OffsetSequenceEditorView : MyGUIView
    {
        public OffsetSequenceEditorView(String name, OffsetModifierSequence sequence = null)
            : base(name)
        {
            ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Bottom)
            {
                AllowedDockLocations = DockLocation.Top | DockLocation.Bottom | DockLocation.Floating
            };
            Sequence = sequence;
        }

        public OffsetModifierSequence Sequence { get; set; }

        protected OffsetSequenceEditorView(LoadInfo info)
            :base(info)
        {

        }
    }
}
