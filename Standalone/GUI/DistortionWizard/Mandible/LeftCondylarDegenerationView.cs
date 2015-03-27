using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Medical.Editor;

namespace Medical.GUI.AnomalousMvc
{
    class LeftCondylarDegenerationView : WizardView
    {
        public LeftCondylarDegenerationView(String name)
            : base(name)
        {

        }

        public override ViewHostComponent createViewHost(AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return new LeftCondylarDegenerationGUI(this, context, viewHost);
        }

        [EditableAction]
        public String NormalAction { get; set; }

        [EditableAction]
        public String ShowOsteophyteAction { get; set; }

        protected LeftCondylarDegenerationView(LoadInfo info)
            :base(info)
        {

        }
    }
}
