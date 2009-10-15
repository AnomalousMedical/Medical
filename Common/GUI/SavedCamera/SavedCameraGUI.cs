using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine.ObjectManagement;
using Medical.Controller;

namespace Medical.GUI
{
    public partial class SavedCameraGUI : GUIElement
    {
        private ListViewGroup predefined = new ListViewGroup("Predefined");
        private ListViewGroup userDefined = new ListViewGroup("User Defined");
        private SavedCameraController userCameras;
        private DrawingWindowController windowController;
        private NavigationController navigation;
        private bool allowCustomCameras = true;

        public SavedCameraGUI()
        {
            InitializeComponent();
            cameraNameList.MouseDoubleClick += new MouseEventHandler(cameraNameList_MouseDoubleClick);
            cameraNameList.KeyUp += new KeyEventHandler(cameraNameList_KeyUp);
            cameraNameList.Groups.Add(predefined);
            cameraNameList.Groups.Add(userDefined);
        }

        public void initialize(DrawingWindowController windowController, String camerasFile, NavigationController navigation)
        {
            this.windowController = windowController;
            userCameras = new SavedCameraController(camerasFile);
            if (navigation != null)
            {
                this.navigation = navigation;
                navigation.NavigationStateSetChanged += navigation_NavigationStateSetChanged;
            }
        }

        public void createShortcuts(ShortcutController shortcutController)
        {
            ShortcutGroup group = shortcutController.createOrRetrieveGroup("Cameras");
            group.clearShortcuts();
            foreach (ListViewItem item in predefined.Items)
            {
                NavigationState state = item.Tag as NavigationState;
                if (state.ShortcutKey != KeyCodes.None)
                {
                    ShortcutEventCommand shortcutEvent = new ShortcutEventCommand(state.Name, (Keys)state.ShortcutKey, true);
                    shortcutEvent.UserData = state.Name;
                    shortcutEvent.Execute += shortcutEvent_Execute;
                    group.addShortcut(shortcutEvent);
                }
            }
        }

        void shortcutEvent_Execute(ShortcutEventCommand shortcut)
        {
            navigation.setNavigationState(shortcut.UserData.ToString(), windowController.getActiveWindow().DrawingWindow);
        }

        public bool AllowCustomCameras
        {
            get
            {
                return allowCustomCameras;
            }
            set
            {
                allowCustomCameras = value;
                saveCameraButton.Visible = value;
                deleteCameraButton.Visible = value;
            }
        }

        protected override void sceneLoaded(SimScene scene)
        {
            if (allowCustomCameras)
            {
                foreach (String name in userCameras.getSavedCameraNames())
                {
                    ListViewItem item = cameraNameList.Items.Add(name, name, 0);
                    item.Group = userDefined;
                }
            }
            base.sceneLoaded(scene);
        }

        protected override void sceneUnloading()
        {
            cameraNameList.Items.Clear();
            base.sceneUnloading();
        }

        void navigation_NavigationStateSetChanged(NavigationController controller)
        {
            foreach (NavigationState state in controller.NavigationSet.States)
            {
                if (!state.Hidden)
                {
                    ListViewItem item = cameraNameList.Items.Add(state.Name, state.Name, 0);
                    item.Group = predefined;
                    item.Tag = state;
                }
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
                if (item.Group == predefined)
                {
                    navigation.setNavigationState(item.Text, windowController.getActiveWindow().DrawingWindow);
                }
                else
                {
                    SavedCameraDefinition def = userCameras.getSavedCamera(item.Text);
                    windowController.getActiveWindow().DrawingWindow.setCamera(def.Position, def.LookAt);
                }
            }
        }

        private void saveCameraButton_Click(object sender, EventArgs e)
        {
            InputResult result = InputBox.GetInput("Save Camera", "Enter a name for the saved camera.", this.FindForm(), validateSaveCameraName);
            if (result.ok)
            {
                DrawingWindowHost currentWindow = windowController.getActiveWindow();
                userCameras.addOrUpdateSavedCamera(new SavedCameraDefinition(result.text, currentWindow.DrawingWindow.Translation, currentWindow.DrawingWindow.LookAt));
                if (!cameraNameList.Items.ContainsKey(result.text))
                {
                    ListViewItem item = cameraNameList.Items.Add(result.text, result.text, 0);
                    item.Group = userDefined;
                }
                userCameras.saveCameras();
            }
        }

        private void deleteSelectedCamera()
        {
            if (cameraNameList.SelectedItems.Count > 0)
            {
                String selectedItem = cameraNameList.SelectedItems[0].Text;
                if (userCameras.removeSavedCamera(selectedItem))
                {
                    cameraNameList.Items.RemoveByKey(selectedItem);
                }
                userCameras.saveCameras();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (cameraNameList != null)
            {
                cameraNameList.Columns[0].Width = -2;
            }
        }
    }
}
