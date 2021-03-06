﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class MyGUIContinuePromptProvider : ContinuePromptProvider, IDisposable
    {
        private Button button;

        public MyGUIContinuePromptProvider()
        {
            button = (Button)Gui.Instance.createWidgetT("Button", "Button", 0, 0, 100, 25, Align.Default, "Overlapped", "");
            button.Caption = "Continue";
            button.setSize((int)button.getTextSize().Width + ScaleHelper.Scaled(25), ScaleHelper.Scaled(25));
            button.Visible = false;
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(button);
        }

        protected override void doShowPrompt(String text)
        {
            ensureVisible(RenderManager.Instance.ViewWidth, RenderManager.Instance.ViewHeight);
            LayerManager.Instance.upLayerItem(button);
            button.Caption = text;
            button.setSize((int)button.getTextSize().Width + ScaleHelper.Scaled(25), ScaleHelper.Scaled(25));
            button.Visible = true;
        }

        protected override void doHidePrompt()
        {
            button.Visible = false;
        }

        /// <summary>
        /// Have the window compute its position to ensure it is visible in the given screen area.
        /// </summary>
        public void ensureVisible(int guiWidth, int guiHeight)
        {
            int left = guiWidth - button.Width - 30;
            int top = guiHeight - button.Height - 30;
            int right = left + button.Width;
            int bottom = top + button.Height;

            if (right > guiWidth)
            {
                left -= right - guiWidth;
                if (left < 0)
                {
                    left = 0;
                }
            }

            if (bottom > guiHeight)
            {
                top -= bottom - guiHeight;
                if (top < 0)
                {
                    top = 0;
                }
            }
            button.setPosition(left, top);
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            this.executeCallback();
        }
    }
}
