using Engine;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface SlideInstanceLayoutStrategy
    {
        void addView(MyGUIView view);
    }
}
