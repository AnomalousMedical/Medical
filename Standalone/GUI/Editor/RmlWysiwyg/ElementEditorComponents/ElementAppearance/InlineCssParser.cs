using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class InlineCssParser
    {
        public const String CSSGroups = @"(?<name>[^}:]+):?(?<value>[^};]+);?";
        private static readonly Regex rStyles = new Regex(CSSGroups, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private CssRule rule = new CssRule();

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
                        rule[nameGroup.Value] = valueGroup.Value;
                    }
                }
            }
        }

        public int? intValue(String key)
        {
            return rule.intValue(key);
        }

        public Color? colorValue(String key)
        {
            return rule.colorValue(key);
        }

        public bool isValuePercent(String key)
        {
            return rule.isValuePercent(key);
        }

        public String this[String key]
        {
            get
            {
                return rule[key];
            }
        }

        public bool contains(String key)
        {
            return rule.contains(key);
        }
    }
}
