using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class DistortionWizardViewHostFactory : ViewHostFactory
    {
        public ViewHost createViewHost(View view, AnomalousMvcContext context)
        {
            if (typeof(WizardView).IsAssignableFrom(view.GetType()))
            {
                WizardView wizardView = (WizardView)view;
                return wizardView.createViewHost(context);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            BrowserNode wizardNode = new BrowserNode("Wizard Views", null);
            wizardNode.addChild(new BrowserNode("Disclaimer", typeof(DisclaimerView)));
            browser.addNode("", null, wizardNode);
        }
    }
}
