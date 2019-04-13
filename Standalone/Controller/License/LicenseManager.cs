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
using Engine.Threads;

namespace Medical
{
    public class LicenseManager
    {
        private String keyFile;

        public LicenseManager(String keyFile)
        {
            this.keyFile = keyFile;
        }

        public void getKey(Action<bool> resultCallback)
        {
            ThreadManager.invoke(() => resultCallback(true));
        }

        public bool allowFeature(long featureCode)
        {
            return true;
        }

        public bool allowPropUse(long? propId)
        {
            return true;
        }

        /// <summary>
        /// This method can be used to get a new license from the server if the
        /// original is missing a feature or something that was added after the
        /// program started running.
        /// </summary>
        /// <returns></returns>
        public bool getNewLicense()
        {
            return true;
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
            return true;
        }

        public void deleteLicense()
        {

        }

        public bool Valid
        {
            get
            {
                return true;
            }
        }

        public string User
        {
            get
            {
                return "User";
            }
        }

        public String MachinePassword
        {
            get
            {
                return "";
            }
        }

        public String IdentifiedUserName
        {
            get
            {
                return "";
            }
        }

        public String KeyDialogMessage
        {
            get
            {
                return "";
            }
        }
    }
}
