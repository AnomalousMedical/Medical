using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    sealed class NotificationGUI : PopupContainer
    {
        private NotificationGUIManager notificationManager;
        private Notification notification;
        private bool allowClose = true;

        public NotificationGUI(Notification notification, NotificationGUIManager notificationManager)
            : base("Medical.GUI.Notification.NotificationGUI.layout")
        {
            this.KeepOpen = true;
            this.notificationManager = notificationManager;
            this.notification = notification;

            widget.MouseButtonClick += new MyGUIEvent(widget_MouseButtonClick);
            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            TextBox staticText = (TextBox)widget.findWidget("Text");
            staticText.Caption = notification.Text;

            ImageBox image = (ImageBox)widget.findWidget("Image");
            image.setItemResource(notification.ImageKey);

            //Setup size
            Size2 textSize = staticText.getTextSize();
            if (textSize.Width < staticText.Width)
            {
                textSize.Width = staticText.Width;
            }
            if (textSize.Height < staticText.Height)
            {
                textSize.Height = staticText.Height;
            }
            int widthDelta = widget.Width - staticText.Width;
            int heightDelta = widget.Height - staticText.Height;

            widget.setSize((int)textSize.Width + widthDelta, (int)textSize.Height + heightDelta);
        }

        public void closeNotification()
        {
            if (allowClose)
            {
                if (Visible)
                {
                    this.Hiding += NotificationGUI_Hiding;
                    this.Hidden += new EventHandler(NotificationGUI_Hidden);
                    this.hide();
                    allowClose = false;
                }
                else
                {
                    notificationManager.notificationClosed(this);
                    this.Dispose();
                }
            }
        }

        private void clicked()
        {
            notification.clicked();
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            closeNotification();
        }

        void widget_MouseButtonClick(Widget source, EventArgs e)
        {
            if (allowClose)
            {
                clicked();
                closeNotification();
            }
        }

        void NotificationGUI_Hiding(object sender, CancelEventArgs e)
        {
            notificationManager.notificationClosed(this);
        }

        void NotificationGUI_Hidden(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
