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
        private List<Button> variableSizeButtons = new List<Button>();
        private int fixedSizeArea = 0;

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

            foreach (ButtonDefinitionBase buttonDef in buttonDefinitions)
            {
                Button button = buttonDef.createButton(widget, currentX, 4, buttonWidth - BUTTON_PADDING, 28);
                button.UserObject = buttonDef.Action;
                button.MouseButtonClick +=new MyGUIEvent(button_MouseButtonClick);
                currentX += button.Width + BUTTON_PADDING;
                buttons.Add(button);
                if (buttonDef.FixedSize)
                {
                    fixedSizeArea += button.Width + BUTTON_PADDING;
                }
                else
                {
                    variableSizeButtons.Add(button);
                }
            }
        }

        public override void Dispose()
        {
            child.Dispose();
            base.Dispose();
        }

        public void topLevelResized()
        {
            int variableButtonCount = variableSizeButtons.Count;
            if (variableButtonCount > 0)
            {
                int buttonWidth = (widget.Width - fixedSizeArea) / variableSizeButtons.Count;
                foreach (Button button in variableSizeButtons)
                {
                    button.setSize(buttonWidth - BUTTON_PADDING, button.Height);
                }
            }
            int currentX = BUTTON_PADDING;

            foreach (Button button in buttons)
            {
                button.setPosition(currentX, button.Top);
                currentX += button.Width + BUTTON_PADDING;
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
