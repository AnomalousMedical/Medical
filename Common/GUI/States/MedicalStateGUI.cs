using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    public partial class MedicalStateGUI : DockContent
    {
        private MedicalController medicalController;

        public MedicalStateGUI()
        {
            InitializeComponent();
            medicalStateTrackBar.CurrentBlendChanged += new CurrentBlendChanged(medicalStateTrackBar_CurrentBlendChanged);
        }

        public void initialize(MedicalController medicalController)
        {
            this.medicalController = medicalController;
            medicalController.MedicalStates.StateAdded += new MedicalStateAdded(MedicalStates_StateAdded);
            medicalController.MedicalStates.StateRemoved += new MedicalStateRemoved(MedicalStates_StateRemoved);
            medicalController.MedicalStates.StatesCleared += new MedicalStatesCleared(MedicalStates_StatesCleared);
        }

        public void sceneLoaded()
        {

        }

        public void sceneUnloading()
        {

        }

        void medicalStateTrackBar_CurrentBlendChanged(MedicalStateTrackBar trackBar, double currentTime)
        {
            medicalController.MedicalStates.blend((float)currentTime);
        }

        private void addStateButton_Click(object sender, EventArgs e)
        {
            medicalController.createMedicalState("Test");
        }

        void MedicalStates_StatesCleared(MedicalStateController controller)
        {
            
        }

        void MedicalStates_StateRemoved(MedicalStateController controller, MedicalState state)
        {
            
        }

        void MedicalStates_StateAdded(MedicalStateController controller, MedicalState state)
        {
            medicalStateTrackBar.Enabled = controller.getNumStates() > 1;
            if (medicalStateTrackBar.Enabled)
            {
                medicalStateTrackBar.MaxBlend = controller.getNumStates() - 1;
            }
        }
    }
}
