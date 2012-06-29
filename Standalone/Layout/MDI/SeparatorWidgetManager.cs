using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.GUI;

namespace Medical.Controller
{
    public abstract class SeparatorWidgetManager : IDisposable
    {
        protected List<Widget> separatorWidgets = new List<Widget>();
        private Gui gui = Gui.Instance;
        protected MDILayoutContainer parentContainer;

        public SeparatorWidgetManager(MDILayoutContainer parentContainer)
        {
            this.parentContainer = parentContainer;
        }

        public void Dispose()
        {
            foreach (Widget widget in separatorWidgets)
            {
                gui.destroyWidget(widget);
            }
        }

        public void createSeparator()
        {
            //Show the previous last separator if avaliable
            if (separatorWidgets.Count > 0)
            {
                separatorWidgets[separatorWidgets.Count - 1].Visible = true;
            }

            Widget separator = gui.createWidgetT("Widget", "MDISeparator", 0, 0, 10, 10, Align.Left | Align.Top, "Back", "");
            separatorWidgets.Add(separator);
            separator.MouseDrag += separator_MouseDrag;
            separator.MouseButtonPressed += separator_MouseButtonPressed;
            separator.MouseButtonReleased += separator_MouseButtonReleased;
            if (parentContainer.Layout == MDILayoutContainer.LayoutType.Horizontal)
            {
                separator.Pointer = MainWindow.SIZE_HORZ;
            }
            else
            {
                separator.Pointer = MainWindow.SIZE_VERT;
            }
            separator.Visible = false;
        }

        public void removeSeparator()
        {
            Widget separator = separatorWidgets[separatorWidgets.Count - 1];
            gui.destroyWidget(separator);
            separatorWidgets.RemoveAt(separatorWidgets.Count - 1);

            //Hide the new last separator
            if (separatorWidgets.Count > 0)
            {
                separatorWidgets[separatorWidgets.Count - 1].Visible = false;
            }
        }

        public void setSeparatorCoord(int index, int x, int y, int width, int height)
        {
            separatorWidgets[index].setCoord(x, y, width, height);
        }

        protected abstract void separator_MouseDrag(Widget source, EventArgs e);

        protected abstract void separator_MouseButtonPressed(Widget source, EventArgs e);

        protected abstract void separator_MouseButtonReleased(Widget source, EventArgs e);
    }
}
