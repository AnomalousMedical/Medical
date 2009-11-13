using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using ComponentFactory.Krypton.Navigator;

namespace Medical.GUI
{
    class StateGUIController : IDisposable
    {
        private BasicForm form;
        private BasicController controller;
        private StateList stateList;
        private KryptonPage kryptonPage;
        private bool pageActive = false;

        public StateGUIController(BasicForm form, BasicController controller)
        {
            this.form = form;
            this.controller = controller;
            
            MedicalStateController stateController = controller.MedicalStateController;
            stateList = new StateList(stateController, controller.ImageRenderer);
            kryptonPage = new KryptonPage("States");
            kryptonPage.Controls.Add(stateList);

            stateController.StateAdded += new MedicalStateAdded(stateController_StateAdded);
            stateController.StateRemoved += new MedicalStateRemoved(stateController_StateRemoved);
            stateController.StatesCleared += new MedicalStatesCleared(stateController_StatesCleared);
        }

        public void Dispose()
        {
            stateList.Dispose();
            kryptonPage.Dispose();
        }

        void stateController_StatesCleared(MedicalStateController controller)
        {
            removePage();
        }

        void stateController_StateRemoved(MedicalStateController controller, MedicalState state, int index)
        {
            if (controller.getNumStates() == 0)
            {
                removePage();
            }
        }

        void stateController_StateAdded(MedicalStateController controller, MedicalState state, int index)
        {
            if (controller.getNumStates() == 1)
            {
                addPage();
            }
        }

        void removePage()
        {
            if (pageActive)
            {
                form.removePrimaryNavigatorPage(kryptonPage);
                pageActive = false;
            }
        }

        void addPage()
        {
            if (!pageActive)
            {
                form.addPrimaryNavigatorPage(kryptonPage);
                pageActive = true;
            }
        }
    }
}
