using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class AppButtonTaskbar : Taskbar
    {
        public delegate void OpenTaskMenuEvent(int left, int top, int width, int height);
        public event OpenTaskMenuEvent OpenTaskMenu;

        private Button appButton;

        private String wideIcon;
        private String narrowIcon;

        private int appButtonWideWidth;

        public AppButtonTaskbar()
            : base("Medical.GUI.Taskbar.AppButtonTaskbar.layout")
        {
            appButton = taskbarWidget.findWidget("AppButton") as Button;
            appButton.MouseButtonClick += new MyGUIEvent(appButton_MouseButtonClick);

            appButtonWideWidth = appButton.Width;
        }

        public void setAppIcon(String wideIcon, String narrowIcon)
        {
            this.wideIcon = wideIcon;
            this.narrowIcon = narrowIcon;
            appButton.ImageBox.setItemResource(wideIcon);
        }

        public bool AppButtonVisible
        {
            get
            {
                return appButton.Visible;
            }
            set
            {
                if (value != appButton.Visible)
                {
                    appButton.Visible = value;
                    layout();
                }
            }
        }

        void appButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (OpenTaskMenu != null)
            {
                int left = 0;
                int top = 0;
                int width = (int)TopmostWorkingSize.Width;
                int height = (int)TopmostWorkingSize.Height;
                switch (this.Alignment)
                {
                    case TaskbarAlignment.Left:
                        width -= taskbarWidget.Width;
                        left = taskbarWidget.Right;
                        break;
                    case TaskbarAlignment.Right:
                        width -= taskbarWidget.Width;
                        break;
                    case TaskbarAlignment.Top:
                        height -= taskbarWidget.Height;
                        top = taskbarWidget.Bottom;
                        break;
                    case TaskbarAlignment.Bottom:
                        height -= taskbarWidget.Height;
                        break;
                }
                OpenTaskMenu.Invoke(left, top, width, height);
            }
        }

        protected override void layoutCustomElementsVertical(out Vector2 startLocation, out int heightOffset)
        {
            appButton.ImageBox.setItemResource(narrowIcon);
            appButton.ImageBox.setSize(ScaleHelper.Scaled(28), ScaleHelper.Scaled(32));
            appButton.setSize(taskbarButtonWidth, appButton.Height);
            startLocation = new Vector2(appButton.Left, 0);

            heightOffset = 0;
            if (appButton.Visible)
            {
                heightOffset = 0;
                startLocation.y = appButton.Bottom + Padding;
            }
        }

        protected override void layoutCustomElementsHorizontal(out Vector2 startLocation, out int widthOffset)
        {
            appButton.ImageBox.setItemResource(wideIcon);
            appButton.ImageBox.setSize(ScaleHelper.Scaled(114), ScaleHelper.Scaled(32));
            appButton.setSize(appButtonWideWidth, appButton.Height);
            startLocation = new Vector2(0, appButton.Top);

            widthOffset = 0;
            if (appButton.Visible)
            {
                widthOffset = 0;
                startLocation.x = appButton.Right + Padding;
            }
        }
    }
}
