using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Medical.RmlTimeline.Actions;

namespace Medical
{
    public class RmlTimelineGUIData : AbstractTimelineGUIData
    {
        RmlGuiActionManager actionManager;

        public RmlTimelineGUIData()
        {
            actionManager = new RmlGuiActionManager();
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(actionManager.getEditInterface());
        }

        [Editable]
        public String RmlFile { get; set; }

        public RmlGuiActionManager ActionManager
        {
            get
            {
                return actionManager;
            }
        }

        protected RmlTimelineGUIData(LoadInfo info)
            : base(info)
        {
            
        }
    }
}
