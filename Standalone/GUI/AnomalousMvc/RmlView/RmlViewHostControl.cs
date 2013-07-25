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
        private RmlWidgetComponent rmlWidgetComponent;

        public RmlViewHostControl(Element element, RmlWidgetComponent rmlWidgetComponent, RocketWidget rocketWidget)
        {
            this.element = element;
            this.rocketWidget = rocketWidget;
            this.rmlWidgetComponent = rmlWidgetComponent;
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

        public bool Visible
        {
            get
            {
                String display = element.GetPropertyString("display");
                return display != null && !display.Equals("none", StringComparison.InvariantCultureIgnoreCase);
            }
            set
            {
                element.SetProperty("display", value ? "block" : "none"); //Yea... just assuming block
            }
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
                    rmlWidgetComponent.startRmlUpdate();
                    element.InnerRml = value;
                    rmlWidgetComponent.endRmlUpdate();
                }
                rocketWidget.renderOnNextFrame();
            }
        }
    }
}
