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
using Mono.Security;

namespace Medical
{
    public class UpdateController
    {
        [Flags]
        public enum UpdateCheckResult
        {
            NoUpdates = 0,
            PluginUpdates = 1,
            PlatformUpdate = 1 << 1,
        }

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

        public static void checkForUpdate(Action<UpdateCheckResult> checkCompletedCallback, AtlasPluginManager pluginManager, LicenseManager licenseManager)
        {
            //Check for updates on a background thread
            ThreadPool.QueueUserWorkItem(state =>
            {
                UpdateCheckResult result = UpdateCheckResult.NoUpdates;
                try
                {
                    ServerUpdateInfo updateInfo = getUpdateInfo(licenseManager);
                    if (updateInfo.RemotePlatformVersion > CurrentVersion)
                    {
                        result |= UpdateCheckResult.PlatformUpdate;
                    }
                    else
                    {
                        foreach(var pluginUpdate in updateInfo.PluginUpdateInfo)
                        {
                            AtlasPlugin plugin = pluginManager.getPlugin(pluginUpdate.PluginId);
                            if (plugin != null && pluginUpdate.Version > plugin.Version)
                            {
                                result |= UpdateCheckResult.PluginUpdates;
                                break;
                            }
                        }
                        foreach(var dependencyUpdate in updateInfo.DependencyUpdateInfo)
                        {
                            AtlasPlugin plugin = pluginManager.getPlugin(dependencyUpdate.PluginId);
                            if(plugin != null && dependencyUpdate.Version > plugin.Version)
                            {
                                result |= UpdateCheckResult.PluginUpdates;
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Could not read update status from the server. Reason:\n{0}", e.Message);
                }

                ThreadManager.invoke(checkCompletedCallback, result);
            });
        }

        /// <summary>
        /// Get the update info from the server, note that this makes a web request on the calling thread.
        /// </summary>
        /// <param name="licenseManager"></param>
        /// <returns></returns>
        public static ServerUpdateInfo getUpdateInfo(LicenseManager licenseManager)
        {
            CredentialServerConnection serverConnection = new CredentialServerConnection(MedicalConfig.UpdateCheckURL, licenseManager.User, licenseManager.MachinePassword);
            serverConnection.addArgument("OsId", ((int)PlatformConfig.OsId).ToString());
            ServerUpdateInfo updateInfo = null;
            serverConnection.makeRequestDownloadResponse(responseStream =>
            {
                updateInfo = new ServerUpdateInfo(responseStream.ToArray());
            });
            if (updateInfo == null)
            {
                throw new UpdateException("No update info found");
            }
            return updateInfo;
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
