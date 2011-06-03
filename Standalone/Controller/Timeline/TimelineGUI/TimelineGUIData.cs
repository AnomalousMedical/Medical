using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    public interface TimelineGUIData : Saveable
    {
        EditInterface getEditInterface();

        TimelineGUIData createCopy();
    }
}
