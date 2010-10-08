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
        private Dictionary<String, ActionViewRow> rows = new Dictionary<string, ActionViewRow>();
        private ActionViewButton currentButton;

        public event EventHandler ActiveActionChanged;

        public ActionView(ScrollView scrollView)
        {
            this.scrollView = scrollView;
            int y = 3;
            foreach (TimelineActionProperties actionProp in TimelineActionFactory.ActionProperties)
            {
                rows.Add(actionProp.TypeName, new ActionViewRow(y, pixelsPerSecond, actionProp.Color));
                y += 19;
            }
        }

        public void Dispose()
        {
            foreach (ActionViewRow row in rows.Values)
            {
                row.Dispose();
            }
        }

        public void addAction(TimelineAction action)
        {
            Button button = scrollView.createWidgetT("Button", "Button", (int)(pixelsPerSecond * action.StartTime), 0, 10, 10, Align.Left | Align.Top, "") as Button;
            ActionViewButton actionButton = rows[action.TypeName].addButton(button, action);
            actionButton.Clicked += new EventHandler(actionButton_Clicked);
            if (button.Right > scrollView.CanvasSize.Width)
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Width = button.Right;
                scrollView.CanvasSize = canvasSize;
            }
            if (button.Bottom > scrollView.CanvasSize.Height)
            {
                Size2 canvasSize = scrollView.CanvasSize;
                canvasSize.Height = button.Bottom;
                scrollView.CanvasSize = canvasSize;
            }
        }

        public void removeAllActions()
        {

        }

        public ActionViewButton CurrentAction
        {
            get
            {
                return currentButton;
            }
            set
            {
                if (currentButton != null)
                {
                    currentButton.StateCheck = false;
                }
                currentButton = value;
                currentButton.StateCheck = true;
                if (ActiveActionChanged != null)
                {
                    ActiveActionChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        void actionButton_Clicked(object sender, EventArgs e)
        {
            CurrentAction = sender as ActionViewButton;
        }
    }
}
