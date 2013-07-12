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
                CredentialServerConnection serverConnection = new CredentialServerConnection(MedicalConfig.PluginInfoURL, licenseManager.User, licenseManager.MachinePassword);
                serverConnection.addArgument("OsId", ((int)PlatformConfig.OsId).ToString());
                serverConnection.makeRequestDownloadResponse(responseStream =>
                    {
                        ASN1 asn1 = new ASN1(responseStream.ToArray());
                        if (!foundPlatformUpdate)
                        {
                            Version remoteVersion = Version.Parse(Encoding.ASCII.GetString(asn1.Element(0, 0x13).Value));
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
                        ASN1 pluginInfos = asn1.Element(1, 0x30);
                        for(int i = 0; i < pluginInfos.Count && active; ++i)
                        {
                            ASN1 asn1PluginInfo = pluginInfos[i];
                            long pluginId = BitConverter.ToInt64(asn1PluginInfo[0].Value, 0);
                            Version remotePluginVersion = Version.Parse(Encoding.ASCII.GetString(asn1PluginInfo.Element(1, 0x13).Value));
                            String name = Encoding.BigEndianUnicode.GetString(asn1PluginInfo.Element(2, 0x1E).Value);
                            AtlasPlugin plugin = pluginManager.getPlugin(pluginId);

                            ServerPluginDownloadInfo pluginInfo = null;
                            if (plugin == null)
                            {
                                pluginInfo = new ServerPluginDownloadInfo(this, pluginId, name, ServerDownloadStatus.NotInstalled);
                            }
                            else if(remotePluginVersion > plugin.Version)
                            {
                                pluginInfo = new ServerPluginDownloadInfo(this, pluginId, name, ServerDownloadStatus.Update);
                            }

                            if (pluginInfo != null)
                            {
                                String imageURL = Encoding.BigEndianUnicode.GetString(asn1PluginInfo.Element(3, 0x1E).Value);
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
					Log.Error("Error reading plugin data from server.");
					Log.Default.printException(e);
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
