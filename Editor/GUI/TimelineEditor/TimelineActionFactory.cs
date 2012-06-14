using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.GUI;
using Engine;

namespace Medical
{
    public class TimelineActionFactoryData : IDisposable
    {
        public TimelineActionFactoryData(Type type, Color color, TimelineDataPanel dataPanel)
        {
            this.Color = color;
            this.Type = type;
            this.Panel = dataPanel;

            try
            {
                TimelineActionProperties properties = (TimelineActionProperties)(type.GetCustomAttributes(typeof(TimelineActionProperties), false)[0]);
                TypeName = properties.TypeName;
            }
            catch (Exception)
            {
                throw new Exception("All TimelineActions added to the factory must have a TimelineActionProperties attribute.");
            }
        }

        public void Dispose()
        {
            if (Panel != null)
            {
                Panel.Dispose();
            }
        }

        public Color Color { get; private set; }

        public Type Type { get; private set; }

        public String TypeName { get; private set; }

        public TimelineDataPanel Panel { get; private set; }
    }

    public class TimelineActionFactory : IDisposable
    {
        private Dictionary<String, TimelineActionFactoryData> actions = new Dictionary<String, TimelineActionFactoryData>();

        public TimelineActionFactory(Widget parentWidget, EditorPlugin editorGUI)
        {
            addType(new TimelineActionFactoryData(typeof(ChangeMedicalStateAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(HighlightTeethAction), new Color(247 / 255f, 150 / 255f, 70 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(LayerChangeAction), new Color(155 / 255f, 187 / 255f, 89 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(MoveCameraAction), new Color(192 / 255f, 80 / 255f, 77 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(PlaySequenceAction), new Color(31 / 255f, 73 / 255f, 125 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(MusclePositionAction), new Color(255 / 255f, 0 / 255f, 0 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(ShowImageAction), new Color(31 / 255f, 73 / 255f, 125 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(ShowTextAction), new Color(31 / 255f, 255 / 255f, 125 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(PlaySoundAction), new Color(0 / 255f, 0 / 255f, 0 / 255f), null));
            addType(new TimelineActionFactoryData(typeof(ShowPropAction), new Color(128 / 255f, 0 / 255f, 255 / 255f), new ShowPropProperties(parentWidget, editorGUI.PropEditController)));
        }

        public void Dispose()
        {
            foreach (TimelineActionFactoryData factoryData in ActionProperties)
            {
                factoryData.Dispose();
            }
            actions.Clear();
        }

        public void addType(TimelineActionFactoryData data)
        {
            this.actions.Add(data.TypeName, data);
        }

        public TimelineAction createAction(String typeName)
        {
            return (TimelineAction)Activator.CreateInstance(actions[typeName].Type);
        }

        public IEnumerable<TimelineActionFactoryData> ActionProperties
        {
            get
            {
                return actions.Values;
            }
        }
    }
}
