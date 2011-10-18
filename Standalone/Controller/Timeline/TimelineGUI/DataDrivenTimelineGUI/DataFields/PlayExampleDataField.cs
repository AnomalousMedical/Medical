using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    class PlayExampleDataField : DataField
    {
        private String timeline;

        public PlayExampleDataField(String name)
            :base(name)
        {

        }

        public override DataControl createControl(Widget parentWidget, DataDrivenTimelineGUI gui)
        {
            return new PlayExampleButton(parentWidget, gui, this);
        }

        [Editable]
        public String Timeline
        {
            get
            {
                return timeline;
            }
            set
            {
                timeline = value;
            }
        }

        public override string Type
        {
            get { return "Play Example"; }
        }

        protected PlayExampleDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
