using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Medical.GUI
{
    class CheckForUpdatesTaskMenuItem : TaskMenuItem
    {
        private StandaloneController standaloneController;

        public CheckForUpdatesTaskMenuItem(StandaloneController standaloneController, int weight)
            :base("Check for Updates", "FileToolstrip/CheckForUpdates", TaskMenuCategories.System)
        {
            this.standaloneController = standaloneController;
            this.Weight = weight;
        }

        public override void clicked()
        {
            UpdateManager.checkForUpdates(Assembly.GetAssembly(this.GetType()).GetName().Version, standaloneController.App.ProductID);
        }
    }
}
