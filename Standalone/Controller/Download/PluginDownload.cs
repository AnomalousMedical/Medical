using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    class PluginDownload : Download
    {
        public PluginDownload(int pluginId, DownloadController controller, DownloadListener downloadListener)
            :base(controller, downloadListener)
        {
            this.PluginId = pluginId;
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
                        atlasPluginManager.addPlugin(Path.Combine(DestinationFolder, FileName));
                        atlasPluginManager.initialzePlugins();
                    }));
                }
            }
        }

        public long PluginId { get; set; }

        void licenseServerReadFail()
        {
            MessageBox.show("There was an problem getting a new license. Please restart the program to use your new plugin.", "License Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
        }

        public override string DestinationFolder
        {
            get
            {
                return MedicalConfig.PluginConfig.PluginsFolder;
            }
        }

        public override string Type
        {
            get
            {
                return "Plugin";
            }
        }

        public override string AdditionalArgs
        {
            get
            {
                return "pluginId=" + PluginId.ToString();
            }
        }
    }
}
