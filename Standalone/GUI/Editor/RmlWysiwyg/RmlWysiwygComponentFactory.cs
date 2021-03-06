﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;
using Medical.GUI.AnomalousMvc;

namespace Medical.GUI
{
    class RmlWysiwygComponentFactory : ViewHostComponentFactory
    {
        public RmlWysiwygComponentFactory()
        {
            
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is RmlWysiwygView)
            {
                return new RmlWysiwygComponent((RmlWysiwygView)view, context, viewHost);
            }
            if (view is RawRmlWysiwygView)
            {
                return new RmlWysiwygComponent((RawRmlWysiwygView)view, context, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
