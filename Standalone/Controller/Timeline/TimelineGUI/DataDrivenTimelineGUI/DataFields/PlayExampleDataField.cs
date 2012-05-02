using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;

namespace Medical
{
    public partial class PlayExampleDataField : DataField
    {
        private String timeline;

        public PlayExampleDataField(String name)
            :base(name)
        {

        }

        public PlayExampleDataField(String name, String timeline)
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
            get { return "Play Example"; }
        }

        protected PlayExampleDataField(LoadInfo info)
            :base(info)
        {

        }
    }

    public partial class PlayExampleDataField
    {
        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new TimelineEditableProperty("Timeline", new PropertyMemberWrapper(this.GetType().GetProperty("Timeline")), this, TimelineBrowserController.TimelineSearchPattern));
        }
    }
}
