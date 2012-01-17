using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Logging;

namespace Medical.GUI
{
    public class SavePatientDialog : AbstractFullscreenGUIPopup
    {
        public event EventHandler SaveFile;

        private PatientDataFile patientData;
        private bool allowFileNameUpdate = true;
        private bool autoUpdate = false;

        Edit lastText;
        Edit firstText;
        Edit fileNameTextBox;
        Edit locationTextBox;
        StaticText warningLabel;
        StaticImage warningImage;
        Button saveButton;
        Button cancelButton;
        Widget dataPanel;

        public SavePatientDialog(GUIManager guiManager)
            : base("Medical.GUI.FileManagement.SavePatientDialog.layout", guiManager)
        {
            firstText = widget.findWidget("Save/FirstName") as Edit;
            lastText = widget.findWidget("Save/LastName") as Edit;
            fileNameTextBox = widget.findWidget("Save/Filename") as Edit;
            locationTextBox = widget.findWidget("Save/Location") as Edit;
            warningLabel = widget.findWidget("Save/WarningLabel") as StaticText;
            warningImage = widget.findWidget("Save/WarningImage") as StaticImage;
            saveButton = widget.findWidget("Save/SaveButton") as Button;
            cancelButton = widget.findWidget("Save/CancelButton") as Button;
            Button browseButton = widget.findWidget("Save/BrowseButton") as Button;
            dataPanel = widget.findWidget("DataPanel");

            lastText.EventEditTextChange +=new MyGUIEvent(lastText_EventEditTextChange);
            firstText.EventEditTextChange += new MyGUIEvent(firstText_EventEditTextChange);
            fileNameTextBox.EventEditTextChange += new MyGUIEvent(fileNameTextBox_EventEditTextChange);
            locationTextBox.EventEditTextChange += new MyGUIEvent(locationTextBox_EventEditTextChange);
            warningLabel.Visible = false;
            warningImage.Visible = false;
            saveButton.MouseButtonClick += new MyGUIEvent(saveButton_MouseButtonClick);
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);

            this.Shown += new EventHandler(SavePatientDialog_Shown);
        }

        public override void setSize(int width, int height)
        {
            base.setSize(width, height);
            if (width > 800)
            {
                width = 800;
            }
            dataPanel.setSize(width, dataPanel.Height);
        }

        public void save()
        {
            if (patientData == null)
            {
                this.show(0, 0);
            }
            else
            {
                fireSaveFile();
            }
        }

        public void saveAs()
        {
            this.show(0, 0);
        }

        void locationTextBox_EventEditTextChange(Widget source, EventArgs e)
        {
            checkForFile();
        }

        void fileNameTextBox_EventEditTextChange(Widget source, EventArgs e)
        {
            if (!autoUpdate)
            {
                allowFileNameUpdate = false;
            }
            checkForFile();
        }

        void firstText_EventEditTextChange(Widget source, EventArgs e)
        {
            updateFileName();
        }

        void lastText_EventEditTextChange(Widget source, EventArgs e)
        {
            updateFileName();
        }

        void updateFileName()
        {
            if (allowFileNameUpdate)
            {
                autoUpdate = true;
                fileNameTextBox.Caption = String.Format("{0}, {1}", lastText.Caption, firstText.Caption);
                autoUpdate = false;
                checkForFile();
            }
        }

        void checkForFile()
        {
            warningImage.Visible = warningLabel.Visible = File.Exists(SavePath);
        }

        void SavePatientDialog_Shown(object sender, EventArgs e)
        {
            if (patientData == null)
            {
                lastText.Caption = "";
                firstText.Caption = "";
                fileNameTextBox.Caption = "";
                locationTextBox.Caption = MedicalConfig.SaveDirectory;
            }
            else
            {
                lastText.Caption = patientData.LastName;
                firstText.Caption = patientData.FirstName;
                fileNameTextBox.Caption = patientData.BackingFileName;
                locationTextBox.Caption = patientData.BackingFileDirectory;
            }
            allowFileNameUpdate = true;
            InputManager.Instance.setKeyFocusWidget(firstText);
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    MessageBox.show(String.Format("The file {0} already exists. Would you like to overwrite it?", fileNameTextBox.Caption),
                        "Overwrite?", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, overwriteMessageCallback);
                }
                else
                {
                    doSaveAndClose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving this patient.\nTry using a different file name and do not include special characters such as \\ / : * ? \" < > and |."), "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Exception saving patient data. Type {0}. Message {1}.", ex.GetType().ToString(), ex.Message);
            }
        }

        private void overwriteMessageCallback(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                doSaveAndClose();
            }
        }

        private void doSaveAndClose()
        {
            try
            {
                patientData = new PatientDataFile(SavePath);
                patientData.FirstName = firstText.Caption;
                patientData.LastName = lastText.Caption;
                this.hide();
                fireSaveFile();
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving this patient.\nTry using a different file name and do not include special characters such as \\ / : * ? \" < > and |."), "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Exception saving patient data. Type {0}. Message {1}.", ex.GetType().ToString(), ex.Message);
            }
        }

        public PatientDataFile PatientData
        {
            get
            {
                return patientData;
            }
            set
            {
                patientData = value;
            }
        }

        public String SavePath
        {
            get
            {
                return String.Format("{0}/{1}.pdt", locationTextBox.Caption, fileNameTextBox.Caption);
            }
        }

        private void fireSaveFile()
        {
            if (SaveFile != null)
            {
                SaveFile.Invoke(this, EventArgs.Empty);
            }
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            using (DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Choose the path to load files from.", locationTextBox.Caption))
            {
                if (dirDialog.showModal() == NativeDialogResult.OK)
                {
                    locationTextBox.Caption = dirDialog.Path;
                }
            }
        }
    }
}
