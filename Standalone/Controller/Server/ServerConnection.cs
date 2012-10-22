using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Medical
{
    public class ServerConnection
    {
        private List<Tuple<String, String>> arguments;

        static ServerConnection()
        {
            ServicePointManager.ServerCertificateValidationCallback = checkValidationResult;
        }

        public ServerConnection(String url)
        {
            this.Url = url;
            Timeout = 10000;
        }

        public virtual void makeRequest(Action<HttpWebResponse> response)
        {
            StringBuilder postData = new StringBuilder();
            if (arguments != null)
            {
                String argumentFormat = "{0}={1}";
                foreach (Tuple<String, String> arg in arguments)
                {
                    postData.AppendFormat(CultureInfo.InvariantCulture, argumentFormat, arg.Item1, Uri.EscapeDataString(arg.Item2));
                    argumentFormat = "&{0}={1}";
                }
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(Url));
            request.Timeout = Timeout;
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToString());
            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            // Get the response.
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            {
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    response(webResponse);
                }
            }
        }

        public void makeRequest(Action<Stream> response)
        {
            makeRequest(webResponse =>
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        response(responseStream);
                    }
                });
        }

        public void makeRequestDownloadResponse(Action<Stream> response)
        {
            makeRequest(serverDataStream =>
                {
                    using (MemoryStream localDataStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[8 * 1024];
                        int len;
                        while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            localDataStream.Write(buffer, 0, len);
                        }
                        localDataStream.Seek(0, SeekOrigin.Begin);
                        response(localDataStream);
                    }
                });
        }

        public void addArgument(String key, String value)
        {
            if (arguments == null)
            {
                arguments = new List<Tuple<string, string>>();
            }
            arguments.Add(Tuple.Create(key, value));
        }

        public int Timeout { get; set; }

        public String Url { get; set; }

        private static bool checkValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
