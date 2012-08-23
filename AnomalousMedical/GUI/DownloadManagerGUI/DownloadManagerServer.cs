using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using System.IO;
using Medical.Controller;
using Logging;
using MyGUIPlugin;
using System.Globalization;
using Engine;
using System.Threading;

namespace Medical.GUI
{
    class DownloadManagerServer : IDisposable
    {
        public event Action<ServerDownloadInfo> DownloadFound;
        public event Action FinishedReadingDownloads;

        private ImageAtlas serverImages = new ImageAtlas("PluginManagerServerImages", new Size2(100, 100), new Size2(1024, 1024));
        private LicenseManager licenseManager;
        List<ServerPluginDownloadInfo> detectedServerPlugins = new List<ServerPluginDownloadInfo>();
        private bool foundPlatformUpdate = false;
        private bool active = true;

        public DownloadManagerServer(LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
        }

        public void Dispose()
        {
            serverImages.Dispose();
            active = false;
        }

        public ImageAtlas ServerImages
        {
            get
            {
                return serverImages;
            }
        }

        public void readPluginInfoFromServer(List<AtlasPlugin> installedPlugins)
        {
            Thread serverReadThread = new Thread(delegate()
            {
                StringBuilder sb = new StringBuilder();
                foreach (AtlasPlugin plugin in installedPlugins)
                {
                    sb.AppendFormat("{0}|{1}", plugin.PluginId.ToString(), plugin.Version.ToString());
                    sb.Append(",");
                }
                String installedPluginsList = String.Empty;
                if (sb.Length > 0)
                {
                    installedPluginsList = sb.ToString(0, sb.Length - 1);
                }
                readServerPluginInfo(installedPluginsList);
                ThreadManager.invoke(new Action(delegate()
                {
                    if (FinishedReadingDownloads != null)
                    {
                        FinishedReadingDownloads.Invoke();
                    }
                }));
            });
            serverReadThread.Start();
        }

        public String readLicenseFromServer(int pluginId)
        {
            String postData = String.Format(CultureInfo.InvariantCulture, "pluginId={0}", pluginId);
            return readServerLicenseInfo(postData);
        }

        public String readPlatformLicenseFromServer()
        {
            String postData = "";
            return readServerLicenseInfo(postData);
        }

        internal void removeDetectedPlugin(ServerPluginDownloadInfo serverPluginDownloadInfo)
        {
            detectedServerPlugins.Remove(serverPluginDownloadInfo);
        }

        private String readServerLicenseInfo(String postData)
        {
            try
            {
                String license = null;

                ServerConnection serverConnection = new ServerConnection(MedicalConfig.LicenseReaderURL);
                serverConnection.Timeout = 60000;
                serverConnection.addArgument("Type", LicenseReadType.Plugin.ToString());
                serverConnection.makeRequest(responseStream =>
                    {
                        using (StreamReader serverDataStream = new StreamReader(responseStream))
                        {
                            license = serverDataStream.ReadToEnd();
                        }
                    });
                return license;
            }
            catch (Exception e)
            {
                Log.Error("Could not read license from server because {0}.", e.Message);
            }
            return null;
        }

        private void readServerPluginInfo(String commaSeparatedPluginList)
        {
            try
            {
                Version localVersion = UpdateController.CurrentVersion;
                CredentialServerConnection serverConnection = new CredentialServerConnection(MedicalConfig.PluginInfoURL, licenseManager.User, licenseManager.MachinePassword);
                serverConnection.Timeout = 60000;
                serverConnection.addArgument("Version", localVersion.ToString());
                serverConnection.addArgument("OsId", ((int)PlatformConfig.OsId).ToString());
                serverConnection.addArgument("PluginList", commaSeparatedPluginList);
                serverConnection.makeRequestDownloadResponse(responseStream =>
                    {
                        using (BinaryReader streamReader = new BinaryReader(responseStream))
                        {
                            String versionString = streamReader.ReadString();
                            if (!foundPlatformUpdate)
                            {
                                Version remoteVersion = new Version(versionString);
                                if (remoteVersion > localVersion && remoteVersion > UpdateController.DownloadedVersion)
                                {
                                    ThreadManager.invoke(new Action<ServerDownloadInfo>(delegate(ServerDownloadInfo downloadInfo)
                                    {
                                        if (DownloadFound != null)
                                        {
                                            DownloadFound.Invoke(downloadInfo);
                                        }
                                    }), new PlatformUpdateDownloadInfo(remoteVersion, this));
                                    foundPlatformUpdate = true;
                                }
                            }
                            while (active && streamReader.PeekChar() != -1)
                            {
                                ServerPluginDownloadInfo pluginInfo = new ServerPluginDownloadInfo(this, streamReader.ReadInt32(), streamReader.ReadString(), (ServerDownloadStatus)streamReader.ReadInt16());
                                String imageURL = streamReader.ReadString();
                                if (!alreadyFoundPlugin(pluginInfo.PluginId))
                                {
                                    if (!String.IsNullOrEmpty(imageURL))
                                    {
                                        using (Bitmap image = loadImageFromURL(imageURL))
                                        {
                                            if (image != null)
                                            {
                                                ThreadManager.invokeAndWait(new Action(delegate()
                                                {
                                                    pluginInfo.ImageKey = serverImages.addImage(pluginInfo, image);
                                                }));
                                            }
                                        }
                                    }
                                    ThreadManager.invoke(new Action<ServerDownloadInfo>(delegate(ServerDownloadInfo downloadInfo)
                                    {
                                        if (DownloadFound != null)
                                        {
                                            DownloadFound.Invoke(downloadInfo);
                                        }
                                    }), pluginInfo);
                                    detectedServerPlugins.Add(pluginInfo);
                                }
                            }
                        }
                    });
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    MessageBox.show(String.Format("Error reading plugin data from the server. Please try again later.\nReason: {0}", e.Message), "Server Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }));
            }
        }

        //Runs on background thread
        private Bitmap loadImageFromURL(String url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            return new Bitmap(responseStream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    Log.Error("Could not load image from {0} because {1}.", url, e.Message);
                }));
            }
            return null;
        }

        private bool alreadyFoundPlugin(int pluginId)
        {
            foreach (ServerPluginDownloadInfo downloadInfo in detectedServerPlugins)
            {
                if (downloadInfo.PluginId == pluginId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
