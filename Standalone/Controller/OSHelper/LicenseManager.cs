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
using Logging;

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

        private delegate void CallbackDelegate();
        private String keyFile;
        private String programName;
        private CallbackDelegate keyValidCallback;
        private CallbackDelegate showKeyDialogCallback;

        private AnomalousLicense license;
        private String keyDialogMessage = null;
        private String keyDialogUserName = null;

        public LicenseManager(String programName, String keyFile)
        {
            this.keyFile = keyFile;
            this.programName = programName;
            keyValidCallback = new CallbackDelegate(fireKeyValid);
            showKeyDialogCallback = new CallbackDelegate(showKeyDialog);
        }

        public void getKey()
        {
#if ALLOW_OVERRIDE
            if (MedicalConfig.Cracked)
            {
                Logging.Log.ImportantInfo("Running with no copy protection");
                ThreadManager.invoke(keyValidCallback);
                return;
            }
#endif
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
                                byte[] licenseBytes = licenseServer.createLicenseFile(license.User, license.Pass);
                                if (licenseBytes != null)
                                {
                                    storeLicenseFile(licenseBytes);
                                    license = new AnomalousLicense(licenseBytes);
                                }
                                else
                                {
                                    //Null the license. Something was not valid from the server.
                                    keyDialogUserName = license.User;
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
            t.IsBackground = true;
            t.Start();
        }

        public bool allowFeature(long featureCode)
        {
#if ALLOW_OVERRIDE
            if (MedicalConfig.Cracked)
            {
                return true;
            }

            if (featureCode == -1)
            {
                return true;
            }
#endif

            //0 is the main plugin
            //29 is the intro tutorial
            if (featureCode == 0 || featureCode == 29)
            {
                return true;
            }

            if (license != null)
            {
                return license.supportsFeature(featureCode);
            }
            return false;
        }

        /// <summary>
        /// This method can be used to get a new license from the server if the
        /// original is missing a feature or something that was added after the
        /// program started running.
        /// </summary>
        /// <returns></returns>
        public bool getNewLicense()
        {
            try
            {
                AnomalousLicenseServer licenseServer = new AnomalousLicenseServer(MedicalConfig.LicenseServerURL);
                byte[] licenseBytes = licenseServer.createLicenseFile(license.User, license.Pass);
                if (licenseBytes != null)
                {
                    storeLicenseFile(licenseBytes);
                    license = new AnomalousLicense(licenseBytes);
                }
            }
            catch (AnomalousLicenseServerException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void deleteLicense()
        {
#if ALLOW_OVERRIDE
            if (MedicalConfig.Cracked)
            {
                return;
            }
#endif
            try
            {
                File.Delete(keyFile);
            }
            catch (Exception)
            {

            }
        }

        public bool Valid
        {
            get
            {
#if ALLOW_OVERRIDE
                if (MedicalConfig.Cracked)
                {
                    return true;
                }
#endif
                return license != null;
            }
        }

        public String LicenseeName
        {
            get
            {
#if ALLOW_OVERRIDE
                if (MedicalConfig.Cracked)
                {
                    return "Anomalous Medical Internal";
                }
#endif
                return license != null ? license.LicenseeName : "Invalid";
            }
        }

        public string User
        {
            get
            {
#if ALLOW_OVERRIDE
                if (MedicalConfig.Cracked)
                {
                    return "AnomalousMedicalInternal";
                }
#endif
                return license != null ? license.User : "Invalid";
            }
        }

        public String MachinePassword
        {
            get
            {
                return license != null ? license.Pass : "";
            }
        }

        public String IdentifiedUserName
        {
            get
            {
                return keyDialogUserName;
            }
        }

        public String KeyDialogMessage
        {
            get
            {
                return keyDialogMessage;
            }
        }

        private void showKeyDialog()
        {
            if (KeyDialogShown != null)
            {
                KeyDialogShown.Invoke(this, EventArgs.Empty);
            }
        }

        public void keyInvalid()
        {
            if (KeyInvalid != null)
            {
                KeyInvalid.Invoke(this, EventArgs.Empty);
            }
        }

        public void keyEnteredSucessfully(byte[] licenseBytes)
        {
            storeLicenseFile(licenseBytes);
            license = new AnomalousLicense(licenseBytes);
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
    }
}
