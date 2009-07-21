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
    public partial class SavedCameraGUI : GUIElement
    {
        private DrawingWindowController drawingWindowController;
        private ListViewGroup defaultGroup = new ListViewGroup("Default");
        private ListViewGroup userDefined = new ListViewGroup("User Defined");

        public SavedCameraGUI()
        {
            InitializeComponent();
            cameraNameList.MouseDoubleClick += new MouseEventHandler(cameraNameList_MouseDoubleClick);
            cameraNameList.KeyUp += new KeyEventHandler(cameraNameList_KeyUp);
            cameraNameList.Groups.Add(defaultGroup);
            cameraNameList.Groups.Add(userDefined);
        }

        public void initialize(DrawingWindowController drawingWindowController)
        {
            this.drawingWindowController = drawingWindowController;
            //foreach (String name in PredefinedCameraController.getCameraNameList())
            //{
            //    ListViewItem item = cameraNameList.Items.Add(name, name, 0);
            //    item.Group = defaultGroup;
            //}
            //foreach (String name in drawingWindowController.getSavedCameraNames())
            //{
            //    ListViewItem item = cameraNameList.Items.Add(name, name, 0);
            //    item.Group = userDefined;
            //}
        }

        protected override void sceneLoaded()
        {
            foreach (String name in PredefinedCameraController.getCameraNameList())
            {
                ListViewItem item = cameraNameList.Items.Add(name, name, 0);
                item.Group = defaultGroup;
            }
            foreach (String name in drawingWindowController.getSavedCameraNames())
            {
                ListViewItem item = cameraNameList.Items.Add(name, name, 0);
                item.Group = userDefined;
            }
            base.sceneLoaded();
        }

        protected override void sceneUnloading()
        {
            cameraNameList.Items.Clear();
            base.sceneUnloading();
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
            InputResult result = InputBox.GetInput("Save Camera", "Enter a name for the saved camera.", this.FindForm(), validateSaveCameraName);
            if (result.ok)
            {
                drawingWindowController.saveCamera(result.text);
                if (!cameraNameList.Items.ContainsKey(result.text))
                {
                    cameraNameList.Items.Add(result.text, result.text, 0);
                }
            }
        }

        private bool validateSaveCameraName(string input, out string error)
        {
            if (input == null || input == "")
            {
                error = "Please enter a non empty name.";
                return false;
            }
            if (cameraNameList.Items.ContainsKey(input))
            {
                error = "Camera name already exists please enter another.";
                return false;
            }
            error = "";
            return true;
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
            if (cameraNameList.SelectedItems.Count > 0)
            {
                ListViewItem item = cameraNameList.SelectedItems[0];
                if (item.Group == defaultGroup)
                {
                    drawingWindowController.restorePredefinedCamera(item.Text);
                }
                else
                {
                    drawingWindowController.restoreSavedCamera(item.Text);
                }
            }
        }

        private void deleteSelectedCamera()
        {
            if (cameraNameList.SelectedItems.Count > 0)
            {
                String selectedItem = cameraNameList.SelectedItems[0].Text;
                if (drawingWindowController.destroySavedCamera(selectedItem))
                {
                    cameraNameList.Items.RemoveByKey(selectedItem);
                }
            }
        }
    }
}
