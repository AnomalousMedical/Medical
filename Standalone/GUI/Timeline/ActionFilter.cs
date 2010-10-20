using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ActionFilter : IDisposable
    {
        private ScrollView scrollView;
        private Dictionary<ActionViewRow, ActionFilterButton> filterButtons = new Dictionary<ActionViewRow, ActionFilterButton>();

        private int buttonWidth;
        private int buttonHeight = 19;

        public ActionFilter(ScrollView scrollView, ActionView actionView)
        {
            this.scrollView = scrollView;
            buttonWidth = (int)scrollView.CanvasSize.Width;

            actionView.RowPositionChanged += new ActionViewRowEvent(actionView_RowPositionChanged);
            actionView.CanvasHeightChanged += new CanvasSizeChanged(actionView_CanvasHeightChanged);

            foreach (ActionViewRow row in actionView.Rows)
            {
                String actionName = row.Name;
                Button button = scrollView.createWidgetT("Button", "CheckBox", 0, row.Top, buttonWidth, buttonHeight, Align.Default, "") as Button;
                ActionFilterButton filterButton = new ActionFilterButton(button, actionName);
                filterButtons.Add(row, filterButton);
                button.TextColor = row.Color;
            }
        }

        public void Dispose()
        {

        }

        void actionView_RowPositionChanged(ActionViewRow row)
        {
            ActionFilterButton button = filterButtons[row];
            button.moveButtonTop(row.Top);
        }

        void actionView_CanvasHeightChanged(float newSize)
        {
            Size2 canvasSize = scrollView.CanvasSize;
            canvasSize.Height = newSize;
            scrollView.CanvasSize = canvasSize;
        }
    }
}
