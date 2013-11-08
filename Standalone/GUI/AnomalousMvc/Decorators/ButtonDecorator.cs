using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    class ButtonDecorator : Component, ViewHostComponent, ButtonFactory
    {
        private ViewHostComponent child;
        private int widgetHeight;
        private static int BUTTON_AREA_HEIGHT = ScaleHelper.Scaled(36);
        private static int BUTTON_PADDING = ScaleHelper.Scaled(3);
        private static int BUTTON_Y = ScaleHelper.Scaled(4);
        private static int BUTTON_HEIGHT = ScaleHelper.Scaled(28);
        private static int BUTTON_X_BIG_WIDTH = ScaleHelper.Scaled(24);
        private static int BUTTON_X_BIG_HEIGHT = ScaleHelper.Scaled(20);

        private List<Button> buttons = new List<Button>();
        private List<Button> variableSizeButtons = new List<Button>();
        private int fixedSizeArea = 0;
        private int currentX;

        public ButtonDecorator(ViewHostComponent child, ButtonCollection buttonDefinitions, ButtonFactory buttonFactory = null)
            :base("Medical.GUI.AnomalousMvc.Decorators.ButtonDecorator.layout")
        {
            this.child = child;
            widget.setSize(child.Widget.Width, child.Widget.Height + BUTTON_AREA_HEIGHT);
            child.Widget.attachToWidget(widget);
            child.Widget.setPosition(0, BUTTON_AREA_HEIGHT);
            child.Widget.Align = Align.Stretch;

            widgetHeight = widget.Height;

            int buttonWidth = widget.Width / buttonDefinitions.Count;
            currentX = BUTTON_PADDING;

            if (buttonFactory == null)
            {
                buttonFactory = this;
            }

            foreach (ButtonDefinitionBase buttonDef in buttonDefinitions)
            {
                buttonDef.createButton(buttonFactory, currentX, BUTTON_Y, buttonWidth - BUTTON_PADDING, BUTTON_HEIGHT);
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

        public void animatedResizeStarted(IntSize2 finalSize)
        {
            child.animatedResizeStarted(finalSize);
        }

        public void animatedResizeCompleted()
        {
            child.animatedResizeCompleted();
        }

        public void opening()
        {
            child.opening();
        }

        public void closing()
        {
            child.closing();
        }

        public void populateViewData(IDataProvider dataProvider)
        {
            child.populateViewData(dataProvider);
        }

        public void analyzeViewData(IDataProvider dataProvider)
        {
            child.analyzeViewData(dataProvider);
        }

        public ViewHostControl findControl(string name)
        {
            return child.findControl(name);
        }

        public void changeScale(float newScale)
        {
            child.changeScale(newScale);
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

        public void addTextButton(ButtonDefinition buttonDefinition, int x, int y, int width, int height)
        {
            Button button = (Button)widget.createWidgetT("Button", "Button", x, y, width, height, Align.Default, "");
            button.Caption = buttonDefinition.Text;
            addButton(button, buttonDefinition);
        }

        public void addCloseButton(CloseButtonDefinition buttonDefinition, int x, int y, int width, int height)
        {
            addButton((Button)widget.createWidgetT("Button", "ButtonXBig", x, y + BUTTON_Y, BUTTON_X_BIG_WIDTH, BUTTON_X_BIG_HEIGHT, Align.Default, ""), buttonDefinition);
        }

        private void addButton(Button button, ButtonDefinitionBase buttonDef)
        {
            button.UserObject = buttonDef.Action;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
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
}
