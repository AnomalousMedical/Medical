using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ToothButton
    {
        private Button button;
        private String toothName;

        public ToothButton(Button button)
        {
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
            toothName = button.getUserString("Tooth");
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            bool extracted = !button.StateCheck;
            button.StateCheck = extracted;
            TeethController.getTooth(toothName).Extracted = extracted;
        }
    }
}
