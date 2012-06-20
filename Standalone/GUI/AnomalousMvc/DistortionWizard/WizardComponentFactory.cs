using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class WizardComponentFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            WizardView wizardView = view as WizardView;
            if (wizardView != null)
            {
                ViewHostComponent component = wizardView.createViewHost(context, viewHost);
                return component;
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            BrowserNode wizardNode = new BrowserNode("Wizard Views", null);
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Disclaimer", name => { return new DisclaimerView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Left Condylar Growth", name => { return new LeftCondylarGrowthView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Right Condylar Growth", name => { return new RightCondylarGrowthView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Left Condylar Degeneration", name => { return new LeftCondylarDegenerationView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Right Condylar Degeneration", name => { return new RightCondylarDegenerationView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Disc Space", name => { return new DiscSpaceView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Doppler", name => { return new DopplerView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Left Fossa", name => { return new LeftFossaView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Right Fossa", name => { return new RightFossaView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Preset State", name => { return new PresetStateView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Profile Distortion", name => { return new ProfileDistortionView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Remove Bottom Teeth", name => { return new RemoveBottomTeethView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Remove Top Teeth", name => { return new RemoveTopTeethView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Teeth Adaptation", name => { return new TeethAdaptationView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Teeth Height Adaptation", name => { return new TeethHeightAdaptationView(name); }));
            wizardNode.addChild(new GenericBrowserNode<ViewCollection.CreateView>("Notes", name => { return new NotesView(name); }));
            browser.addNode("", null, wizardNode);
        }
    }
}
