using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class TimelineActionFactory
    {
        private static Dictionary<String, Type> actions = new Dictionary<String, Type>();

        static TimelineActionFactory()
        {
            actions.Add(ChangeMedicalStateAction.Name, typeof(ChangeMedicalStateAction));
            actions.Add(HighlightTeethAction.Name, typeof(HighlightTeethAction));
            actions.Add(LayerChangeAction.Name, typeof(LayerChangeAction));
            actions.Add(MoveCameraAction.Name, typeof(MoveCameraAction));
            actions.Add(PlaySequenceAction.Name, typeof(PlaySequenceAction));
        }

        public static IEnumerable<String> ActionNames
        {
            get
            {
                return actions.Keys;
            }
        }

        private TimelineActionFactory()
        {

        }
    }
}
