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
        private static Object validateServerLock = new Object();
        private static bool trustServerConnections = false;

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
            if (!trustServerConnections)
            {
                lock (validateServerLock)
                {
                    //Double check this in lock, can skip if this has already been derived
                    if (!trustServerConnections)
                    {
                        trustServerConnections = PlatformConfig.TrustServerConnections;
                    }
                }
            }
            StringBuilder postData = new StringBuilder();
            if (arguments != null)
            {
                String argumentFormat = "{0}={1}";
                foreach (Tuple<String, String> arg in arguments)
                {
                    postData.AppendFormat(CultureInfo.InvariantCulture, argumentFormat, Uri.EscapeDataString(arg.Item1), Uri.EscapeDataString(arg.Item2));
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
            //This works by trusting a connection to a page on the anomalous medical website (specified by whatever the host passed to the program is).
            //If that connection is valid rely on the default runtime checking. If not then override the default runtime checking.
            //This is useful because mono seems to accept every connection no matter what, so on the mac platform we do an extra ping to the website
            //using cocoa's method of connecting. If that proves to be valid, then we assume the connections are safe.
            //
            //This still leaves open the possiblity that other servers are compromised, but in our case the only other servers we are connecting to are the
            //windows azure servers, and those should be ok.
            //
            //Likely a much better solution exists for this problem, but this method will ensure that at least some kind of check is done to make sure the
            //anomalousmedical.com server is who it says it is. That is the only server we send credentials to at this point.
			return trustServerConnections && sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}
