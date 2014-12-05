using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using Logging;
using System.Threading;
using MyGUIPlugin;
using Medical.Controller;
using System.Diagnostics;
using Engine.Platform;
using OgrePlugin;

namespace Medical.GUI
{
    public class OpenPatientDialog : AbstractFullscreenGUIPopup
    {
        public event EventHandler OpenFile;

        private MultiListBox fileDataGrid;
        private EditBox locationTextBox;
        private ImageBox warningImage;
        private TextBox warningText;
        private ProgressBar loadingProgress;
        private Button openButton;
        private Button deleteButton;
        private EditBox searchBox;
        private Widget loadingWidget;
        private bool allowOpen = false;

        private PatientDataFile currentFile = null;

        private ListPatientsBgTask listPatientsTask;

        public OpenPatientDialog(GUIManager guiManager)
            : base("Medical.GUI.FileManagement.OpenPatientDialog.layout", guiManager)
        {
            fileDataGrid = widget.findWidget("Open/FileList") as MultiListBox;
            locationTextBox = widget.findWidget("Open/LoadLocation") as EditBox;
            warningImage = widget.findWidget("Open/WarningImage") as ImageBox;
            warningText = widget.findWidget("Open/WarningText") as TextBox;
            loadingProgress = widget.findWidget("Open/LoadingProgress") as ProgressBar;
            openButton = widget.findWidget("Open/OpenButton") as Button;
            deleteButton = widget.findWidget("Open/DeleteButton") as Button;
            Button cancelButton = widget.findWidget("Open/CancelButton") as Button;
            searchBox = widget.findWidget("Open/SearchText") as EditBox;
            Button browseButton = widget.findWidget("Open/BrowseButton") as Button;

            int fileGridWidth = fileDataGrid.Width - 2;
            fileDataGrid.addColumn("First Name", 50);
            fileDataGrid.setColumnResizingPolicyAt(0, ResizingPolicy.Fill);
            fileDataGrid.addColumn("Last Name", 50);
            fileDataGrid.setColumnResizingPolicyAt(1, ResizingPolicy.Fill);
            fileDataGrid.addColumn("Date Modified", 50);
            fileDataGrid.setColumnResizingPolicyAt(2, ResizingPolicy.Fill);
            fileDataGrid.ListChangePosition += new MyGUIEvent(fileDataGrid_ListChangePosition);
            fileDataGrid.ListSelectAccept += new MyGUIEvent(fileDataGrid_ListSelectAccept);
            fileDataGrid.SortOnChanges = false;
            
            String saveDirectory = MedicalConfig.PatientSaveDirectory;
            if (!Directory.Exists(saveDirectory))
            {
                try
                {
                    Directory.CreateDirectory(saveDirectory);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not create save file directory at location {0}. Reason {1}", saveDirectory, ex.Message);
                }
            }
            locationTextBox.Caption = saveDirectory;
            locationTextBox.EventEditTextChange += new MyGUIEvent(locationTextBox_EventEditTextChange);

            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);
            searchBox.KeyButtonReleased += new MyGUIEvent(searchBox_KeyButtonReleased);

            warningImage.Visible = warningText.Visible = false;
            loadingProgress.Visible = false;
            loadingProgress.Range = 100;

            listPatientsTask = new ListPatientsBgTask(this);

            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);
            deleteButton.MouseButtonClick += new MyGUIEvent(deleteButton_MouseButtonClick);
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);

            loadingWidget = widget.findWidget("Loading");
            loadingWidget.Visible = false;

            this.Showing += new EventHandler(OpenPatientDialog_Showing);
            this.Shown += new EventHandler(OpenPatientDialog_Shown);
            this.Hiding += OpenPatientDialog_Hiding;
            this.Hidden += new EventHandler(OpenPatientDialog_Hidden);
        }

        void searchBox_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            switch (ke.Key)
            {
                case KeyboardButtonCode.KC_RETURN:
                    if (fileDataGrid.hasItemSelected())
                    {
                        fireOpen();
                    }
                    break;
            }
        }

        void searchBox_EventEditTextChange(Widget source, EventArgs e)
        {
            uint index;
            if (fileDataGrid.findSubItemWith(1, searchBox.Caption, out index))
            {
                fileDataGrid.setIndexSelected(index);
            }
            toggleButtonsEnabled();
        }

        void locationTextBox_EventEditTextChange(Widget source, EventArgs e)
        {
            listFiles();
        }

        void OpenPatientDialog_Hidden(object sender, EventArgs e)
        {
            fileDataGrid.removeAllItems();
            searchBox.Caption = "";
        }

        void OpenPatientDialog_Hiding(object sender, Engine.CancelEventArgs e)
        {
            if (listPatientsTask.IsWorking)
            {
                e.Cancel = true;
                listPatientsTask.CancelPostAction = CancelPostAction.Close;
                listPatientsTask.cancel();
            }
        }

        void OpenPatientDialog_Showing(object sender, EventArgs e)
        {
            loadingWidget.Visible = false;
            allowOpen = true;
        }

        void OpenPatientDialog_Shown(object sender, EventArgs e)
        {
            listFiles();
            currentFile = null;
            toggleButtonsEnabled();
            listPatientsTask.CancelPostAction = CancelPostAction.None;
            InputManager.Instance.setKeyFocusWidget(searchBox);
        }

        public PatientDataFile CurrentFile
        {
            get
            {
                return currentFile;
            }
        }

        public void clearFileList()
        {
            fileDataGrid.removeAllItems();
        }

        public void addFile(PatientDataFile file)
        {
            fileDataGrid.addItem(file.FirstName, file);
            uint newIndex = fileDataGrid.getItemCount() - 1;
            fileDataGrid.setSubItemNameAt(1, newIndex, file.LastName);
            fileDataGrid.setSubItemNameAt(2, newIndex, file.DateModified.ToString());
        }

        public bool LoadingProgressVisible
        {
            get
            {
                return loadingProgress.Visible;
            }
            set
            {
                loadingProgress.Visible = value;
            }
        }

        public uint LoadingProgress
        {
            get
            {
                return loadingProgress.Position;
            }
            set
            {
                loadingProgress.Position = value;
            }
        }

        void fileDataGrid_ListSelectAccept(Widget source, EventArgs e)
        {
            openButton_MouseButtonClick(null, null);
        }

        void fileDataGrid_ListChangePosition(Widget source, EventArgs e)
        {
            toggleButtonsEnabled();
        }

        private void toggleButtonsEnabled()
        {
            deleteButton.Enabled = openButton.Enabled = fileDataGrid.hasItemSelected();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            currentFile = null;
            this.hide();
        }

        void deleteButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (fileDataGrid.hasItemSelected())
            {
                uint selectedIndex = fileDataGrid.getIndexSelected();
                PatientDataFile deleteFile = (PatientDataFile)fileDataGrid.getItemDataAt(selectedIndex);
                try
                {
                    File.Delete(deleteFile.BackingFile);
                    fileDataGrid.removeItemAt(selectedIndex);
                    fileDataGrid_ListChangePosition(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.show(String.Format("Could not delete file {0}.\nReason\n{1}.", deleteFile.BackingFile, ex.Message), "Delete Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError, null);
                }
            }
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fireOpen();
        }

        private void fireOpen()
        {
            if (allowOpen && fileDataGrid.hasItemSelected())
            {
                allowOpen = false;
                loadingWidget.Visible = true;
                OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget.update();
                currentFile = (PatientDataFile)fileDataGrid.getItemDataAt(fileDataGrid.getIndexSelected());
                if (OpenFile != null)
                {
                    OpenFile.Invoke(this, EventArgs.Empty);
                }
                this.hide();
            }
        }

        private void listFiles()
        {
            listPatientsTask.listFiles(locationTextBox.Caption);
            warningImage.Visible = warningText.Visible = !listPatientsTask.CanDoWork;
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Choose the path to load files from.", locationTextBox.Caption);
            dirDialog.showModal((result, path) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    locationTextBox.Caption = path;
                    listFiles();
                }
            });
        }
    }
}
