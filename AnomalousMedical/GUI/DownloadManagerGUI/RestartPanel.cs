using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RestartPanel
    {
        private Widget widget;
        private StaticImage restartIcon;
        private Edit restartName;
        private Edit restartDescription;
        public event EventHandler Restart;

        public RestartPanel(Widget widget)
        {
            this.widget = widget;

            restartIcon = (StaticImage)widget.findWidget("RestartIcon");
            restartName = (Edit)widget.findWidget("RestartName");
            restartDescription = (Edit)widget.findWidget("RestartDescription");

            Button restartButton = (Button)widget.findWidget("RestartButton");
            restartButton.MouseButtonClick += new MyGUIEvent(restartButton_MouseButtonClick);
        }

        public void setInfo(UninstallInfo info)
        {
            restartIcon.setItemResource(info.ImageKey);
            restartName.Caption = info.Name;
            restartName.TextCursor = 0;
            restartDescription.Caption = String.Format("You must restart Anomalous Medical to finish uninstalling {0}. You may uninstall more things by selecting them and clicking uninstall before restarting.", info.Name);
            restartDescription.TextCursor = 0;
        }

        internal void setInfo(ServerDownloadInfo info)
        {
            restartIcon.setItemResource(info.ImageKey);
            restartName.Caption = info.Name;
            restartName.TextCursor = 0;
            restartDescription.Caption = String.Format("You must restart Anomalous Medical to finish installing {0}. You may install more things by selecting them and clicking install before restarting.", info.Name);
            restartDescription.TextCursor = 0;
        }

        public bool Visible
        {
            get
            {
                return widget.Visible;
            }
            set
            {
                widget.Visible = value;
            }
        }

        void restartButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Restart != null)
            {
                Restart.Invoke(this, e);
            }
        }
    }
}
