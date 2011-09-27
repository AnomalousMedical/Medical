using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.Net;
using System.IO;
using System.Globalization;
using Logging;

namespace Medical.GUI
{
    class PluginManagerGUI : MDIDialog
    {
        enum InstallStatus
        {
            Installed,
            NotInstalled
        }

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
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Installed");

            installPanel = window.findWidget("InstallPanel");
            installPanel.Visible = false;

            Button installButton = (Button)window.findWidget("InstallButton");
            installButton.MouseButtonClick += new MyGUIEvent(installButton_MouseButtonClick);
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            pluginGrid.clear();
            List<int> serverPlugins = readServerPlugins();
            foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
            {
                ButtonGridItem item = pluginGrid.addItem("Installed", plugin.PluginName);
                item.UserObject = InstallStatus.Installed;
                serverPlugins.Remove((int)plugin.PluginId);
            }
            foreach (int pluginId in serverPlugins)
            {
                ButtonGridItem item = pluginGrid.addItem("Not Installed", pluginId.ToString());
                item.UserObject = InstallStatus.NotInstalled;
            }
        }

        void pluginGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            installPanel.Visible = selectedItem != null && (InstallStatus)selectedItem.UserObject == InstallStatus.NotInstalled;
        }

        void installButton_MouseButtonClick(Widget source, EventArgs e)
        {
            
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
    }
}
