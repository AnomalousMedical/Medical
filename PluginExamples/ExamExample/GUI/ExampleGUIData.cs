using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.Editing;
using Engine.Saving;

namespace ExamExample.GUI
{
    /// <summary>
    /// This class provides data for the ExampleGUI. This data can be modified
    /// using the TimelineEditor and will be used by ExampleGUI as needed.
    /// </summary>
    class ExampleGUIData : AbstractTimelineGUIData
    {
        /// <summary>
        /// Normal constructor.
        /// </summary>
        public ExampleGUIData()
        {

        }

        [Editable]
        public String SecondTimeline { get; set; }

        /// <summary>
        /// Deserialize constructor. Must be present, must be protected and must
        /// call the base class constructor with load info.
        /// </summary>
        /// <param name="info"></param>
        protected ExampleGUIData(LoadInfo info)
            : base(info)
        {

        }
    }
}
