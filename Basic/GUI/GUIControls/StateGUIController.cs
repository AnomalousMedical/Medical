using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using ComponentFactory.Krypton.Navigator;
using System.Windows.Forms;

namespace Medical.GUI
{
    class StateGUIController : IDisposable
    {
        private BasicForm form;
        private BasicController controller;
        private StateList stateList;
        private KryptonPage stateListPage;
        private StateNotesPanel notesPanel;
        private KryptonPage notesPage;
        private bool pageActive = false;

        public StateGUIController(BasicForm form, BasicController controller)
        {
            this.form = form;
            this.controller = controller;
            
            MedicalStateController stateController = controller.MedicalStateController;

            stateList = new StateList(stateController);
            stateList.Dock = DockStyle.Fill;
            stateListPage = new KryptonPage("States");
            stateListPage.Controls.Add(stateList);
            stateListPage.TextTitle = "States";
            stateListPage.MinimumSize = stateList.MinimumSize;

            notesPanel = new StateNotesPanel(controller.MedicalStateController, controller.ShortcutController);
            notesPanel.Dock = DockStyle.Fill;
            notesPage = new KryptonPage("Notes");
            notesPage.Controls.Add(notesPanel);
            notesPage.TextTitle = "Notes";
            notesPage.MinimumSize = stateList.MinimumSize;

            stateController.StateAdded += new MedicalStateAdded(stateController_StateAdded);
            stateController.StateRemoved += new MedicalStateRemoved(stateController_StateRemoved);
            stateController.StatesCleared += new MedicalStateEvent(stateController_StatesCleared);
        }

        public void Dispose()
        {
            stateList.Dispose();
            stateListPage.Dispose();
        }

        void stateController_StatesCleared(MedicalStateController controller)
        {
            removePage();
        }

        void stateController_StateRemoved(MedicalStateController controller, MedicalState state, int index)
        {
            if (controller.getNumStates() == 1)
            {
                removePage();
            }
        }

        void stateController_StateAdded(MedicalStateController controller, MedicalState state, int index)
        {
            if (controller.getNumStates() == 2)
            {
                addPage();
            }
        }

        void removePage()
        {
            if (pageActive)
            {
                form.removePrimaryNavigatorPage(stateListPage);
                form.removePrimaryNavigatorPage(notesPage);
                pageActive = false;
            }
        }

        void addPage()
        {
            if (!pageActive)
            {
                form.addPrimaryNavigatorPage(stateListPage);
                form.addPrimaryNavigatorPage(notesPage);
                pageActive = true;
            }
        }
    }
}
