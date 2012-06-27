using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;
using Medical.Controller;

namespace Medical.GUI.AnomalousMvc
{
    public class MyGUIViewHostFactory : ViewHostFactory
    {
        DecoratorComponentFactory componentFactory;

        public MyGUIViewHostFactory(MDILayoutManager mdiManager)
        {
            componentFactory = new DecoratorComponentFactory(mdiManager);
            componentFactory.addFactory(new RmlComponentFactory());
            componentFactory.addFactory(new NavigationComponentFactory());
            componentFactory.addFactory(new WizardComponentFactory());
        }

        public ViewHost createViewHost(View view, AnomalousMvcContext context)
        {
            MyGUIView myGUIView = view as MyGUIView;
            if (myGUIView != null)
            {
                MyGUIViewHost viewHost = new MyGUIViewHost(context, view);
                viewHost.setTopComponent(componentFactory.createViewHostComponent(myGUIView, context, viewHost));
                return viewHost;
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            componentFactory.createViewBrowser(browser);
        }

        public void addFactory(ViewHostComponentFactory factory)
        {
            componentFactory.addFactory(factory);
        }
    }
}
