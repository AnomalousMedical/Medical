using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class UpdateGUI : PopupContainer
    {
        private Task openDownloadsTask;

        public UpdateGUI(Task openDownloadsTask)
            :base("Medical.GUI.UpdateGUI.UpdateGUI.layout")
        {
            this.KeepOpen = true;
            this.openDownloadsTask = openDownloadsTask;

            widget.MouseButtonClick += new MyGUIEvent(widget_MouseButtonClick);
            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        void widget_MouseButtonClick(Widget source, EventArgs e)
        {
            openDownloadsTask.clicked(null);
            this.hide();
        }
    }
}
