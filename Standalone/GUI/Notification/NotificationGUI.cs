using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class NotificationGUI : PopupContainer
    {
        private NotificationGUIManager notificationManager;

        public NotificationGUI(String text, String imageKey, NotificationGUIManager notificationManager)
            : base("Medical.GUI.Notification.NotificationGUI.layout")
        {
            this.KeepOpen = true;
            this.notificationManager = notificationManager;

            widget.MouseButtonClick += new MyGUIEvent(widget_MouseButtonClick);
            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            StaticText staticText = (StaticText)widget.findWidget("Text");
            staticText.Caption = text;

            StaticImage image = (StaticImage)widget.findWidget("Image");
            image.setItemResource(imageKey);

            this.Hiding += new EventHandler(NotificationGUI_Hiding);
            this.Hidden += new EventHandler(NotificationGUI_Hidden);
        }

        protected virtual void clicked()
        {

        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        void widget_MouseButtonClick(Widget source, EventArgs e)
        {
            clicked();
            this.hide();
        }

        void NotificationGUI_Hiding(object sender, EventArgs e)
        {
            notificationManager.notificationClosed(this);
        }

        void NotificationGUI_Hidden(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
