using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// This class helps the downloader find installed products on the hard drive.
    /// </summary>
    class ProductTracker
    {
        private static String productsFile = String.Format("{0}/Anomalous Medical/installedProducts.ini", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        private ProductTracker()
        {

        }

        public static void SetInstalledLocation(int productID, String installLocation)
        {
            ConfigFile productConfig = new ConfigFile(productsFile);
            productConfig.loadConfigFile();
            ConfigSection productSection = productConfig.createOrRetrieveConfigSection("Products");
            productSection.setValue("Product" + productID, installLocation);
            productConfig.writeConfigFile();
        }

        public static String GetInstalledLocation(int productID)
        {
            ConfigFile productConfig = new ConfigFile(productsFile);
            productConfig.loadConfigFile();
            ConfigSection productSection = productConfig.createOrRetrieveConfigSection("Products");
            return productSection.getValue("Product" + productID, null);
        }
    }
}
