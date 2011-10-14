using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace Engine
{
    class AnomalousLicenseServerException : Exception
    {
        public AnomalousLicenseServerException(String message)
            : base(message)
        {

        }
    }

    public class AnomalousLicenseServer
    {
        private String baseURL;
        private static byte[] ERROR_RESPOSE_BYTES = Encoding.UTF8.GetBytes("ANOMALOUS_RESPOSE|");

        public AnomalousLicenseServer(String baseURL)
        {
            this.baseURL = baseURL;
        }

        public byte[] createLicenseFile(String user, String pass, String machineID)
        {
            byte[] licenseBytes = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(baseURL));
            request.Timeout = 10000;
            request.Method = "POST";
            String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}&machineID={2}", user, pass, machineID);
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
                    using (Stream dataStream = response.GetResponseStream())
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
                    }
                }
            }
            return licenseBytes;
        }
    }
}
