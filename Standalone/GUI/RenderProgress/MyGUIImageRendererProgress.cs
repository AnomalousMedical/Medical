﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using OgrePlugin;

namespace Medical.GUI
{
    class MyGUIImageRendererProgress : ImageRendererProgress, IDisposable
    {
        private Layout layout;
        private Widget mainWidget;

        private ProgressBar rendererProgress;
        private TextBox statusText; 

        public MyGUIImageRendererProgress()
        {
            layout = LayoutManager.Instance.loadLayout("Medical.GUI.RenderProgress.MyGUIImageRendererProgress.layout");
            mainWidget = layout.getWidget(0);

            rendererProgress = mainWidget.findWidget("RenderingProgress") as ProgressBar;
            statusText = mainWidget.findWidget("StatusText") as TextBox;

            Button cancelButton = (Button)mainWidget.findWidget("CancelButton");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Visible = false;
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void update(uint percentage, String status)
        {
            if (Visible)
            {
                rendererProgress.Position = percentage;
                statusText.Caption = status;
                OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget.update();
            }
        }

        public bool Visible
        {
            get
            {
                return mainWidget.Visible;
            }
            set
            {
                if (value)
                {
                    int x = (int)((RenderManager.Instance.ViewWidth - mainWidget.Width) / 2.0f);
                    int y = (int)((RenderManager.Instance.ViewHeight - mainWidget.Height) / 2.0f);
                    mainWidget.setPosition(x, y);
                    InputManager.Instance.addWidgetModal(mainWidget);
                }
                else
                {
                    if (mainWidget.Visible)
                    {
                        InputManager.Instance.removeWidgetModal(mainWidget);
                    }
                }
                mainWidget.Visible = value;
            }
        }

        public bool Cancel { get; set; }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Cancel = true;
        }
    }
}
