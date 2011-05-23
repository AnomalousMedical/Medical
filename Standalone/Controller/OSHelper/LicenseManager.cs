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
        private String keyFile;
        private String programName;
        private int productID;
        private CallbackDelegate keyValidCallback;
        private CallbackDelegate showKeyDialogCallback;

        private AnomalousLicense license;

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
                if (File.Exists(keyFile))
                {
                    if (MedicalConfig.StoreCredentials)
                    {
                        using (Stream stream = File.OpenRead(keyFile))
                        {
                            try
                            {
                                byte[] fileContents = new byte[stream.Length];
                                stream.Read(fileContents, 0, fileContents.Length);
                                license = new AnomalousLicense(fileContents);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            File.Delete(keyFile);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                //Check validity
                if (Valid)
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
            return license != null ? license.supportsFeature(featureCode) : false;
        }

        public String Key
        {
            get
            {
                return license != null ? license.ProductKey : "None";
            }
        }

        /// <summary>
        /// If a license is invalid because it is expired this will be true.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return license != null ? license.IsExpired : false;
            }
        }

        public bool Valid
        {
            get
            {
                return license != null && !license.IsExpired && license.MachineID == getMachineId() && KeyChecker.checkValid(programName, license.ProductKey);
            }
        }

        public String FeatureLevelString
        {
            get
            {
                return license != null ? license.FeatureLevel : "";
            }
        }

        public String LicenseeName
        {
            get
            {
                return license != null ? license.LicenseeName : "Invalid";
            }
        }

        private void showKeyDialog()
        {
            licenseDialog = new LicenseDialog(programName, getMachineId(), productID);
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
            if (MedicalConfig.StoreCredentials)
            {
                using (Stream fileStream = new FileStream(keyFile, FileMode.Create))
                {
                    fileStream.Write(licenseDialog.License, 0, licenseDialog.License.Length);
                }
            }
            license = new AnomalousLicense(licenseDialog.License);
            licenseDialog.Dispose();
            fireKeyValid();
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
