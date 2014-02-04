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
    public class MovementSequenceEditorView : MyGUIView
    {
        public MovementSequenceEditorView(String name, MovementSequence sequence = null, bool listenForSequenceChanges = false)
            : base(name)
        {
            ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Bottom)
            {
                AllowedDockLocations = DockLocation.Top | DockLocation.Bottom | DockLocation.Floating
            };
            Sequence = sequence;
            ListenForSequenceChanges = listenForSequenceChanges;
        }

        public MovementSequence Sequence { get; set; }

        public bool ListenForSequenceChanges { get; set; }

        protected MovementSequenceEditorView(LoadInfo info)
            :base(info)
        {

        }
    }
}
