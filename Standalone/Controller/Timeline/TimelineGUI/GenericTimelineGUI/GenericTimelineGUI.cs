using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Medical.GUI;

namespace Medical
{
    /// <summary>
    /// This class provides a way of having a timeline GUI that is created in
    /// its prototype and destroyed when it is finished. It can also play other
    /// timelines and close itself.
    /// </summary>
    /// <typeparam name="TimelineGUIDataType">The type of the TimelineGUIData associated with this class. It can be TimelineGUIData if no type is defined.</typeparam>
    public class GenericTimelineGUI<TimelineGUIDataType> : AbstractTimelineGUI
        where TimelineGUIDataType : TimelineGUIData
    {
        private TimelineGUIDataType guiData;

        public GenericTimelineGUI(String layoutFile)
            : base(layoutFile)
        {

        }

        public override void initialize(ShowTimelineGUIAction showTimelineAction)
        {
            guiData = (TimelineGUIDataType)showTimelineAction.GUIData;
            base.initialize(showTimelineAction);
        }

        public TimelineGUIDataType GUIData
        {
            get
            {
                return guiData;
            }
        }
    }
}
