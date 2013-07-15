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
using Mono.Security;

namespace Medical.GUI
{
    class DownloadManagerServer : IDisposable
    {
        public event Action<ServerDownloadInfo> DownloadFound;
        public event Action FinishedReadingDownloads;

        private ImageAtlas serverImages = new ImageAtlas("PluginManagerServerImages", new IntSize2(100, 100));
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

        public void readPluginInfoFromServer(AtlasPluginManager pluginManager)
        {
            Thread serverReadThread = new Thread(delegate()
            {
                readServerPluginInfo(pluginManager);
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

        public String readLicenseFromServer(long pluginId)
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
                serverConnection.makeRequestGetStream(responseStream =>
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

        private void readServerPluginInfo(AtlasPluginManager pluginManager)
        {
            try
            {
                ServerUpdateInfo serverUpdateInfo = UpdateController.getUpdateInfo(licenseManager);
                if (!foundPlatformUpdate)
                {
                    Version remoteVersion = serverUpdateInfo.RemotePlatformVersion;
                    if (remoteVersion > UpdateController.CurrentVersion && remoteVersion > UpdateController.DownloadedVersion)
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

                Dictionary<long, ServerPluginDownloadInfo> newDownloadInfos = new Dictionary<long, ServerPluginDownloadInfo>();
                foreach(var pluginUpdateInfo in serverUpdateInfo.PluginUpdateInfo)
                {
                    if (!alreadyFoundPlugin(pluginUpdateInfo.PluginId))
                    {
                        AtlasPlugin plugin = pluginManager.getPlugin(pluginUpdateInfo.PluginId);
                        if (plugin == null)
                        {
                            newDownloadInfos.Add(pluginUpdateInfo.PluginId, new ServerPluginDownloadInfo(this, pluginUpdateInfo.PluginId, "", ServerDownloadStatus.NotInstalled));
                        }
                        else if(pluginUpdateInfo.Version > plugin.Version)
                        {
                            alertDownloadFound(new ServerPluginDownloadInfo(this, pluginUpdateInfo.PluginId, plugin.PluginName, ServerDownloadStatus.Update)
                            {
                                ImageKey = plugin.BrandingImageKey
                            });
                        }
                    }
                }

                if (newDownloadInfos.Count > 0)
                {
                    using (BinaryWriter br = new BinaryWriter(new MemoryStream()))
                    {
                        foreach (var info in newDownloadInfos.Keys)
                        {
                            br.Write(info);
                        }
                        br.Seek(0, SeekOrigin.Begin);

                        ServerConnection connection = new ServerConnection(MedicalConfig.PluginInfoURL);
                        connection.addFileStream("PluginIds", br.BaseStream);
                        connection.makeRequestDownloadResponse(response =>
                        {
                            ASN1 results = new ASN1(response.ToArray());
                            for (int i = 0; i < results.Count; ++i)
                            {
                                ASN1 pluginInfo = results[i];
                                long pluginId = BitConverter.ToInt64(pluginInfo[0].Value, 0);
                                ServerPluginDownloadInfo downloadInfo = newDownloadInfos[pluginId];
                                downloadInfo.Name = Encoding.BigEndianUnicode.GetString(pluginInfo[1].Value);
                                ASN1 brandingImageLocationAsn1 = pluginInfo[2];
                                if (brandingImageLocationAsn1.Tag != 0x05)
                                {
                                    String imageURL = Encoding.BigEndianUnicode.GetString(brandingImageLocationAsn1.Value);
                                    if (!String.IsNullOrEmpty(imageURL))
                                    {
                                        using (Bitmap image = loadImageFromURL(imageURL))
                                        {
                                            if (image != null)
                                            {
                                                ThreadManager.invokeAndWait(new Action(delegate()
                                                {
                                                    downloadInfo.ImageKey = serverImages.addImage(pluginInfo, image);
                                                }));
                                            }
                                        }
                                    }
                                }

                                alertDownloadFound(downloadInfo);
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    MessageBox.show(String.Format("Error reading plugin data from the server. Please try again later.\nReason: {0}", e.Message), "Server Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
					Log.Error("Error reading plugin data from server.");
					Log.Default.printException(e);
                }));
            }
        }

        //Runs on background thread
        private void alertDownloadFound(ServerPluginDownloadInfo downloadInfo)
        {
            ThreadManager.invoke(new Action<ServerDownloadInfo>(delegate(ServerDownloadInfo downloadInfoCb)
            {
                if (DownloadFound != null)
                {
                    DownloadFound.Invoke(downloadInfoCb);
                }
            }), downloadInfo);
            detectedServerPlugins.Add(downloadInfo);
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

        private bool alreadyFoundPlugin(long pluginId)
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
