﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class RmlComponentFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is RmlView)
            {
                return new RmlWidgetComponent((RmlView)view, context, viewHost);
            }
            else if (view is RawRmlView)
            {
                return new RmlWidgetComponent((RawRmlView)view, context, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            BrowserNode rmlNode = new GenericBrowserNode<ViewCollection.CreateView>("Rml View", name =>
            {
                return new RmlView(name);
            });
            browser.addNode(null, null, rmlNode);

            rmlNode = new GenericBrowserNode<ViewCollection.CreateView>("Closing Rml View", name =>
            {
                RmlView rmlView = new RmlView(name);
                rmlView.Buttons.add(new CloseButtonDefinition("Close", name + "/Close"));
                return rmlView;
            });
            browser.addNode(null, null, rmlNode);
            browser.DefaultSelection = rmlNode;
        }
    }
}
