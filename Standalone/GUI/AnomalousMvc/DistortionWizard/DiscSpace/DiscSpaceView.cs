using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class DiscSpaceView : WizardView
    {
        public DiscSpaceView(String name)
            :base(name)
        {
            
        }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new DiscSpaceGUI(StateDirectory, this, context);
        }

        [Editable]
        public String StateDirectory { get; set; }

        [EditableAction]
        public String NormalViewAction { get; set; }

        [EditableAction]
        public String ShowDiscAction { get; set; }

        protected DiscSpaceView(LoadInfo info)
            :base(info)
        {

        }
    }
}
