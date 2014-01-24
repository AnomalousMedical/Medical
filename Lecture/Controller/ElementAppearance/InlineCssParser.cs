using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lecture
{
    class InlineCssParser
    {
        public const String CSSGroups = @"(?<name>[^}:]+):?(?<value>[^};]+);?";
        private static readonly Regex rStyles = new Regex(CSSGroups, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private Dictionary<String, String> properties = new Dictionary<string, string>();

        public InlineCssParser(String inlineCss)
        {
            if (!String.IsNullOrEmpty(inlineCss))
            {
                MatchCollection MatchList = rStyles.Matches(inlineCss);
                foreach (Match item in MatchList)
                {
                    var nameGroup = item.Groups["name"];
                    var valueGroup = item.Groups["value"];
                    if (nameGroup != null && nameGroup.Value != null && valueGroup != null && valueGroup.Value != null && valueGroup.Value != ":")
                    {
                        String lowerName = nameGroup.Value.ToLowerInvariant();
                        if (properties.ContainsKey(lowerName))
                        {
                            properties[lowerName] = valueGroup.Value.ToLowerInvariant();
                        }
                        else
                        {
                            properties.Add(lowerName, valueGroup.Value.ToLowerInvariant());
                        }
                    }
                }
            }
        }

        public int? intValue(String key)
        {
            int intVal;
            if (int.TryParse(Regex.Match(properties[key], @"\d+").Value, out intVal))
            {
                return intVal;
            }
            return null;
        }

        public bool isValuePercent(String key)
        {
            return properties[key].EndsWith("%");
        }

        public String this[String key]
        {
            get
            {
                return properties[key];
            }
        }

        public bool contains(String key)
        {
            return properties.ContainsKey(key.ToLowerInvariant());
        }
    }
}
