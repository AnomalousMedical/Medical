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

        public static Func<HttpClient> HttpClientProvider;

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

            DefaultTimeout = 60000;

            HttpClientProvider = () => new HttpClient();
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
            HttpClient client = HttpClientProvider();
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            MultipartFormDataContent content = null;
            try
            {
                message.RequestUri = new Uri(Url);
                if (arguments != null)
                {
                    content = new MultipartFormDataContent();
                    foreach (Tuple<String, String> arg in arguments)
                    {
                        content.Add(new StringContent(arg.Item2), arg.Item1);
                    }
                    message.Content = content;
                    message.Method = HttpMethod.Post;
                }
                if (fileStreams != null)
                {
                    if (content == null)
                    {
                        content = new MultipartFormDataContent();
                        message.Content = content;
                        message.Method = HttpMethod.Post;
                    }
                    foreach (var item in fileStreams)
                    {
                        var streamContent = new StreamContent(item.Stream, (int)item.Stream.Length);
                        streamContent.Headers.Add("Content-Type", item.ContentType);
                        content.Add(streamContent, item.Key, item.FileName);
                    }
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
            catch(AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1)
                {
                    throw ex.InnerException;
                }
                throw;
            }
            finally
            {
                if (content != null)
                {
                    content.Dispose();
                }
                try
                {
                    message.Dispose();
                }
                catch(NullReferenceException) { } //Ignore the null ref exceptions thrown on some platforms when disposing (ModernHttpClient on Android).
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
    }
}
