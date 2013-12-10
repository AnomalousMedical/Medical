using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class ClosingTaskbar : Taskbar
    {
        public event Action Close;

        private Button closeButton;

        public ClosingTaskbar()
            : base("Medical.GUI.Taskbar.ClosingTaskbar.layout")
        {
            closeButton = (Button)taskbarWidget.findWidget("Close");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Close != null)
            {
                Close.Invoke();
            }
        }

        protected override void layoutCustomElementsVertical(out Vector2 startLocation, out int heightOffset)
        {
            startLocation = new Vector2(0, closeButton.Bottom + Padding);
            heightOffset = 0;
        }

        protected override void layoutCustomElementsHorizontal(out Vector2 startLocation, out int widthOffset)
        {
            startLocation = new Vector2(0, 0);
            widthOffset = closeButton.Width + Padding;
        }
    }
}
