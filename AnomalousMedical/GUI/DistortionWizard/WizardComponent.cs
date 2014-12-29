using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class WizardComponent<WizardViewType> : LayoutComponent
        where WizardViewType : WizardView
    {
        protected WizardViewType wizardView;
        protected AnomalousMvcContext context;

        public WizardComponent(String layoutFile, WizardViewType view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            :base(layoutFile, viewHost)
        {
            this.wizardView = view;
            this.context = context;
        }
    }
}
