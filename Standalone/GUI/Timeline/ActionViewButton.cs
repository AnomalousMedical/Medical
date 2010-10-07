using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ActionViewButton
    {
        private Button button;
        private TimelineAction action;

        public event EventHandler Clicked;

        public ActionViewButton(Button button, TimelineAction action)
        {
            this.action = action;
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(button);
        }

        public TimelineAction Action
        {
            get
            {
                return action;
            }
        }

        public bool StateCheck
        {
            get
            {
                return button.StateCheck;
            }
            set
            {
                button.StateCheck = value;
            }
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Clicked != null)
            {
                Clicked.Invoke(this, e);
            }
        }
    }
}
