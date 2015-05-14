using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anomalous.GuiFramework;

namespace Medical.Tasks
{
    class RunIntroTutorial : Task
    {
        private StandaloneController standaloneController;

        public RunIntroTutorial(StandaloneController standaloneController)
            : base("Medical.IntroductionTutorial.Bootstrap", "Introduction Tutorial", "IntroductionTutorial/Icon", "Anomalous Medical")
        {
            this.standaloneController = standaloneController;
            this.ShowOnTaskbar = false;
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            Task introTask;
            if(PlatformConfig.ForwardTouchAsMouse)
            {
                introTask = standaloneController.TaskController.getTask("DDPlugin.IntroductionTutorial.MobileTask");
            }
            else
            {
                introTask = standaloneController.TaskController.getTask("DDPlugin.IntroductionTutorial.DesktopTutorial");
            }

            if (introTask != null)
            {
                introTask.clicked(EmptyTaskPositioner.Instance);
            }
        }
    }
}
