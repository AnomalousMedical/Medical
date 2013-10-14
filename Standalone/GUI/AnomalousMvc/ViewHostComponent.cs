using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    public interface ViewHostComponent : IDisposable
    {
        void topLevelResized();

        void animatedResizeStarted(IntSize2 finalSize);

        void animatedResizeCompleted();

        void opening();

        void closing();

        void populateViewData(IDataProvider dataProvider);

        void analyzeViewData(IDataProvider dataProvider);

        MyGUIViewHost ViewHost { get; }

        Widget Widget { get; }

        ViewHostControl findControl(string name);
    }
}
