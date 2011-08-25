using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical
{
    public class DataDrivenExamSection : Saveable
    {
        private String prettyName;
        private EditInterface editInterface;
        private Dictionary<String, Object> values = new Dictionary<String, Object>();
        private Dictionary<String, DataDrivenExamSection> sections = new Dictionary<String, DataDrivenExamSection>();

        public DataDrivenExamSection(String prettyName)
        {
            this.prettyName = prettyName;
        }

        public T getValue<T>(String key, T defaultValue)
        {
            Object value;
            if (values.TryGetValue(key, out value) && value is T)
            {
                return (T)value;
            }
            return defaultValue;
        }

        public void setValue<T>(String key, T value)
        {
            if (values.ContainsKey(key))
            {
                values[key] = value;
            }
            else
            {
                values.Add(key, value);
            }
        }

        public DataDrivenExamSection getSection(String name)
        {
            DataDrivenExamSection section;
            if (!sections.TryGetValue(name, out section))
            {
                section = new DataDrivenExamSection(name);
                sections.Add(name, section);
            }
            return section;
        }

        public string PrettyName
        {
            get
            {
                return prettyName;
            }
        }

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = new EditInterface(prettyName);
                    EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
                    propertyInfo.addColumn(new EditablePropertyColumn("Name", true));
                    propertyInfo.addColumn(new EditablePropertyColumn("Value", true));
                    editInterface.setPropertyInfo(propertyInfo);

                    foreach (DataDrivenExamSection section in sections.Values)
                    {
                        editInterface.addSubInterface(section.EditInterface);
                    }

                    foreach (String key in values.Keys)
                    {
                        editInterface.addEditableProperty(new DataDrivenExamValueProperty(key, values[key]));
                    }
                }
                return editInterface;
            }
        }

        protected DataDrivenExamSection(LoadInfo info)
        {
            prettyName = info.GetValue("PrettyName", prettyName);
            info.RebuildDictionary<String, DataDrivenExamSection>("ExamSection", sections);

            String keyBase = "ExamValueKey";
            String valueBase = "ExamValueValue";
            for (int i = 0; info.hasValue(keyBase + i); ++i)
            {
                values.Add(info.GetString(keyBase + i), info.GetValue<Object>(valueBase + i));
            }
        }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue("PrettyName", prettyName);
            info.ExtractDictionary<String, DataDrivenExamSection>("ExamSection", sections);

            String keyBase = "ExamValueKey";
            String valueBase = "ExamValueValue";
            int i = 0;
            Type keyT = typeof(String);
            foreach (String key in values.Keys)
            {
                Object value = values[key];
                Type valueT = value.GetType();
                info.AddValue(keyBase + i, key);
                if (valueT == typeof(decimal))
                {
                    info.AddValue(valueBase + i, (decimal)value);
                }
                else if (valueT == typeof(bool))
                {
                    info.AddValue(valueBase + i, (bool)value);
                }
                else if (valueT == typeof(String))
                {
                    info.AddValue(valueBase + i, (String)value);
                }
                else
                {
                    Log.Warning("Could not serialize DataDrivenExam value '{0}' of type '{1}' because it is not a supported serialization type. Value skipped.", key, valueT.FullName);
                }
                ++i;
            }
        }
    }
}
