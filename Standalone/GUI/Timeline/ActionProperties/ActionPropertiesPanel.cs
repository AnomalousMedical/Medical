using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    abstract class ActionPropertiesPanel : IDisposable
    {
        protected Widget parentWidget;
        private Layout layout;
        protected Widget mainWidget;

        protected ActionPropertiesPanel(Widget parentWidget, String layoutFile)
        {
            this.parentWidget = parentWidget;
            layout = LayoutManager.Instance.loadLayout(layoutFile);
            mainWidget = layout.getWidget(0);
            mainWidget.attachToWidget(parentWidget);
            mainWidget.Visible = false;
        }

        public virtual void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void setPosition(int x, int y)
        {
            mainWidget.setPosition(x, y);
        }

        public bool Visible
        {
            get
            {
                return mainWidget.Visible;
            }
            set
            {
                mainWidget.Visible = value;
            }
        }

        public int Bottom
        {
            get
            {
                return mainWidget.Bottom;
            }
        }

        public abstract TimelineAction CurrentAction { get; set; }
    }
}
