using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Medical.GUI
{
    class CheckForUpdatesTask : Task
    {
        private StandaloneController standaloneController;

        public CheckForUpdatesTask(StandaloneController standaloneController, int weight)
            :base("Medical.CheckForUpdates", "Check for Updates", "FileToolstrip/CheckForUpdates", TaskMenuCategories.System)
        {
            this.standaloneController = standaloneController;
            this.Weight = weight;
            this.ShowOnTaskbar = false;
        }

        public override void clicked()
        {
            UpdateManager.checkForUpdates(Assembly.GetAssembly(this.GetType()).GetName().Version, standaloneController.App.ProductID);
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
