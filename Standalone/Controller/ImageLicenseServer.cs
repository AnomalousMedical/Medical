using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Globalization;
using System.IO;
using Logging;
using System.Security.Cryptography;
using Medical.Controller;
using Engine.Threads;

namespace Medical
{
    public enum ImageLicenseType
    {
        Personal = 0,
        Commercial = 1
    }

    public class ImageLicenseServer
    {
        public delegate void LicenseCallback(bool success, bool promptStoreVisit, String message);
        public delegate void LicenseTextCallback(bool success, String message);

        private LicenseManager licenseManager;

        public ImageLicenseServer(LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
        }

        public void licenseImage(ImageLicenseType type, LicenseCallback callback)
        {
            LicensingImage = true;
            ThreadPool.QueueUserWorkItem(state =>
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    LicensingImage = false;
                    callback.Invoke(true, false, "");
                }));
            });
        }

        public bool LicensingImage { get; private set; }

        public bool ReadingLicenseText { get; private set; }
    }
}
