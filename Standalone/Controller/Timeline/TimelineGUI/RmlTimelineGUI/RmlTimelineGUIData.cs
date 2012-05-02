using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    class RmlTimelineGUIData : AbstractTimelineGUIData
    {
        public RmlTimelineGUIData()
        {

        }

        [Editable]
        public String RmlFile { get; set; }

        protected RmlTimelineGUIData(LoadInfo info)
            : base(info)
        {

        }
    }
}
