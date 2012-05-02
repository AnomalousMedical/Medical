using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Medical.RmlTimeline.Actions;
using Engine.Reflection;

namespace Medical
{
    public class RmlTimelineGUIData : AbstractTimelineGUIData
    {
        RmlGuiActionManager actionManager;

        public RmlTimelineGUIData()
        {
            actionManager = new RmlGuiActionManager();
        }

        public String RmlFile { get; set; }

        public RmlGuiActionManager ActionManager
        {
            get
            {
                return actionManager;
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new TimelineEditableProperty("RmlFile", new PropertyMemberWrapper(this.GetType().GetProperty("RmlFile")), this, TimelineBrowserController.RmlSearchPattern));
            editInterface.addSubInterface(actionManager.getEditInterface());
        }

        protected RmlTimelineGUIData(LoadInfo info)
            : base(info)
        {
            
        }
    }
}
