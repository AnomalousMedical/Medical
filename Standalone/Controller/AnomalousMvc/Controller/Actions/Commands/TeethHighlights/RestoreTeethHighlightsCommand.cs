using Engine.Editing;
using Engine.Saving;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class RestoreTeethHighlightsCommand : ActionCommand
    {
        public RestoreTeethHighlightsCommand()
        {
            Name = "DefaultTeethHighlights";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                TeethHighlightState saved = context.getModel<TeethHighlightState>(Name);
                TeethController.HighlightContacts = saved.Highlighted;
            }
            else
            {
                Log.Warning("No name defined.");
            }
        }

        [Editable]
        public String Name { get; set; }

        public override string Type
        {
            get
            {
                return "Restore Teeth Highlights";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected RestoreTeethHighlightsCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
