using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;

namespace Medical
{
    public class ServerConnection
    {
        private List<Tuple<String, String>> arguments;
        private List<UploadFileInfo> fileStreams;

        static ServerConnection()
        {
            ServicePointManager.ServerCertificateValidationCallback = checkValidationResult;
            DefaultTimeout = 60000;
        }

        public static int DefaultTimeout { get; set; }

        public ServerConnection(String url)
        {
            this.Url = url;
            Timeout = DefaultTimeout;
            ResponseStatusCode = HttpStatusCode.Unused;
        }

        public virtual void makeRequest(Action<HttpWebResponse> response)
        {
            HttpWebRequest request;
            if (fileStreams != null)
            {
                request = buildRequestFormMultipart();
            }
            else
            {
                request = buildRequestFormUrlEncoded();
            }

            // Get the response.
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            {
                ResponseStatusCode = webResponse.StatusCode;
                if (ResponseStatusCode == HttpStatusCode.OK)
                {
                    response(webResponse);
                }
            }
        }

        public void makeRequestGetStream(Action<Stream> response)
        {
            makeRequest(webResponse =>
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        response(responseStream);
                    }
                });
        }

        public void makeRequestDownloadResponse(Action<MemoryStream> response)
        {
            makeRequestGetStream(serverDataStream =>
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

        public Saveable makeRequestSaveableResponse()
        {
            Saveable response = null;
            makeRequestGetStream(serverDataStream =>
                {
                    XmlSaver xmlSaver = new XmlSaver();
                    using (XmlReader xmlReader = new XmlTextReader(serverDataStream))
                    {
                        response = xmlSaver.restoreObject(xmlReader) as Saveable;
                    }
                });
            return response;
        }

        public void addArgument(String key, String value)
        {
            if (arguments == null)
            {
                arguments = new List<Tuple<string, string>>();
            }
            arguments.Add(Tuple.Create(key, value));
        }

        /// <summary>
        /// Add a stream to this request. The stream will be closed when the request is made.
        /// </summary>
        /// <param name="key">The key for the stream.</param>
        /// <param name="fileName">The file name for the stream.</param>
        /// <param name="stream">The stream.</param>
        public void addFileStream(String key, Stream stream, String contentType = null, String fileName = null)
        {
            if (fileStreams == null)
            {
                fileStreams = new List<UploadFileInfo>();
            }
            fileStreams.Add(new UploadFileInfo()
            {
                Key = key,
                Stream = stream,
                ContentType = contentType != null ? contentType : "application/octet-stream",
                FileName = fileName != null ? fileName : "__AutoFileName" + fileStreams.Count
            });
        }

        public int Timeout { get; set; }

        public String Url { get; set; }

        public HttpStatusCode ResponseStatusCode { get; set; }

        /// <summary>
        /// Verify the validation result either using the default based on sslPolicyErrors or a custom function for a given os. This depends on
        /// the value of PlatformConfig.HasCustomSSLValidation.
        /// </summary>
        /// <returns>True if the cert is valid. False otherwise.</returns>
        private static bool checkValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (PlatformConfig.HasCustomSSLValidation)
            {
                String host = null;
                if (sender is HttpWebRequest)
                {
                    host = ((HttpWebRequest)sender).Host;
                }
                else if (sender is String)
                {
                    host = sender.ToString();
                }

                if (host != null)
                {
                    bool trusted = PlatformConfig.TrustSSLCertificate(certificate, host);
					if (!trusted) 
					{
						Logging.Log.Error("Could not trust ssl certificate with subject '{0}' for host '{1}'. Connections to this server will not be possible", certificate.Subject, host);
					}
					return trusted;
                }
                else
                {
					Logging.Log.Error("Host not specified when validating ssl certificate with subject '{0}'. Connections to this server will not be possible", certificate.Subject, host);
                    return false; //If we cannot check with the hosts, we just want to fail.
                }
            }
            else
            {
                return sslPolicyErrors == SslPolicyErrors.None;
            }
        }

        private HttpWebRequest buildRequestFormUrlEncoded()
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

            return request;
        }

        private HttpWebRequest buildRequestFormMultipart()
        {
            List<Mime> parts = new List<Mime>();
            if (arguments != null)
            {
                foreach (var arg in arguments)
                {
                    Mime part = new Mime();
                    part.Headers["Content-Disposition"] = String.Format("form-data; name=\"{0}\"", arg.Item1);
                    part.Data = new MemoryStream(Encoding.UTF8.GetBytes(arg.Item2));
                    parts.Add(part);
                }
            }

            if (fileStreams != null)
            {
                foreach (var file in fileStreams)
                {
                    Mime part = new Mime();

                    part.Headers["Content-Disposition"] = String.Format("form-data; name=\"{0}\"; filename=\"{1}\"", file.Key, file.FileName);
                    part.Headers["Content-Type"] = file.ContentType;

                    part.Data = file.Stream;

                    parts.Add(part);
                }
            }

            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(Url));
            request.Timeout = Timeout;
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary; ;

            byte[] footer = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
            long contentLength = footer.Length;

            foreach (Mime part in parts)
            {
                contentLength += part.GenerateHeader(boundary);
            }

            request.ContentLength = contentLength;

            byte[] buffer = new byte[8192];
            byte[] afterFile = Encoding.UTF8.GetBytes("\r\n");
            int read;
            using (Stream dataStream = request.GetRequestStream())
            {
                foreach (Mime part in parts)
                {
                    dataStream.Write(part.Header, 0, part.Header.Length);

                    while ((read = part.Data.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataStream.Write(buffer, 0, read);
                    }

                    part.Dispose();

                    dataStream.Write(afterFile, 0, afterFile.Length);
                }

                dataStream.Write(footer, 0, footer.Length);
            }

            return request;
        }
    }
}
