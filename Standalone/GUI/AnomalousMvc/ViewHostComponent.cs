using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class ViewHostComponent : IDisposable
    {
        public ViewHostComponent(MyGUIViewHost viewHost)
        {
            this.ViewHost = viewHost;
        }

        public virtual void Dispose()
        {

        }

        public virtual void topLevelResized()
        {

        }

        public virtual void opening()
        {

        }

        public virtual void closing()
        {

        }

        public MyGUIViewHost ViewHost { get; private set; }

        public Widget Widget { get; set; }
    }
}
