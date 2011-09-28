using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.Net;
using System.IO;
using System.Globalization;
using Logging;
using Engine;

namespace Medical.GUI
{
    class PluginManagerGUI : MDIDialog
    {
        private Widget installPanel;
        private ButtonGrid pluginGrid;

        private AtlasPluginManager pluginManager;
        private LicenseManager licenseManager;

        public PluginManagerGUI(AtlasPluginManager pluginManager, LicenseManager licenseManager)
            :base("Medical.GUI.PluginManagerGUI.PluginManagerGUI.layout")
        {
            this.pluginManager = pluginManager;
            this.licenseManager = licenseManager;

            pluginGrid = new ButtonGrid((ScrollView)window.findWidget("PluginScrollList"), new ButtonGridListLayout());
            pluginGrid.SelectedValueChanged += new EventHandler(pluginGrid_SelectedValueChanged);

            installPanel = window.findWidget("InstallPanel");
            installPanel.Visible = false;

            Button installButton = (Button)window.findWidget("InstallButton");
            installButton.MouseButtonClick += new MyGUIEvent(installButton_MouseButtonClick);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);

            pluginGrid.clear();
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Installed");

            pluginGrid.SuppressLayout = true;
            List<int> serverPlugins = readServerPlugins();
            foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
            {
                ButtonGridItem item = pluginGrid.addItem("Installed", plugin.PluginName);
                serverPlugins.Remove((int)plugin.PluginId);
            }
            StringBuilder sb = new StringBuilder();
            foreach (int pluginId in serverPlugins)
            {
                sb.Append(pluginId.ToString());
                sb.Append(",");
            }
            if (sb.Length > 0)
            {
                String uninstalledPluginsList = sb.ToString(0, sb.Length - 1);

                List<ServerPluginInfo> pluginInfo = readServerPluginInfo(uninstalledPluginsList);
                foreach (ServerPluginInfo plugin in pluginInfo)
                {
                    ButtonGridItem item = pluginGrid.addItem("Not Installed", plugin.Name);
                    item.UserObject = plugin;
                }
            }

            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();
        }

        void pluginGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            installPanel.Visible = selectedItem != null && selectedItem.UserObject != null;
        }

        void installButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerPluginInfo pluginInfo = selectedItem.UserObject as ServerPluginInfo;
            if (pluginInfo != null)
            {
                downloadPlugin(pluginInfo.PluginId);
            }
        }

        List<int> readServerPlugins()
        {
            List<int> pluginIdList = new List<int>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.UserPluginListURL));
                request.Timeout = 10000;
                request.Method = "POST";
                String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}", licenseManager.User, licenseManager.MachinePassword);
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
                        using (Stream localDataStream = new MemoryStream())
                        {
                            byte[] buffer = new byte[8 * 1024];
                            int len;
                            while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                localDataStream.Write(buffer, 0, len);
                            }
                            localDataStream.Seek(0, SeekOrigin.Begin);
                            using (BinaryReader binReader = new BinaryReader(localDataStream))
                            {
                                while (binReader.BaseStream.Position < binReader.BaseStream.Length)
                                {
                                    pluginIdList.Add(binReader.ReadInt32());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error reading plugin data from the server: {0}", e.Message);
            }

            return pluginIdList;
        }

        List<ServerPluginInfo> readServerPluginInfo(String commaSeparatedPluginList)
        {
            List<ServerPluginInfo> pluginInfoList = new List<ServerPluginInfo>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.PluginInfoURL));
                request.Timeout = 10000;
                request.Method = "POST";
                String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}&list={2}", licenseManager.User, licenseManager.MachinePassword, commaSeparatedPluginList);
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
                        using (Stream localDataStream = new MemoryStream())
                        {
                            byte[] buffer = new byte[8 * 1024];
                            int len;
                            while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                localDataStream.Write(buffer, 0, len);
                            }
                            localDataStream.Seek(0, SeekOrigin.Begin);
                            using (StreamReader streamReader = new StreamReader(localDataStream))
                            {
                                while (!streamReader.EndOfStream)
                                {
                                    pluginInfoList.Add(new ServerPluginInfo(NumberParser.ParseInt(streamReader.ReadLine()), streamReader.ReadLine()));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error reading plugin data from the server: {0}", e.Message);
            }

            return pluginInfoList;
        }

        void downloadPlugin(int pluginId)
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
                        String pluginFileLocation = Path.Combine(MedicalConfig.PluginConfig.PluginsFolder, filename);
                        using (Stream localDataStream = new FileStream(pluginFileLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            byte[] buffer = new byte[8 * 1024];
                            int len;
                            while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                localDataStream.Write(buffer, 0, len);
                            }
                        }
                        pluginManager.addPlugin(pluginFileLocation);
                        pluginManager.initialzePlugins();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error reading plugin data from the server: {0}", e.Message);
            }
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            pluginGrid.layout();
        }
    }
}
