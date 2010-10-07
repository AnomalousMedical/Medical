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
        private FlowLayoutContainer flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 0.0f, new Vector2(0.0f, 0.0f));
        private Dictionary<String, ActionFilterButton> filterButtons = new Dictionary<string, ActionFilterButton>();

        private int buttonWidth;
        private int buttonHeight = 19;

        public ActionFilter(ScrollView scrollView)
        {
            this.scrollView = scrollView;
            buttonWidth = (int)scrollView.CanvasSize.Width;

            foreach (String actionName in TimelineActionFactory.ActionNames)
            {
                Button button = scrollView.createWidgetT("Button", "CheckBox", 0, 0, buttonWidth, buttonHeight, Align.Default, "") as Button;
                ActionFilterButton filterButton = new ActionFilterButton(button, actionName);
                flowLayout.addChild(filterButton.Layout);
                scrollView.CanvasSize = flowLayout.DesiredSize;
                filterButtons.Add(actionName, filterButton);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
