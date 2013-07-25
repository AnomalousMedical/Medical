using Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Config
{
    class StoreManagerConfig
    {
        static StoreManagerConfig()
        {
            StoreListUrl = String.Format("{0}/PluginUpload/GetUserStores", MedicalConfig.WebsiteHostUrl);
        }

        public static String StoreListUrl { get; private set; }
    }
}
