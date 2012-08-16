using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    public class AnalyzeFormData : ActionCommand
    {
        public AnalyzeFormData()
        {
            DataModelName = DataModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            DataModel dataModel = context.getModel<DataModel>(DataModelName);
            if (dataModel != null)
            {
                context.analyzeViewData(dataModel);
            }
            else
            {
                Log.Error("Could not find data model named {0}", DataModelName);
            }
        }

        [Editable]
        public String DataModelName { get; set; }

        public override string Type
        {
            get
            {
                return "Analyze Form Data";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected AnalyzeFormData(LoadInfo info)
            :base(info)
        {

        }
    }
}
