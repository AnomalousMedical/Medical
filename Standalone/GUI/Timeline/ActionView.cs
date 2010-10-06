using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ActionView : IDisposable
    {
        private ScrollView scrollView;
        private int pixelsPerSecond = 100;
        private Dictionary<String, List<ActionViewButton>> buttons = new Dictionary<string, List<ActionViewButton>>();

        public ActionView(ScrollView scrollView)
        {
            this.scrollView = scrollView;
        }

        public void Dispose()
        {
            removeAllActions();
        }

        public void addAction(TimelineAction action)
        {
            Button button = scrollView.createWidgetT("Button", "Button", (int)(pixelsPerSecond * action.StartTime), 0, 10, 10, Align.Left | Align.Top, "") as Button;
            ActionViewButton viewButton = new ActionViewButton(button, action);
            if (button.Right > scrollView.CanvasSize.Width)
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Width = button.Right;
                scrollView.CanvasSize = canvasSize;
            }
        }

        public void removeAllActions()
        {

        }
    }
}
