using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Medical.Controller;
using System.Net;
using System.Globalization;
using Logging;

namespace Medical
{
    public class UpdateController
    {
        static ConfigFile updateInfo;
        static ConfigSection updateSection;

        static UpdateController()
        {
            String updateIndex = Path.Combine(MedicalConfig.SafeDownloadFolder, "update.ini");
            updateInfo = new ConfigFile(updateIndex);
            updateInfo.loadConfigFile();
            updateSection = updateInfo.createOrRetrieveConfigSection("Update");
            try
            {
                DownloadedVersion = new Version(updateSection.getValue("Version", "0.0.0.0"));
            }
            catch (Exception)
            {
                DownloadedVersion = new Version(0, 0, 0, 0);
            }
            InstallFile = updateSection.getValue("File", "");
            AutoStartUpdate = false;
        }

        private UpdateController()
        {

        }

        public static Version CurrentVersion { get; set; }

        /// <summary>
        /// If this is false the user will be prompted to start the update. If
        /// this is true it will start automatically when promptForUpdate is
        /// called.
        /// </summary>
        public static bool AutoStartUpdate { get; set; }

        public static Version DownloadedVersion { get; private set; }

        public static String InstallFile { get; private set; }

        public static bool HasUpdate
        {
            get
            {
                return CurrentVersion < DownloadedVersion && !String.IsNullOrEmpty(InstallFile) && File.Exists(InstallFile);
            }
        }

        public static void checkForUpdate(Action<bool> checkCompletedCallback, AtlasPluginManager pluginManager, LicenseManager licenseManager)
        {
            //Get a copy of the installed plugins
            List<AtlasPlugin> installedPlugins = new List<AtlasPlugin>(pluginManager.LoadedPlugins);

            //Check for updates on a background thread
            Thread updateThread = new Thread(delegate()
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

                bool foundUpdate = false;
                try
                {
                    Version localVersion = CurrentVersion > DownloadedVersion ? CurrentVersion : DownloadedVersion;
                    CredentialServerConnection serverConnection = new CredentialServerConnection(MedicalConfig.UpdateCheckURL, licenseManager.User, licenseManager.MachinePassword);
                    serverConnection.Timeout = 60000;
                    serverConnection.addArgument("Version", localVersion.ToString());
                    serverConnection.addArgument("OsId", ((int)PlatformConfig.OsId).ToString());
                    serverConnection.addArgument("PluginList", installedPluginsList);
                    serverConnection.makeRequest(responseStream =>
                        {
                            using (BinaryReader serverDataStream = new BinaryReader(responseStream))
                            {
                                foundUpdate = serverDataStream.ReadBoolean();
                            }
                        });
                }
                catch (Exception e)
                {
                    Log.Error("Could not read update status from the server. Reason:\n{0}", e.Message);
                }

                ThreadManager.invoke(checkCompletedCallback, foundUpdate);
            });
            updateThread.Start();
        }

        public static void writeUpdateIndex(String updateInstallerFile, Version version)
        {
            DownloadedVersion = version;
            InstallFile = updateInstallerFile;
            updateSection.setValue("File", updateInstallerFile);
            updateSection.setValue("Version", version.ToString());
            updateInfo.writeConfigFile();
        }

        /// <summary>
        /// Prompt the user for an update. Return true if the update was started.
        /// </summary>
        /// <returns></returns>
        public static bool promptForUpdate()
        {
            if (HasUpdate)
            {
                if (AutoStartUpdate || MessageDialog.showQuestionDialog(String.Format("A new version of the Anomalous Medical Platform {0} has been downloaded. Would you like to install it?", DownloadedVersion), "Update?") == NativeDialogResult.YES)
                {
                    try
                    {
                        Process.Start(InstallFile);
                        return true;
                    }
                    catch (Exception e)
                    {
                        MessageDialog.showErrorDialog(String.Format("Could not install update file {0}. Reason:\n{1}", InstallFile, e.Message), "Update error");
                    }
                }
            }
            return false;
        }
    }
}
