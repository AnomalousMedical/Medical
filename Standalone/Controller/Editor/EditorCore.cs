using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;

namespace Medical
{
    class EditorCore
    {
        public EditorCore(MyGUIViewHostFactory viewHostFactory)
        {
            viewHostFactory.addFactory(new RmlWysiwygComponentFactory());
        }
    }
}
