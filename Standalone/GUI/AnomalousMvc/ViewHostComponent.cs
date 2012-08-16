using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    public interface ViewHostComponent : IDisposable
    {
        void topLevelResized();

        void opening();

        void closing();

        void populateViewData(IDataProvider dataProvider);

        void analyzeViewData(IDataProvider dataProvider);

        MyGUIViewHost ViewHost { get; }

        Widget Widget { get; }
    }
}
