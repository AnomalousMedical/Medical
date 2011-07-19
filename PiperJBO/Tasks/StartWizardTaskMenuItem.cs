using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class StartWizardTask : Task
    {
        private PiperJBOAtlasPlugin piperAtlasPlugin;
        private StateWizard wizard;

        public StartWizardTask(PiperJBOAtlasPlugin piperAtlasPlugin, StateWizard wizard)
            : base("PiperJBO." + wizard.Name, wizard.Name, wizard.ImageKey, TaskMenuCategories.Exams)
        {
            this.piperAtlasPlugin = piperAtlasPlugin;
            this.wizard = wizard;
            ShowOnTaskbar = false;
        }

        public override void clicked()
        {
            piperAtlasPlugin.startWizard(wizard);
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
