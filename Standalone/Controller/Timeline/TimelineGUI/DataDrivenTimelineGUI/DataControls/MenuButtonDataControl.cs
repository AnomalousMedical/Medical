using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    class MenuButtonDataControl : DataControl
    {
        private Button button;

        public MenuButtonDataControl(Widget parentWidget, MenuItemField numericField)
        {
            button = (Button)parentWidget.createWidgetT("Button", "Button", 0, 0, 100, 20, Align.Default, "");
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
        }

        public override void Dispose()
        {
            
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {

        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            
        }

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public override void layout()
        {
            button.setPosition((int)Location.x, (int)Location.y);
            button.setSize((int)WorkingSize.Width, (int)WorkingSize.Height);
        }

        public override Size2 DesiredSize
        {
            get
            {
                return new Size2(button.Width, button.Height);
            }
        }

        public override bool Visible
        {
            get
            {
                return button.Visible;
            }
            set
            {
                button.Visible = value;
            }
        }
    }
}
