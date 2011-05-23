using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;
using Medical.GUI;
using System.Runtime.InteropServices;
using System.Threading;
using Medical.Controller;

namespace Medical
{
    public class LicenseManager
    {
        /// <summary>
        /// This event will fire if the key is entered in the dialog sucessfully.
        /// </summary>
        public event EventHandler KeyValid;
        
        /// <summary>
        /// This event will fire if the key could not be entered.
        /// </summary>
        public event EventHandler KeyInvalid;

        /// <summary>
        /// This will be fired when the key dialog is shown.
        /// </summary>
        public event EventHandler KeyDialogShown;

        private delegate void MachineIDCallback(IntPtr value);
        private delegate void CallbackDelegate();
        private MachineIDCallback idCallback;
        private String machineID = null;
        private LicenseDialog licenseDialog;
        private UserPermissions userPermissions;
        private String keyFile;
        private String programName;
        private int productID;
        private CallbackDelegate keyValidCallback;
        private CallbackDelegate showKeyDialogCallback;

        public LicenseManager(String programName, String keyFile, int productID)
        {
            this.keyFile = keyFile;
            this.programName = programName;
            this.productID = productID;
            idCallback = new MachineIDCallback(getMachineIdCallback);
            keyValidCallback = new CallbackDelegate(fireKeyValid);
            showKeyDialogCallback = new CallbackDelegate(showKeyDialog);
        }

        public void getKey()
        {
            Thread t = new Thread(delegate()
            {
                userPermissions = new UserPermissions(keyFile, programName, getMachineId);
                //Temp
                if (userPermissions.Valid)
                {
                    ThreadManager.invoke(keyValidCallback);
                }
                else
                {
                    ThreadManager.invoke(showKeyDialogCallback);
                }
            });
            t.Start();
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

        /// <summary>
        /// If a license is invalid because it is expired this will be true.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return userPermissions.IsExpired;
            }
        }

        public String FeatureLevelString
        {
            get
            {
                return userPermissions.FeatureLevelString;
            }
        }

        public String LicenseeName
        {
            get
            {
                return userPermissions.LicenseeName;
            }
        }

        private void showKeyDialog()
        {
            licenseDialog = new LicenseDialog(userPermissions.ProgramName, getMachineId(), productID);
            licenseDialog.KeyEnteredSucessfully += new EventHandler(licenseDialog_KeyEnteredSucessfully);
            licenseDialog.KeyInvalid += new EventHandler(licenseDialog_KeyInvalid);
            licenseDialog.center();
            licenseDialog.open(true);
            if (KeyDialogShown != null)
            {
                KeyDialogShown.Invoke(this, EventArgs.Empty);
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
            if (userPermissions.Valid)
            {
                fireKeyValid();
            }
            else
            {
                showKeyDialog();
            }
        }

        private void fireKeyValid()
        {
            if (KeyValid != null)
            {
                KeyValid.Invoke(this, EventArgs.Empty);
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
