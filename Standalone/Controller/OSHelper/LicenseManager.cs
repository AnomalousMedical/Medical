//#define CRACKED


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
        private String keyDialogMessage = null;

        public LicenseManager(String programName, String keyFile, int productID)
        {
            this.keyFile = keyFile;
            this.programName = programName;
            this.productID = productID;
            idCallback = new MachineIDCallback(getMachineIdCallback);
            keyValidCallback = new CallbackDelegate(fireKeyValid);
            showKeyDialogCallback = new CallbackDelegate(showKeyDialog);
            getMachineId();
        }

        public void getKey()
        {
#if CRACKED
            Logging.Log.ImportantInfo("Running with no copy protection");
            ThreadManager.invoke(keyValidCallback);
#else
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
                                //Load old license
                                byte[] fileContents = new byte[stream.Length];
                                stream.Read(fileContents, 0, fileContents.Length);
                                license = new AnomalousLicense(fileContents);
                            }
                            catch (Exception)
                            {
                                license = null;
                                keyDialogMessage = "Local license corrupted. Please sign in again.";
                            }
                        }
                        //Validate credentials by getting the license from the server again
                        if (license != null)
                        {
                            try
                            {
                                AnomalousLicenseServer licenseServer = new AnomalousLicenseServer(MedicalConfig.LicenseServerURL);
                                byte[] licenseBytes = licenseServer.createLicenseFile(license.User, license.Pass, machineID, productID);
                                if (licenseBytes != null)
                                {
                                    storeLicenseFile(licenseBytes);
                                    license = new AnomalousLicense(licenseBytes);
                                }
                                else
                                {
                                    //Null the license. Something was not valid from the server.
                                    keyDialogMessage = "License has expired. Please log in again.";
                                    license = null;

                                    //Delete the old license
                                    deleteLicense();
                                }
                            }
                            catch (AnomalousLicenseServerException alse)
                            {
                                //Null the license. Something was not valid from the server.
                                keyDialogMessage = String.Format("License has expired. Reason {0}. Please log in again.", alse.Message);
                                license = null;

                                //Delete the old license
                                deleteLicense();
                            }
                            catch (Exception)
                            {
                                //Do Nothing, allow license that was loaded to be used.
                            }
                        }
                    }
                    else
                    {
                        deleteLicense();
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
#endif
        }

        public bool allowFeature(long featureCode)
        {
#if CRACKED
            return true;
#else
            return license != null ? license.supportsFeature(featureCode) : false;
#endif
        }

        public void deleteLicense()
        {
#if !CRACKED
            try
            {
                File.Delete(keyFile);
            }
            catch (Exception)
            {

            }
#endif
        }

        public bool Valid
        {
            get
            {
#if CRACKED
                return true;
#else
                return license != null && license.MachineID == getMachineId();
#endif
            }
        }

        public String LicenseeName
        {
            get
            {
#if CRACKED
                return "Anomalous Medical Internal";
#else
                return license != null ? license.LicenseeName : "Invalid";
#endif
            }
        }

        public string User
        {
            get
            {
#if CRACKED
                return "AnomalousMedicalInternal";
#else
                return license != null ? license.User : "Invalid";
#endif
            }
        }

        public String MachinePassword
        {
            get
            {
                return license != null ? license.Pass : "";
            }
        }

        private void showKeyDialog()
        {
            licenseDialog = new LicenseDialog(programName, getMachineId(), productID, keyDialogMessage);
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
            storeLicenseFile(licenseDialog.License);
            license = new AnomalousLicense(licenseDialog.License);
            licenseDialog.Dispose();
            fireKeyValid();
        }

        private void storeLicenseFile(byte[] license)
        {
            if (MedicalConfig.StoreCredentials)
            {
                using (Stream fileStream = new FileStream(keyFile, FileMode.Create))
                {
                    fileStream.Write(license, 0, license.Length);
                }
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

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void LicenseManager_getMachineID(MachineIDCallback callback);
    }
}
