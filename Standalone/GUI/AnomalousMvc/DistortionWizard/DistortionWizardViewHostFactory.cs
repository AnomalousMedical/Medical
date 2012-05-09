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
            wizardNode.addChild(new BrowserNode("Left Condylar Growth", typeof(LeftCondylarGrowthView)));
            wizardNode.addChild(new BrowserNode("Right Condylar Growth", typeof(RightCondylarGrowthView)));
            wizardNode.addChild(new BrowserNode("Left Condylar Degeneration", typeof(LeftCondylarDegenerationView)));
            wizardNode.addChild(new BrowserNode("Right Condylar Degeneration", typeof(RightCondylarDegenerationView)));
            wizardNode.addChild(new BrowserNode("Disc Space", typeof(DiscSpaceView)));
            wizardNode.addChild(new BrowserNode("Doppler", typeof(DopplerView)));
            wizardNode.addChild(new BrowserNode("Left Fossa", typeof(LeftFossaView)));
            wizardNode.addChild(new BrowserNode("Right Fossa", typeof(RightFossaView)));
            browser.addNode("", null, wizardNode);
        }
    }
}
