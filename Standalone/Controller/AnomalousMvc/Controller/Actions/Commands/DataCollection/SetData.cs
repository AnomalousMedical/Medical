using Engine.Editing;
using Engine.Saving;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class SetData : ActionCommand
    {
        public SetData()
        {
            DataModelName = DataModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            DataModel dataModel = context.getModel<DataModel>(DataModelName);
            if (dataModel != null)
            {
                dataModel.setValue(FieldName, Value);
            }
            else
            {
                Log.Error("Could not find a data model named {0}", DataModelName);
            }
        }

        [Editable]
        public String DataModelName { get; set; }

        [Editable]
        public String FieldName { get; set; }

        [Editable]
        public String Value { get; set; }

        public override string Type
        {
            get
            {
                return "Set Data";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected SetData(LoadInfo info)
            :base(info)
        {

        }
    }
}
