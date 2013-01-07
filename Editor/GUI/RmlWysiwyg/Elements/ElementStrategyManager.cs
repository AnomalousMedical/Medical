using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ElementStrategyManager
    {
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
                    ElementStrategy strat;
                    if (strategies.TryGetValue(element.TagName, out strat))
                    {
                        ret = strat;
                    }
                }
                return ret;
            }
        }
    }
}
