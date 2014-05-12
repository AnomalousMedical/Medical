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
        private bool asAdmin;

        public RestartNotification(String text, String imageKey, StandaloneController standaloneController, bool autoStartPlatformUpdate, bool asAdmin)
            :base(text, imageKey)
        {
            this.standaloneController = standaloneController;
            this.autoStartPlatformUpdate = autoStartPlatformUpdate;
            this.asAdmin = asAdmin;
        }

        public override void clicked()
        {
            standaloneController.restartWithWarning("Restarting Anomalous Medical will lose all unsaved data. Are you sure you wish to continue?", autoStartPlatformUpdate, asAdmin);
        }
    }
}
