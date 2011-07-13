using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using System.Reflection;

namespace Medical.GUI
{
    public abstract class AppMenu : Component
    {
        public AppMenu(String layout)
            :base(layout)
        {

        }

        internal void putOnTaskMenu(Widget taskMenuWidget)
        {
            widget.attachToWidget(taskMenuWidget);
        }

        public void layout(int parentWidth, int parentHeight)
        {
            widget.setPosition(0, parentHeight / 2 - widget.Height / 2);
        }

        public void hide()
        {

        }
    }
}
