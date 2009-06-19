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
    public partial class SavedCameraGUI : DockContent
    {
        private MedicalController medicalController;

        public SavedCameraGUI()
        {
            InitializeComponent();
            cameraNameList.MouseDoubleClick += new MouseEventHandler(cameraNameList_MouseDoubleClick);
            cameraNameList.KeyUp += new KeyEventHandler(cameraNameList_KeyUp);
        }

        public void initialize(MedicalController medicalController)
        {
            this.medicalController = medicalController;
            foreach (String name in medicalController.DrawingWindowController.getSavedCameraNames())
            {
                cameraNameList.Items.Add(name);
            }
        }

        private void activateButton_Click(object sender, EventArgs e)
        {
            activateSelectedCamera();
        }

        private void deleteCameraButton_Click(object sender, EventArgs e)
        {
            deleteSelectedCamera();
        }

        private void saveCameraButton_Click(object sender, EventArgs e)
        {
            InputResult result = InputBox.GetInput("Save Camera", "Enter a name for the saved camera.", this.FindForm());
            if (result.ok)
            {
                medicalController.DrawingWindowController.saveCamera(result.text);
                if (!cameraNameList.Items.Contains(result.text))
                {
                    cameraNameList.Items.Add(result.text);
                }
            }
        }

        void cameraNameList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            activateSelectedCamera();
        }

        void cameraNameList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteSelectedCamera();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                activateSelectedCamera();
            }
        }

        private void activateSelectedCamera()
        {
            if (cameraNameList.SelectedItem != null)
            {
                medicalController.DrawingWindowController.restoreSavedCamera(cameraNameList.SelectedItem.ToString());
            }
        }

        private void deleteSelectedCamera()
        {
            if (cameraNameList.SelectedItem != null)
            {
                String selectedItem = cameraNameList.SelectedItem.ToString();
                if (medicalController.DrawingWindowController.destroySavedCamera(selectedItem))
                {
                    cameraNameList.Items.Remove(selectedItem);
                }
            }
        }
    }
}
