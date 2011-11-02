using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RestartNotification : NotificationGUI
    {
        private StandaloneController standaloneController;
        private bool autoStartPlatformUpdate;

        public RestartNotification(String text, String imageKey, NotificationGUIManager notificationManager, StandaloneController standaloneController, bool autoStartPlatformUpdate)
            :base(text, imageKey, notificationManager)
        {
            this.standaloneController = standaloneController;
            this.autoStartPlatformUpdate = autoStartPlatformUpdate;
        }

        protected override void clicked()
        {
            MessageBox.show("Restarting Anomalous Medical will loose all unsaved data. Are you sure you wish to continue?", "Restart", MessageBoxStyle.IconInfo | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
            {
                if (result == MessageBoxStyle.Yes)
                {
                    UpdateController.AutoStartUpdate = autoStartPlatformUpdate;
                    standaloneController.restart();
                }
            });
        }
    }
}
