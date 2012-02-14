using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Medical;
using Engine.Reflection;
using Engine.Saving;

namespace Medical
{
    public partial class CloseGUIPlayTimelineField : DataField
    {
        private String timeline;

        public CloseGUIPlayTimelineField(String name)
            :base(name)
        {

        }

        public CloseGUIPlayTimelineField(String name, String timeline)
            : this(name)
        {
            this.Timeline = timeline;
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

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
            get { return "Close GUI Play Timeline"; }
        }

        protected CloseGUIPlayTimelineField(LoadInfo info)
            :base(info)
        {

        }
    }

    public partial class CloseGUIPlayTimelineField
    {
        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new TimelineEditableProperty("Timeline", new PropertyMemberWrapper(this.GetType().GetProperty("Timeline")), this));
        }
    }
}
