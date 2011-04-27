using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class AnatomyContextWindow : Dialog
    {
        private const int SIDE_PADDING = 10;
        private const float SCROLL_MAX = 10000;
        private const float SCROLL_MAX_RANGE_MIN = SCROLL_MAX - 50;//The minimum postion of a scroll bar to count as the max value.

        private Anatomy anatomy;
        private List<Widget> dynamicWidgets = new List<Widget>();
        private FlowLayoutContainer layoutContainer = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 5.0f, new Vector2(SIDE_PADDING / 2, 0.0f));

        public AnatomyContextWindow()
            :base("Medical.GUI.Anatomy.AnatomyContextWindow.layout")
        {

        }

        public Anatomy Anatomy
        {
            get
            {
                return anatomy;
            }
            set
            {
                layoutContainer.SuppressLayout = true;
                foreach (Widget widget in dynamicWidgets)
                {
                    Gui.Instance.destroyWidget(widget);
                }
                dynamicWidgets.Clear();
                layoutContainer.clearChildren();
                this.anatomy = value;
                window.Caption = anatomy.AnatomicalName;
                foreach (AnatomyCommand command in anatomy.Commands)
                {
                    switch (command.UIType)
                    {
                        case AnatomyCommandUIType.Sliding:
                            HScroll slider = (HScroll)window.createWidgetT("HScroll", "HSlider", 0, 0, window.Width - SIDE_PADDING, 20, Align.Default, "");
                            slider.ScrollChangePosition += new MyGUIEvent(slider_ScrollChangePosition);
                            slider.UserObject = command;
                            slider.ScrollRange = (int)SCROLL_MAX;
                            layoutContainer.addChild(new MyGUILayoutContainer(slider));
                            dynamicWidgets.Add(slider);
                            break;
                        case AnatomyCommandUIType.Button:
                            break;
                        case AnatomyCommandUIType.Toggle:
                            break;
                    }
                }
                layoutContainer.SuppressLayout = false;
                layoutContainer.layout();
            }
        }

        void slider_ScrollChangePosition(Widget source, EventArgs e)
        {
            HScroll scroll = (HScroll)source;
            float normalizedValue = scroll.ScrollPosition / SCROLL_MAX;
            if (scroll.ScrollPosition > SCROLL_MAX_RANGE_MIN)
            {
                normalizedValue = 1.0f;
            }
            AnatomyCommand command = (AnatomyCommand)scroll.UserObject;
            float range = command.NumericValueMax - command.NumericValueMin;
            command.NumericValue = range * normalizedValue + command.NumericValueMin;
        }
    }
}
