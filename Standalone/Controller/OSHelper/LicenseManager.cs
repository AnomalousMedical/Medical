using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;
using Medical.GUI;

namespace Medical
{
    public class LicenseManager
    {
        /// <summary>
        /// This event will fire if the key is entered in the dialog sucessfully.
        /// </summary>
        public event EventHandler KeyEnteredSucessfully;
        
        /// <summary>
        /// This event will fire if the key could not be entered.
        /// </summary>
        public event EventHandler KeyInvalid;

        private LicenseDialog licenseDialog;
        private String programName;
        private String keyFile;
        private String key;

        public LicenseManager(String programName, String keyFile)
        {
            this.programName = programName;
            this.keyFile = keyFile;

            KeyValid = false;
            key = "";
            if (File.Exists(keyFile))
            {
                using (StreamReader stream = new StreamReader(File.OpenRead(keyFile)))
                {
                    key = stream.ReadLine();
                    KeyValid = KeyChecker.checkValid(programName, key);
                }
            }
        }

        public void showKeyDialog()
        {
            licenseDialog = new LicenseDialog(programName);
            licenseDialog.KeyEnteredSucessfully += new EventHandler(licenseDialog_KeyEnteredSucessfully);
            licenseDialog.KeyInvalid += new EventHandler(licenseDialog_KeyInvalid);
            licenseDialog.center();
            licenseDialog.open(true);
        }

        public String Key
        {
            get
            {
                return key;
            }
        }

        void licenseDialog_KeyInvalid(object sender, EventArgs e)
        {
            licenseDialog.Dispose();
            if (KeyInvalid != null)
            {
                KeyInvalid.Invoke(this, EventArgs.Empty);
            }
        }

        void licenseDialog_KeyEnteredSucessfully(object sender, EventArgs e)
        {
            key = licenseDialog.Key;
            using (StreamWriter fileStream = new StreamWriter(new FileStream(keyFile, FileMode.Create)))
            {
                fileStream.WriteLine(key);
            }
            licenseDialog.Dispose();
            if (KeyEnteredSucessfully != null)
            {
                KeyEnteredSucessfully.Invoke(this, EventArgs.Empty);
            }
        }

        public bool KeyValid { get; private set; }
    }
}
