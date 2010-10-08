using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class TimelineActionFactory
    {
        private static Dictionary<TimelineActionProperties, Type> actions = new Dictionary<TimelineActionProperties, Type>();

        static TimelineActionFactory()
        {
            addType(typeof(ChangeMedicalStateAction));
            addType(typeof(HighlightTeethAction));
            addType(typeof(LayerChangeAction));
            addType(typeof(MoveCameraAction));
            addType(typeof(PlaySequenceAction));
        }

        private static void addType(Type type)
        {
            try
            {
                TimelineActionProperties properties = (TimelineActionProperties)(type.GetCustomAttributes(typeof(TimelineActionProperties), false)[0]);
                actions.Add(properties, type);
            }
            catch (Exception)
            {
                throw new Exception("All TimelineActions added to the factory must have a TimelineActionProperties attribute.");
            }
        }

        public static TimelineAction createAction(TimelineActionProperties key)
        {
            return (TimelineAction)Activator.CreateInstance(actions[key]);
        }

        public static IEnumerable<TimelineActionProperties> ActionProperties
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
