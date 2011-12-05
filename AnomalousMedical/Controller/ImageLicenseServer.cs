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
            Thread readLicenseThread = new Thread(delegate()
            {
                bool success = false;
                bool promptStoreVisit = false;
                String message = "";
                try
                {
                    String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}&licenseType={2}", licenseManager.User, licenseManager.MachinePassword, (int)type);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.LicenseImageURL));
                    request.Timeout = 60000;
                    request.Method = "POST";
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";

                    request.ContentLength = byteArray.Length;
                    using (Stream dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }

                    // Get the response.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                        {
                            using (BinaryReader serverDataStream = new BinaryReader(response.GetResponseStream()))
                            {
                                int signatureLength = serverDataStream.ReadInt32();
                                byte[] signature = serverDataStream.ReadBytes(signatureLength);
                                int serverResponseLength = serverDataStream.ReadInt32();
                                byte[] serverResponse = serverDataStream.ReadBytes(serverResponseLength);

                                RSACryptoServiceProvider decrypt = new RSACryptoServiceProvider();
                                decrypt.FromXmlString(MedicalConfig.ServerPublicKey);
                                if (decrypt.VerifyData(serverResponse, new SHA1CryptoServiceProvider(), signature))
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
                        }
                        else
                        {
                            message = "Error reading data from server.";
                        }
                    }
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
            readLicenseThread.Start();
        }

        public void getLicenseFromServer(ImageLicenseType licenseType, LicenseTextCallback callback)
        {
            ReadingLicenseText = true;
            Thread readLicenseThread = new Thread(delegate()
            {
                bool success = false;
                String message = "";
                try
                {
                    String postData = String.Format(CultureInfo.InvariantCulture, "ImageLicenseId={0}", (int)licenseType);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.LicenseReaderURL));
                    request.Timeout = 60000;
                    request.Method = "POST";
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";

                    request.ContentLength = byteArray.Length;
                    using (Stream dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }

                    // Get the response.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                        {
                            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                            {
                                success = true;
                                message = streamReader.ReadToEnd();
                            }
                        }
                        else
                        {
                            message = "Error reading license from server";
                        }
                    }
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
            readLicenseThread.Start();
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
