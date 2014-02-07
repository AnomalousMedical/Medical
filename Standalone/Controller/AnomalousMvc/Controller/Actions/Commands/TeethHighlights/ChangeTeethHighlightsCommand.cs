using Engine.Editing;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class ChangeTeethHighlightsCommand: ActionCommand
    {
        private bool enabled;

        public ChangeTeethHighlightsCommand()
            :this(false)
        {

        }

        public ChangeTeethHighlightsCommand(bool enabled)
        {
            this.enabled = enabled;
        }

        public override void execute(AnomalousMvcContext context)
        {
            TeethController.HighlightContacts = enabled;
        }

        [Editable]
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        public override string Type
        {
            get
            {
                return "Change Teeth Highlights";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected ChangeTeethHighlightsCommand(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
