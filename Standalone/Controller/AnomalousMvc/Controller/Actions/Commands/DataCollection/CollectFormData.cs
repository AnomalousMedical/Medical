using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    public class CollectFormData : ActionCommand
    {
        public CollectFormData()
        {
            DataModelName = DataModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            DataModel dataModel = context.getModel<DataModel>(DataModelName);
            if (dataModel != null)
            {
                Log.Debug("-----------------------------------");
                foreach (var param in context.ActionArguments)
                {
                    Log.Debug("key {0} | value {1}", param.Item1, param.Item2);
                    dataModel.trySetValue(param.Item1, param.Item2);
                }
                Log.Debug("-----------------------------------");
            }
            else
            {
                Log.Error("Could not find a data model named {0}", DataModelName);
            }
        }

        [Editable]
        public String DataModelName { get; set; }

        public override string Type
        {
            get
            {
                return "Collect Form Data";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected CollectFormData(LoadInfo info)
            :base(info)
        {

        }
    }
}
