using Engine.Editing;
using Engine.Saving;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class SaveTeethHighlightsCommand : ActionCommand
    {
        public SaveTeethHighlightsCommand()
        {
            Name = "DefaultTeethHighlights";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                TeethHighlightState saved = context.getModel<TeethHighlightState>(Name);
                if (saved == null)
                {
                    saved = new TeethHighlightState();
                    context.addModel(Name, saved);
                }
                saved.Highlighted = TeethController.HighlightContacts;
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
                return "Save Teeth Highlights";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected SaveTeethHighlightsCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
