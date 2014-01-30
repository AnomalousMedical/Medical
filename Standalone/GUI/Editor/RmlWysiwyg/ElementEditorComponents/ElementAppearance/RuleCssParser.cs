using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    /// <summary>
    /// This parser can parse simple css that is just a series of rules with properties, no media queries or anything fancy though.
    /// This is simple enough and works for the kinds of css we generate with our tools.
    /// 
    /// It is based on the code project article.
    /// http://www.codeproject.com/Articles/335850/CSSParser
    /// </summary>
    public class RuleCssParser
    {
        private const String SelectorKey = "selector";
        private const String NameKey = "name";
        private const String ValueKey = "value";

        private const String CSSGroups = @"(?<selector>(?:(?:[^,{]+),?)*?)\{(?:(?<name>[^}:]+):?(?<value>[^};]+);?)*?\}";
        private const String CSSComments = @"(?<!"")\/\*.+?\*\/(?!"")";
        private Regex rStyles = new Regex(CSSGroups, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private Dictionary<String, CssRule> rules = new Dictionary<string, CssRule>();

        public RuleCssParser(String ruleCss)
        {
            if (!String.IsNullOrEmpty(ruleCss))
            {
                MatchCollection MatchList = rStyles.Matches(ruleCss);
                foreach (Match item in MatchList)
                {
                    //Check for nulls
                    if (item != null && item.Groups != null &&
                        item.Groups[SelectorKey] != null &&
                        item.Groups[SelectorKey].Captures != null &&
                        item.Groups[SelectorKey].Captures[0] != null &&
                        !String.IsNullOrEmpty(item.Groups[SelectorKey].Value))
                    {
                        CssRule rule = new CssRule(item.Groups[SelectorKey].Captures[0].Value.Trim());
                        for (int i = 0; i < item.Groups[NameKey].Captures.Count; i++)
                        {
                            String className = item.Groups[NameKey].Captures[i].Value;
                            String value = item.Groups[ValueKey].Captures[i].Value;
                            //Check for null values in the properies
                            if (!String.IsNullOrEmpty(className) && !String.IsNullOrEmpty(value))
                            {
                                className = className.Trim();
                                value = value.Trim();
                                //One more check to be sure we are only pulling valid css values
                                if (!String.IsNullOrEmpty(className) && !String.IsNullOrEmpty(value) && value != ":")
                                {
                                    rule[className] = value;
                                }
                            }
                        }
                        rules.Add(rule.Name.ToLowerInvariant(), rule);
                    }
                }
            }
        }

        public CssRule this[String ruleName]
        {
            get
            {
                CssRule rule = null;
                rules.TryGetValue(ruleName.ToLowerInvariant(), out rule);
                return rule;
            }
        }

        public IEnumerable<CssRule> Rules
        {
            get
            {
                return rules.Values;
            }
        }
    }
}
