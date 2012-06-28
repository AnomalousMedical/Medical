using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Medical.Muscles;

namespace Medical.GUI
{
    public class MovementSequenceEditorView : MyGUIView
    {
        public MovementSequenceEditorView(String name, MovementSequence sequence = null, bool listenForSequenceChanges = false)
            : base(name)
        {
            ViewLocation = Controller.AnomalousMvc.ViewLocations.Bottom;
            IsWindow = true;
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
