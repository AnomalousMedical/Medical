using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ActionFilterButton : IDisposable
    {
        private Button button;
        private MyGUILayoutContainer layout;
        private CheckButton checkButton;

        public ActionFilterButton(Button button, String actionType)
        {
            this.button = button;
            button.Caption = actionType;
            layout = new MyGUILayoutContainer(button);
            checkButton = new CheckButton(button);
            checkButton.Checked = true;
            checkButton.CheckedChanged += new MyGUIEvent(checkButton_CheckedChanged);
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(button);
        }

        public MyGUILayoutContainer Layout
        {
            get { return layout; }
        }

        void checkButton_CheckedChanged(Widget source, EventArgs e)
        {
            
        }
    }
}
