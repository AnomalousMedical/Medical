using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class CssRule
    {
        private Dictionary<String, String> properties = new Dictionary<string, string>();

        public CssRule()
        {

        }

        public CssRule(String name)
        {
            this.Name = name;
        }

        public String Name { get; set; }

        public String this[String propName]
        {
            get
            {
                String value;
                tryGetValue(propName, out value);
                return value;
            }
            set
            {
                String lowerName = propName.ToLowerInvariant();
                if (properties.ContainsKey(lowerName))
                {
                    properties[lowerName] = value.ToLowerInvariant();
                }
                else
                {
                    properties.Add(lowerName, value.ToLowerInvariant());
                }
            }
        }

        public bool tryGetValue(String propName, out String value)
        {
            return properties.TryGetValue(propName.ToLowerInvariant(), out value);
        }

        public int? intValue(String key)
        {
            String value;
            if (tryGetValue(key, out value))
            {
                int intVal;
                if (int.TryParse(Regex.Match(value, @"\d+").Value, out intVal))
                {
                    return intVal;
                }
            }
            return null;
        }

        public Color? colorValue(String key)
        {
            String value;
            if (tryGetValue(key, out value))
            {
                Color colorVal;
                if (Color.TryFromRGBAString(value, out colorVal))
                {
                    return colorVal;
                }
            }
            return null;
        }

        public bool isValuePercent(String key)
        {
            String value;
            if (tryGetValue(key, out value))
            {
                return value.EndsWith("%");
            }
            return false;
        }

        public bool contains(String key)
        {
            return properties.ContainsKey(key.ToLowerInvariant());
        }

        public IEnumerable<String> Keys
        {
            get
            {
                return properties.Keys;
            }
        }

        public IEnumerable<String> Values
        {
            get
            {
                return properties.Values;
            }
        }
    }
}
