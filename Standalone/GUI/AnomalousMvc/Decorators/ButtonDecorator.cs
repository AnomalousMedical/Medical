using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class ButtonDecorator : Component, ViewHostComponent
    {
        private ViewHostComponent child;
        private int widgetHeight;
        private const int BUTTON_AREA_HEIGHT = 36;
        private const int BUTTON_PADDING = 3;

        private List<Button> buttons = new List<Button>();

        public ButtonDecorator(ViewHostComponent child, ButtonCollection buttonDefinitions)
            :base("Medical.GUI.AnomalousMvc.Decorators.ButtonDecorator.layout")
        {
            this.child = child;
            widget.setSize(child.Widget.Width, child.Widget.Height + BUTTON_AREA_HEIGHT);
            child.Widget.attachToWidget(widget);
            child.Widget.setPosition(0, BUTTON_AREA_HEIGHT);
            child.Widget.Align = Align.Stretch;

            widgetHeight = widget.Height;

            int buttonWidth = widget.Width / buttonDefinitions.Count;
            int currentX = BUTTON_PADDING;

            foreach (ButtonDefinition buttonDef in buttonDefinitions)
            {
                Button button = (Button)widget.createWidgetT("Button", "Button", currentX, 4, buttonWidth - BUTTON_PADDING, 28, Align.Default, "");
                button.Caption = buttonDef.Text;
                button.UserObject = buttonDef.Action;
                button.MouseButtonClick +=new MyGUIEvent(button_MouseButtonClick);
                currentX += buttonWidth;
                buttons.Add(button);
            }
        }

        public override void Dispose()
        {
            child.Dispose();
            base.Dispose();
        }

        public void topLevelResized()
        {
            int buttonWidth = widget.Width / buttons.Count;
            int currentX = BUTTON_PADDING;

            foreach (Button button in buttons)
            {
                button.setCoord(currentX, button.Top, buttonWidth - BUTTON_PADDING, button.Height);
                currentX += buttonWidth;
            }
            child.topLevelResized();
        }

        public void opening()
        {
            child.opening();
        }

        public void closing()
        {
            child.closing();
        }

        public MyGUIViewHost ViewHost
        {
            get
            {
                return child.ViewHost;
            }
        }

        public Widget Widget
        {
            get
            {
                return widget; 
            }
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            child.ViewHost.Context.runAction(source.UserObject.ToString(), child.ViewHost);
        }
    }
}
