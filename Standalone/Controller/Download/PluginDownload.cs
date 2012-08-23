using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    public class PluginDownload : Download
    {
        public PluginDownload(int pluginId, DownloadController controller, DownloadListener downloadListener)
            :base(controller, downloadListener)
        {
            this.PluginId = pluginId;
            this.DestinationFolder = MedicalConfig.PluginConfig.PluginsFolder;
        }

        protected override void onCompleted(bool success)
        {
            if (success)
            {
                LicenseManager licenseManager = controller.LicenseManager;
                AtlasPluginManager atlasPluginManager = controller.PluginManager;

                if (!licenseManager.allowFeature(PluginId) && !licenseManager.getNewLicense())
                {
                    ThreadManager.invoke(new Action(licenseServerReadFail));
                }
                else
                {
                    //Load plugin back on main thread
                    ThreadManager.invoke(new Action(delegate()
                    {
                        String pluginFile = Path.Combine(DestinationFolder, FileName);
                        LoadedSucessfully = atlasPluginManager.addPlugin(pluginFile);
                        atlasPluginManager.initialzePlugins();
                        if (DownloadedToSafeLocation)
                        {
                            atlasPluginManager.addPluginToMove(pluginFile);
                        }
                        else
                        {
                            Plugin = atlasPluginManager.findPlugin(PluginId);
                        }
                    }));
                }
            }
        }

        public long PluginId { get; set; }

        public AtlasPlugin Plugin { get; private set; }

        public bool LoadedSucessfully { get; set; }

        void licenseServerReadFail()
        {
            MessageBox.show("There was an problem getting a new license. Please restart the program to use your new plugin.", "License Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
        }

        public override DownloadType Type
        {
            get
            {
                return DownloadType.Plugin;
            }
        }

        public override string IdName
        {
            get
            {
                return "pluginId";
            }
        }

        public override string Id
        {
            get
            {
                return PluginId.ToString();
            }
        }
    }
}
