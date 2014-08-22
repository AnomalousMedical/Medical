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
using System.Net;

namespace Medical
{
    public class LicenseManager
    {
        private String keyFile;

        private AnomalousLicense license;
        private String keyDialogMessage = null;
        private String keyDialogUserName = null;

        public LicenseManager(String keyFile)
        {
            this.keyFile = keyFile;
        }

        public void getKey(Action<bool> resultCallback)
        {
#if ALLOW_OVERRIDE
            if (MedicalConfig.Cracked)
            {
                Logging.Log.ImportantInfo("Running with no copy protection");
                ThreadManager.invoke(() => resultCallback(true));
                return;
            }
#endif
            ThreadPool.QueueUserWorkItem(state =>
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
                ThreadManager.invoke(() => resultCallback(Valid));
            });
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

        public bool allowPropUse(long? propId)
        {
            if(propId.HasValue)
            {
                if(license != null)
                {
                    return license.allowPropUse(propId.Value);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
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
                return login(license.User, license.Pass);
            }
            catch (AnomalousLicenseServerException)
            {
                return false;
            }
        }

        /// <summary>
        /// Login to the server with the given username and password. Returns true if the login was correct, false if the user / pass is incorrect.
        /// Throws an AnomalousLicenseServerException if something goes wrong when logging in.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns>True if login was correct, false if it is incorrect.</returns>
        /// <exception cref="AnomalousLicenseServerException">If something goes wrong connecting to the server.</exception>
        public bool login(String user, String pass)
        {
            try
            {
                AnomalousLicenseServer licenseServer = new AnomalousLicenseServer(MedicalConfig.LicenseServerURL);
                byte[] licenseBytes = licenseServer.createLicenseFile(user, pass);
                if (licenseBytes != null)
                {
                    storeLicenseFile(licenseBytes);
                    license = new AnomalousLicense(licenseBytes);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (AnomalousLicenseServerException alse)
            {
                throw alse;
            }
            catch (WebException webException)
            {
                switch (webException.Status)
                {
                    case WebExceptionStatus.TrustFailure:
                        throw new AnomalousLicenseServerException("Error establishing SSL trust relationship with server. Please try again on another internet connection.");
                    case WebExceptionStatus.ConnectFailure:
                        throw new AnomalousLicenseServerException("Cannot connect to License Server. Please try again later.");
                    case WebExceptionStatus.NameResolutionFailure:
                        throw new AnomalousLicenseServerException("Could not find host name. Please try again later.");
                    case WebExceptionStatus.Timeout:
                        throw new AnomalousLicenseServerException("Connection to license server timed out. Please try again later.");
                    default:
                        throw new AnomalousLicenseServerException(String.Format("An undefined error occured connecting to the license server. Please try again later. The status was {0}.", webException.Status));
                }
            }
            catch (LicenseInvalidException ex)
            {
                throw new AnomalousLicenseServerException(String.Format("License returned from server is invalid.\nReason: {0}\nPlease contact support at CustomerService@AnomalousMedical.com.", ex.Message));
            }
            catch (Exception e)
            {
                throw new AnomalousLicenseServerException(String.Format("Could not connect to license server. Please try again later.\nReason: {0}", e.Message));
            }
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
    }
}
