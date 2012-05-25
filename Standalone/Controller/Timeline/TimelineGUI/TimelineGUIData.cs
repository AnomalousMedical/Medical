using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;

namespace Medical
{
    public interface TimelineGUIData : Saveable
    {
        EditInterface getEditInterface();

        TimelineGUIData createCopy();

        void convertToMvc(AnomalousMvcContext context, StringBuilder rmlStringBuilder, MvcController controller, RmlView view);
    }
}
