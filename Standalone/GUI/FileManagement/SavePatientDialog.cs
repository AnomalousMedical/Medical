using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    public class SavePatientDialog : Dialog
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

        public SavePatientDialog()
            : base("Medical.GUI.FileManagement.SavePatientDialog.layout")
        {
            firstText = window.findWidget("Save/FirstName") as Edit;
            lastText = window.findWidget("Save/LastName") as Edit;
            fileNameTextBox = window.findWidget("Save/Filename") as Edit;
            locationTextBox = window.findWidget("Save/Location") as Edit;
            warningLabel = window.findWidget("Save/WarningLabel") as StaticText;
            warningImage = window.findWidget("Save/WarningImage") as StaticImage;
            saveButton = window.findWidget("Save/SaveButton") as Button;
            cancelButton = window.findWidget("Save/CancelButton") as Button;
            Button browseButton = window.findWidget("Save/BrowseButton") as Button;

            lastText.EventEditTextChange +=new MyGUIEvent(lastText_EventEditTextChange);
            firstText.EventEditTextChange += new MyGUIEvent(firstText_EventEditTextChange);
            fileNameTextBox.EventEditTextChange += new MyGUIEvent(fileNameTextBox_EventEditTextChange);
            locationTextBox.EventEditTextChange += new MyGUIEvent(locationTextBox_EventEditTextChange);
            warningLabel.Visible = false;
            warningImage.Visible = false;
            saveButton.MouseButtonClick += new MyGUIEvent(saveButton_MouseButtonClick);
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
        }

        public void save()
        {
            if (patientData == null)
            {
                this.open(true);
            }
            else
            {
                fireSaveFile();
            }
        }

        public void saveAs()
        {
            this.open(true);
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

        protected override void onShown(EventArgs e)
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
            base.onShown(e);
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
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

        private void overwriteMessageCallback(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                doSaveAndClose();
            }
        }

        private void doSaveAndClose()
        {
            patientData = new PatientDataFile(SavePath);
            patientData.FirstName = firstText.Caption;
            patientData.LastName = lastText.Caption;
            this.close();
            fireSaveFile();
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
            using (wx.DirDialog dirDialog = new wx.DirDialog(MainWindow.Instance, "Choose the path to load files from.", locationTextBox.Caption))
            {
                if (dirDialog.ShowModal() == wx.ShowModalResult.OK)
                {
                    locationTextBox.Caption = dirDialog.Path;
                }
            }
        }
    }
}
