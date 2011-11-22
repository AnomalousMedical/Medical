using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RestartNotification : AbstractNotification
    {
        private StandaloneController standaloneController;
        private bool autoStartPlatformUpdate;

        public RestartNotification(String text, String imageKey, StandaloneController standaloneController, bool autoStartPlatformUpdate)
            :base(text, imageKey)
        {
            this.standaloneController = standaloneController;
            this.autoStartPlatformUpdate = autoStartPlatformUpdate;
        }

        public override void clicked()
        {
            standaloneController.restartWithWarning("Restarting Anomalous Medical will loose all unsaved data. Are you sure you wish to continue?", autoStartPlatformUpdate);
        }
    }
}
