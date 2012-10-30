using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using libRocketPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public class RmlViewHostControl : ViewHostControl
    {
        private Element element;
        private RocketWidget rocketWidget;

        public RmlViewHostControl(Element element, RocketWidget rocketWidget)
        {
            this.element = element;
            this.rocketWidget = rocketWidget;
        }

        public void focus()
        {
            element.Focus();
            rocketWidget.setFocus();
        }

        public void blur()
        {
            element.Blur();
        }

        public string Value
        {
            get
            {
                if (element.HasAttribute("value"))
                {
                    return element.GetAttributeString("value");
                }
                return element.InnerRml;
            }
            set
            {
                if (element.HasAttribute("value"))
                {
                    element.SetAttribute("value", value);
                }
                else
                {
                    element.InnerRml = value;
                }
            }
        }
    }
}
