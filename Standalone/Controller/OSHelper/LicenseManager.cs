using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;
using Medical.GUI;
using System.Runtime.InteropServices;

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

        private delegate void MachineIDCallback(IntPtr value);
        private MachineIDCallback idCallback;
        private String machineID = null;
        private LicenseDialog licenseDialog;
        private UserPermissions userPermissions;
        private String keyFile;
        private String programName;

        public LicenseManager(String programName, String keyFile)
        {
            this.keyFile = keyFile;
            this.programName = programName;
            idCallback = new MachineIDCallback(getMachineIdCallback);
            userPermissions = new UserPermissions(keyFile, programName, getMachineId);
        }

        public void showKeyDialog(int productID)
        {
            licenseDialog = new LicenseDialog(userPermissions.ProgramName, getMachineId(), productID);
            licenseDialog.KeyEnteredSucessfully += new EventHandler(licenseDialog_KeyEnteredSucessfully);
            licenseDialog.KeyInvalid += new EventHandler(licenseDialog_KeyInvalid);
            licenseDialog.center();
            licenseDialog.open(true);
        }

        public bool allowFeature(int featureCode)
        {
            return userPermissions.allowFeature(featureCode);
        }

        public String Key
        {
            get
            {
                return userPermissions.ProductKey;
            }
        }

        public bool KeyValid
        {
            get
            {
                return userPermissions.Valid;
            }
        }

        public String FeatureLevelString
        {
            get
            {
                return userPermissions.FeatureLevelString;
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
            using (Stream fileStream = new FileStream(keyFile, FileMode.Create))
            {
                fileStream.Write(licenseDialog.License, 0, licenseDialog.License.Length);
            }
            userPermissions = new UserPermissions(keyFile, programName, getMachineId);
            licenseDialog.Dispose();
            if (KeyEnteredSucessfully != null)
            {
                KeyEnteredSucessfully.Invoke(this, EventArgs.Empty);
            }
        }

        private String getMachineId()
        {
            if (machineID == null)
            {
                LicenseManager_getMachineID(idCallback);
            }
            return machineID;
        }

        private void getMachineIdCallback(IntPtr value)
        {
            machineID = Marshal.PtrToStringAnsi(value);
        }

        [DllImport("OSHelper")]
        private static extern void LicenseManager_getMachineID(MachineIDCallback callback);
    }
}
