using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class MyGUIViewHostFactory : ViewHostFactory
    {
        DecoratorComponentFactory componentFactory = new DecoratorComponentFactory();

        public MyGUIViewHostFactory()
        {
            componentFactory.addFactory(new RmlViewHostFactory());
            componentFactory.addFactory(new NavigationViewHostFactory());
            componentFactory.addFactory(new DistortionWizardViewHostFactory());
        }

        public ViewHost createViewHost(View view, AnomalousMvcContext context)
        {
            MyGUIViewHost viewHost = new MyGUIViewHost();
            viewHost.setTopComponent(componentFactory.createViewHostComponent(view, context, viewHost));
            return viewHost;
        }

        public void createViewBrowser(Browser browser)
        {
            componentFactory.createViewBrowser(browser);
        }
    }
}
