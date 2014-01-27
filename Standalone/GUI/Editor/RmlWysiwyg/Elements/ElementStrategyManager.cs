using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ElementStrategyManager
    {
        private static ElementStrategyManager defaultElementStrategyManager = new ElementStrategyManager();

        static ElementStrategyManager()
        {
            //Headers
            defaultElementStrategyManager.add(new TextElementStrategy("h1"));
            defaultElementStrategyManager.add(new TextElementStrategy("h2"));
            defaultElementStrategyManager.add(new TextElementStrategy("h3"));
            defaultElementStrategyManager.add(new TextElementStrategy("h4"));
            defaultElementStrategyManager.add(new TextElementStrategy("h5"));
            defaultElementStrategyManager.add(new TextElementStrategy("h6"));

            //Text
            defaultElementStrategyManager.add(new TextElementStrategy("p", "Editor/ParagraphsIcon"));
            defaultElementStrategyManager.add(new TextElementStrategy("a"));
            defaultElementStrategyManager.add(new ImageStrategy("img"));
            defaultElementStrategyManager.add(new DivStrategy("x-separator"));

            //Controls
            defaultElementStrategyManager.add(new InputStrategy("input"));
        }

        private Dictionary<String, ElementStrategy> strategies = new Dictionary<string, ElementStrategy>();
        private ElementStrategy defaultStrategy = new ElementStrategy("default");

        public ElementStrategyManager()
        {

        }

        public void add(ElementStrategy strategy)
        {
            strategies.Add(strategy.TagName, strategy);
        }

        public void remove(ElementStrategy strategy)
        {
            strategies.Remove(strategy.TagName);
        }

        public ElementStrategy this[Element element]
        {
            get
            {
                ElementStrategy ret = defaultStrategy;
                if (element != null)
                {
                    String tagName = element.TagName;
                    ElementStrategy strat;
                    if (strategies.TryGetValue(tagName, out strat))
                    {
                        ret = strat;
                    }
                    else if(defaultElementStrategyManager.strategies.TryGetValue(tagName, out strat))
                    {
                        ret = strat;
                    }
                }
                return ret;
            }
        }

        public ElementStrategy DefaultStrategy
        {
            get
            {
                return defaultStrategy;
            }
            set
            {
                defaultStrategy = value;
            }
        }
    }
}
