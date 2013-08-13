using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;
using System.Globalization;
using Medical;

namespace Engine
{
    internal class AnomalousLicenseServer
    {
        private String baseURL;
        private static byte[] ERROR_RESPOSE_BYTES = Encoding.UTF8.GetBytes("ANOMALOUS_RESPOSE|");

        public AnomalousLicenseServer(String baseURL)
        {
            this.baseURL = baseURL;
        }

        public byte[] createLicenseFile(String user, String pass)
        {
            byte[] licenseBytes = null;

            CredentialServerConnection serverConnection = new CredentialServerConnection(baseURL, user, pass);
            serverConnection.makeRequestGetStream(dataStream =>
                {
                    using (Stream licenseStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[8 * 1024];
                        int len;
                        while ((len = dataStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            licenseStream.Write(buffer, 0, len);
                        }
                        licenseStream.Seek(0, SeekOrigin.Begin);
                        licenseBytes = new byte[licenseStream.Length];
                        licenseStream.Read(licenseBytes, 0, licenseBytes.Length);
                        bool errorMatch = true;
                        for (int i = 0; i < ERROR_RESPOSE_BYTES.Length && errorMatch; ++i)
                        {
                            errorMatch = licenseBytes[i] == ERROR_RESPOSE_BYTES[i];
                        }
                        if (errorMatch)
                        {
                            throw new AnomalousLicenseServerException(Encoding.UTF8.GetString(licenseBytes, ERROR_RESPOSE_BYTES.Length, licenseBytes.Length - ERROR_RESPOSE_BYTES.Length));
                        }
                    }
                });
            return licenseBytes;
        }
    }
}
