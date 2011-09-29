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
    public class DownloadController : IDisposable
    {
        public delegate void UpdateDownloadStatus(Object downloadCallbackObject, int progress);

        public delegate void DownloadCompleteCallback(Object downloadCallbackObject, bool success);

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

        public void downloadPlugin(int pluginId, UpdateDownloadStatus statusUpdate, DownloadCompleteCallback downloadCompleted, Object downloadCallbackObject)
        {
            Thread downloadThread = new Thread(delegate()
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.PluginDownloadURL));
                    request.Timeout = 10000;
                    request.Method = "POST";
                    String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}&type=Plugin&pluginId={2}", licenseManager.User, licenseManager.MachinePassword, pluginId);
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
                            String filename = response.Headers["content-disposition"].Substring(21);
                            String sizeStr = response.Headers["Content-Length"];
                            float fileSize = NumberParser.ParseFloat(sizeStr);
                            String pluginFileLocation = Path.Combine(MedicalConfig.PluginConfig.PluginsFolder, filename);
                            using (Stream localDataStream = new FileStream(pluginFileLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                byte[] buffer = new byte[8 * 1024];
                                int len;
                                int totalRead = 0;
                                while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    totalRead += len;
                                    localDataStream.Write(buffer, 0, len);
                                    ThreadManager.invoke(statusUpdate, downloadCallbackObject, (int)(totalRead / fileSize * 100.0f));
                                }
                            }

                            if (!licenseManager.allowFeature(pluginId) && !licenseManager.getNewLicense())
                            {
                                ThreadManager.invoke(new Action(licenseServerReadFail));
                            }
                            else
                            {
                                //Load plugin back on main thread
                                ThreadManager.invoke(new Action<String>(delegate(String pluginFile)
                                {
                                    pluginManager.addPlugin(pluginFile);
                                    pluginManager.initialzePlugins();
                                }), pluginFileLocation);
                            }

                            //If we got here the plugin installed correctly
                            ThreadManager.invoke(downloadCompleted, downloadCallbackObject, true);
                            return;
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
                ThreadManager.invoke(downloadCompleted, downloadCallbackObject, false);
            });
            downloadThread.Start();
        }

        void licenseServerReadFail()
        {
            MessageBox.show("There was an problem getting a new license. Please restart the program to use your new plugin.", "License Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
        }
    }
}
