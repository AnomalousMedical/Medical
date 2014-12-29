﻿using System;
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
                bool success = false;
                bool promptStoreVisit = false;
                String message = "Error reading data from server.";
                try
                {
                    CredentialServerConnection serverConnection = new CredentialServerConnection(MedicalConfig.LicenseImageURL, licenseManager.User, licenseManager.MachinePassword);
                    serverConnection.addArgument("LicenseType", ((int)type).ToString());
                    serverConnection.makeRequestGetStream(responseStream =>
                        {
                            using (BinaryReader serverDataStream = new BinaryReader(responseStream))
                            {
                                int signatureLength = serverDataStream.ReadInt32();
                                byte[] signature = serverDataStream.ReadBytes(signatureLength);
                                int serverResponseLength = serverDataStream.ReadInt32();
                                byte[] serverResponse = serverDataStream.ReadBytes(serverResponseLength);

                                if (CertificateStoreManager.IsValidServerCommunication(serverResponse, signature))
                                {
                                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(serverResponse)))
                                    {
                                        success = binaryReader.ReadBoolean();
                                        message = binaryReader.ReadString();
                                        promptStoreVisit = true;
                                    }
                                }
                                else
                                {
                                    message = "Signature mismatch from server. Image not licensed.";
                                }
                            }
                        });
                }
                catch (Exception e)
                {
                    message = String.Format("Could not license image from server.\nReason: {0}.", e.Message);
                    Log.Error(message);
                }
                ThreadManager.invoke(new Action(delegate()
                {
                    LicensingImage = false;
                    callback.Invoke(success, promptStoreVisit, message);
                }));
            });
        }

        public void getLicenseFromServer(ImageLicenseType licenseType, LicenseTextCallback callback)
        {
            ReadingLicenseText = true;
            ThreadPool.QueueUserWorkItem(state =>
            {
                bool success = false;
                String message = "Error reading license from server";
                try
                {
                    ServerConnection serverConnection = new ServerConnection(MedicalConfig.LicenseReaderURL);
                    serverConnection.addArgument("Type", LicenseReadType.Image.ToString());
                    serverConnection.addArgument("Id", ((int)licenseType).ToString());
                    serverConnection.makeRequestGetStream(responseStream =>
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            success = true;
                            message = streamReader.ReadToEnd();
                        }
                    });
                }
                catch (Exception e)
                {
                    message = String.Format("Could not read license data from server.\nReason: {0}.", e.Message);
                    Log.Error(message);
                }
                ThreadManager.invoke(new Action(delegate()
                {
                    ReadingLicenseText = false;
                    callback.Invoke(success, message);
                }));
            });
        }

        public String LicenseeName
        {
            get
            {
                return licenseManager.LicenseeName;
            }
        }

        public bool LicensingImage { get; private set; }

        public bool ReadingLicenseText { get; private set; }
    }
}
