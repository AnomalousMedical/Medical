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
using System.Net.Http;
using System.Threading.Tasks;

namespace Medical
{
    public class ServerConnection
    {
        private List<Tuple<String, String>> arguments;
        private List<UploadFileInfo> fileStreams;

        /// <summary>
        /// This event is fired when this server connection gets a result that is not HttpStatusCode.OK
        /// when makeRequest is called. This also means that some sort of connection was established
        /// with the server, it will not be called if an exception is raised.
        /// </summary>
        public event Action<ServerConnection> NonOkResultEvent;

        static ServerConnection()
        {
            //Toggle security protocols without hopefully trashing new ones that are added.
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3; //Disable SSL v3.
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls; //Disable TLS 1.0.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; //Enable TLS 1.1 and 1.2

            ServicePointManager.ServerCertificateValidationCallback = checkValidationResult;
            DefaultTimeout = 60000;
        }

        /// <summary>
        /// This property controls whether TLS 1.0 is enabled (through ServicePointManager, although ideally this class is in charge of networking).
        /// This is unsafe but as of today 8-5-15 it does not appear that mono supports tls 1.1 and 1.2, so we need a way to enable this on mono
        /// based platforms.
        /// </summary>
        /// <value><c>true</c> to allow TLS 1.0 connections; otherwise, <c>false</c>.</value>
        public static bool EnableUnsafeTLS1_0
        {
            get
            {
                return (ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls) == SecurityProtocolType.Tls;
            }
            set
            {
                if (value)
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls; //Enable TLS 1.0.
                }
                else
                {
                    ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls; //Disable TLS 1.0.
                }
            }
        }

        public static int DefaultTimeout { get; set; }

        public ServerConnection(String url)
        {
            this.Url = url;
            Timeout = DefaultTimeout;
            ResponseStatusCode = HttpStatusCode.Unused;
        }

        public virtual void makeRequest(Action<HttpResponseMessage> response)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage();
            try
            {
                message.RequestUri = new Uri(Url);
                if (arguments != null)
                {
                    var content = new MultipartFormDataContent();
                    foreach (Tuple<String, String> arg in arguments)
                    {
                        content.Add(new StringContent(arg.Item2), arg.Item1);
                    }
                    message.Content = content;
                    message.Method = HttpMethod.Post;
                }
                else
                {
                    message.Method = HttpMethod.Get;
                }
                var t = client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

                //Get the response
                t.Wait();
                using (HttpResponseMessage httpResponse = t.Result)
                {
                    ResponseStatusCode = httpResponse.StatusCode;
                    if (ResponseStatusCode == HttpStatusCode.OK)
                    {
                        response(httpResponse);
                    }
                    else if (NonOkResultEvent != null)
                    {
                        NonOkResultEvent.Invoke(this);
                    }
                }
            }
            finally
            {
                if(message.Content != null)
                {
                    message.Content.Dispose();
                }
                message.Dispose();
                client.Dispose();
            }
        }

        public void makeRequestGetStream(Action<Stream> response)
        {
            makeRequest(new Action<HttpResponseMessage>(httpResponse =>
            {
                var task = httpResponse.Content.ReadAsStreamAsync();
                task.Wait();
                using (Stream responseStream = task.Result)
                {
                    response(responseStream);
                }
            }));
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

        public Saveable makeRequestSaveableResponse(TypeFinder typeFinder = null)
        {
            Saveable response = null;
            makeRequestGetStream(serverDataStream =>
                {
                    XmlSaver xmlSaver;
                    if (typeFinder == null)
                    {
                        xmlSaver = new XmlSaver();
                    }
                    else
                    {
                        xmlSaver = new XmlSaver(typeFinder);
                    }
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

            if (arguments != null)
            {
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToString());
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
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

        private Task<HttpResponseMessage> setupClientFormUrlEncoded(HttpClient client)
        {
            using (HttpRequestMessage message = new HttpRequestMessage())
            {
                message.RequestUri = new Uri(Url);
                if (arguments != null)
                {
                    using (MultipartFormDataContent content = new MultipartFormDataContent())
                    {
                        foreach (Tuple<String, String> arg in arguments)
                        {
                            content.Add(new StringContent(arg.Item2), arg.Item1);
                        }
                        message.Content = content;
                        message.Method = HttpMethod.Post;
                    }
                }
                else
                {
                    message.Method = HttpMethod.Get;
                }
                return client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
            }

        }

        private Task<HttpResponseMessage> setupClientFormMultipart(HttpClient client)
        {
            return client.GetAsync(new Uri(Url), HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
