using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public interface ViewHostFactory
    {
        ViewHost createViewHost(View view, AnomalousMvcContext context);

        void createViewBrowser(Browser browser);
    }
}
