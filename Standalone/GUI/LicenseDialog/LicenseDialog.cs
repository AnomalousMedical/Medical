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

        private Edit keyEdit;
        private string programName;

        public LicenseDialog(String programName)
            :base("Medical.GUI.LicenseDialog.LicenseDialog.layout")
        {
            this.programName = programName;

            Widget prompt = window.findWidget("Prompt");
            prompt.Caption = prompt.Caption.Replace("$(PROGRAM_NAME)", programName);

            keyEdit = window.findWidget("KeyText") as Edit;

            Button validateButton = window.findWidget("ValidateButton") as Button;
            validateButton.MouseButtonClick += new MyGUIEvent(validateButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        public String Key
        {
            get
            {
                return keyEdit.OnlyText;
            }
        }

        void validateButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (KeyChecker.checkValid(programName, keyEdit.OnlyText))
            {
                this.close();
                if (KeyEnteredSucessfully != null)
                {
                    KeyEnteredSucessfully.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                MessageBox.show("The key you entered is invalid.", "Invalid Key", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
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
