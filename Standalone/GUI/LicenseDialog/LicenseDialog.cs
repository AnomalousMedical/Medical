using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class LicenseDialog : Dialog
    {
        /// <summary>
        /// This event will fire if the key is entered in the dialog sucessfully.
        /// </summary>
        public event EventHandler KeyEnteredSucessfully;

        /// <summary>
        /// This event will fire if the key could not be entered.
        /// </summary>
        public event EventHandler KeyInvalid;

        private Edit userEdit;
        private Edit passwordEdit;
        private string programName;
        private String machineID;
        private int productID;

        public LicenseDialog(String programName, String machineID, int productID)
            :base("Medical.GUI.LicenseDialog.LicenseDialog.layout")
        {
            this.programName = programName;
            this.machineID = machineID;
            this.productID = productID;

            Widget prompt = window.findWidget("Prompt");
            prompt.Caption = prompt.Caption.Replace("$(PROGRAM_NAME)", programName);

            userEdit = window.findWidget("UserText") as Edit;
            passwordEdit = window.findWidget("PasswordText") as Edit;

            Button activateButton = window.findWidget("ActivateButton") as Button;
            activateButton.MouseButtonClick += new MyGUIEvent(activateButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        public byte[] License { get; private set; }

        void activateButton_MouseButtonClick(Widget source, EventArgs e)
        {
            AnomalousLicenseServer licenseServer = new AnomalousLicenseServer(MedicalConfig.LicenseServerURL);
            License = licenseServer.createLicenseFile(userEdit.OnlyText, passwordEdit.OnlyText, machineID, productID);
            if (License != null)
            {
                this.close();
                if (KeyEnteredSucessfully != null)
                {
                    KeyEnteredSucessfully.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                MessageBox.show("Could not get license file. Please try again later.", "Server Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
            if (KeyInvalid != null)
            {
                KeyInvalid.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
