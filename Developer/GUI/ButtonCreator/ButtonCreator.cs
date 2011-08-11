using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

namespace Developer
{
    class ButtonCreator : MDIDialog
    {
        ButtonGrid buttonGrid;
        ulong buttonIndex = 0;

        public ButtonCreator()
            :base("Developer.GUI.ButtonCreator.ButtonCreator.layout")
        {
            buttonGrid = new ButtonGrid((ScrollView)window.findWidget("ButtonScroller"));

            Button add = (Button)window.findWidget("Add");
            add.MouseButtonClick += new MyGUIEvent(add_MouseButtonClick);

            Button remove = (Button)window.findWidget("Remove");
            remove.MouseButtonClick += new MyGUIEvent(remove_MouseButtonClick);

            Button invalid = (Button)window.findWidget("Invalid");
            invalid.MouseButtonClick += new MyGUIEvent(invalid_MouseButtonClick);
        }

        void invalid_MouseButtonClick(Widget source, EventArgs e)
        {
            window.createWidgetT("Button", "Invalid", 0, 0, 10, 10, Align.Default, "");
        }

        void remove_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void add_MouseButtonClick(Widget source, EventArgs e)
        {
            for (int i = 0; i < 10; ++i)
            {
                buttonGrid.addItem("", "Button " + buttonIndex++);
            }
        }
    }
}
