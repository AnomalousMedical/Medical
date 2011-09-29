using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using Medical.Controller;
using System.Globalization;
using Engine;
using MyGUIPlugin;
using Logging;

namespace Medical
{
    public delegate void DownloadCallback(Download download);

    public class DownloadController : IDisposable
    {
        private LicenseManager licenseManager;
        private AtlasPluginManager pluginManager;

        public DownloadController(LicenseManager licenseManager, AtlasPluginManager pluginManager)
        {
            this.licenseManager = licenseManager;
            this.pluginManager = pluginManager;
        }

        public void Dispose()
        {

        }

        public void downloadPlugin(int pluginId, DownloadCallback statusUpdate, DownloadCallback downloadCompleted, Object downloadCallbackObject)
        {
            PluginDownload pluginDownload = new PluginDownload(pluginId, statusUpdate, downloadCompleted, licenseManager, pluginManager);
            pluginDownload.UserObject = downloadCallbackObject;
            Thread t = new Thread(genericBackgroundDownload);
            t.Start(pluginDownload);
        }

        void genericBackgroundDownload(Object downloadObject)
        {
            Download download = (Download)downloadObject;
            bool success = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.PluginDownloadURL));
                request.Timeout = 10000;
                request.Method = "POST";
                String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}&type={2}&{3}", licenseManager.User, licenseManager.MachinePassword, download.Type, download.AdditionalArgs);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";

                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    using (Stream serverDataStream = response.GetResponseStream())
                    {
                        download.FileName = response.Headers["content-disposition"].Substring(21);
                        String sizeStr = response.Headers["Content-Length"];
                        download.TotalSize = NumberParser.ParseLong(sizeStr);
                        String pluginFileLocation = Path.Combine(download.DestinationFolder, download.FileName);
                        using (Stream localDataStream = new FileStream(pluginFileLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            byte[] buffer = new byte[8 * 1024];
                            int len;
                            download.TotalRead = 0;
                            while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                download.TotalRead += len;
                                localDataStream.Write(buffer, 0, len);
                                download.updateStatus();
                            }
                        }

                        //If we got here the file downloaded successfully
                        success = true;
                    }
                }
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    Log.Error("Error reading plugin data from the server: {0}", e.Message);
                }));
            }
            download.completed(success);
        }
    }
}
