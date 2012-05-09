using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    class NavigationButton : IDisposable
    {
        private Button button;

        public event EventDelegate<NavigationButton> Clicked;

        public NavigationButton(Button button)
        {
            this.button = button;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
            button.UserObject = this;
            Layout = new MyGUILayoutContainer(button);
            Action = null;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Clicked != null)
            {
                Clicked.Invoke(this);
            }
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(button);
        }

        public LayoutContainer Layout { get; set; }

        public String Action { get; set; }

        public bool Visible
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

        public bool Selected
        {
            get
            {
                return button.Selected;
            }
            set
            {
                button.Selected = value;
            }
        }

        public int RightEdge
        {
            get
            {
                return button.Right;
            }
        }
    }
}
