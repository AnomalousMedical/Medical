using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using Medical.Controller;

namespace Medical.GUI
{
    class MyGUIImageRendererProgress : ImageRendererProgress
    {
        private Layout layout;
        private Widget mainWidget;

        private Progress rendererProgress;
        private StaticText statusText; 

        public MyGUIImageRendererProgress()
        {
            layout = LayoutManager.Instance.loadLayout("Medical.GUI.RenderProgress.MyGUIImageRendererProgress.layout");
            mainWidget = layout.getWidget(0);

            rendererProgress = mainWidget.findWidget("RenderingProgress") as Progress;
            statusText = mainWidget.findWidget("StatusText") as StaticText;

            Visible = false;
        }

        public void update(uint percentage, String status)
        {
            if (Visible)
            {
                rendererProgress.Position = percentage;
                statusText.Caption = status;
                Root.getSingleton()._updateAllRenderTargets();
                WindowFunctions.pumpMessages();
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
                int x = (int)((Gui.Instance.getViewWidth() - mainWidget.Width) / 2.0f);
                int y = (int)((Gui.Instance.getViewHeight() - mainWidget.Height) / 2.0f);
                mainWidget.setPosition(x, y);
                mainWidget.Visible = value;
            }
        }
    }
}
