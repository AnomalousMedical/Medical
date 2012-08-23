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
        private LicenseManager licenseManager;
        private AtlasPluginManager pluginManager;
        private List<Download> activeDownloads = new List<Download>();

        private ManualResetEvent pauseEvent = new ManualResetEvent(false);

        public DownloadController(LicenseManager licenseManager, AtlasPluginManager pluginManager)
        {
            this.licenseManager = licenseManager;
            this.pluginManager = pluginManager;

            Thread t = new Thread(singleSimultaneousDownloadThread);
            t.IsBackground = true;
            t.Start();
        }

        public void Dispose()
        {
            foreach (Download download in activeDownloads)
            {
                download.cancelDownload(DownloadPostAction.DeleteFile);
            }
        }

        public Download downloadPlugin(int pluginId, DownloadListener downloadListener)
        {
            PluginDownload pluginDownload = new PluginDownload(pluginId, this, downloadListener);
            startDownload(pluginDownload);
            return pluginDownload;
        }

        public Download downloadPlatformUpdate(Version version, DownloadListener downloadListener)
        {
            PlatformUpdateDownload platformDownload = new PlatformUpdateDownload(version, this, downloadListener);
            startDownload(platformDownload);
            return platformDownload;
        }

        public LicenseManager LicenseManager
        {
            get
            {
                return licenseManager;
            }
        }

        public AtlasPluginManager PluginManager
        {
            get
            {
                return pluginManager;
            }
        }

        public bool Downloading
        {
            get
            {
                lock (activeDownloads)
                {
                    return activeDownloads.Count > 0;
                }
            }
        }

        public Task OpenDownloadGUITask { get; set; }

        /// <summary>
        /// Callback method for when a download is completed. DO NOT CALL THIS
        /// unless you are the Download class. To stop downloads call the cancel
        /// method on a download.
        /// </summary>
        /// <param name="download"></param>
        internal void _alertDownloadCompleted(Download download)
        {
            lock (activeDownloads)
            {
                activeDownloads.Remove(download);
            }
        }

        private void startDownload(Download download)
        {
            download.StatusString = "Pending";
            download.updateStatus();
            lock (activeDownloads)
            {
                activeDownloads.Add(download);
                pauseEvent.Set();
            }
        }

        private void singleSimultaneousDownloadThread()
        {
            while (true)
            {
                pauseEvent.WaitOne(Timeout.Infinite);

                Download nextDownload = null;
                lock (activeDownloads)
                {
                    if (activeDownloads.Count > 0)
                    {
                        nextDownload = activeDownloads[0];
                    }
                    else
                    {
                        pauseEvent.Reset();
                    }
                }
                if (nextDownload != null)
                {
                    genericBackgroundDownload(nextDownload);
                }
            }
        }

        private void genericBackgroundDownload(Download download)
        {
            download.StatusString = "Starting";
            download.updateStatus();
            bool success = false;
            try
            {
                CredentialServerConnection serverConnection = new CredentialServerConnection(MedicalConfig.PluginDownloadURL, licenseManager.User, licenseManager.MachinePassword);
                serverConnection.Timeout = 60000;
                serverConnection.addArgument("Type", download.Type.ToString());
                serverConnection.addArgument("Version", UpdateController.CurrentVersion.ToString());
                serverConnection.addArgument(download.IdName, download.Id);
                serverConnection.makeRequest(webResponse =>
                    {
                        using (Stream serverDataStream = webResponse.GetResponseStream())
                        {
                            download.FileName = Path.GetFileName(webResponse.ResponseUri.LocalPath);
                            String sizeStr = webResponse.Headers["Content-Length"];
                            download.TotalSize = NumberParser.ParseLong(sizeStr);
                            String pluginFileLocation = Path.Combine(download.DestinationFolder, download.FileName);
                            try
                            {
                                downloadData(download, serverDataStream, pluginFileLocation);
                            }
                            catch (IOException)
                            {
                                download.DownloadedToSafeLocation = true;
                                download.DestinationFolder = MedicalConfig.SafeDownloadFolder;
                                pluginFileLocation = Path.Combine(download.DestinationFolder, download.FileName);
                                downloadData(download, serverDataStream, pluginFileLocation);
                            }

                            if (download.Cancel)
                            {
                                switch (download.CancelPostAction)
                                {
                                    case DownloadPostAction.DeleteFile:
                                        File.Delete(pluginFileLocation);
                                        break;
                                }
                            }
                            else
                            {
                                //If we got here the file downloaded successfully
                                success = true;
                            }
                        }
                    });
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

        private static void downloadData(Download download, Stream serverDataStream, String pluginFileLocation)
        {
            using (Stream localDataStream = new FileStream(pluginFileLocation, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                download.StatusString = "Downloading";
                byte[] buffer = new byte[8 * 1024];
                int len;
                download.TotalRead = 0;
                while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0 && !download.Cancel)
                {
                    download.TotalRead += len;
                    localDataStream.Write(buffer, 0, len);
                    download.updateStatus();
                }
            }
        }
    }
}
