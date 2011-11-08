using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class UninstallInfo
    {
        private AtlasPlugin plugin;

        public UninstallInfo(AtlasPlugin plugin)
        {
            this.plugin = plugin;
            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In ut urna ac orci posuere gravida. Sed vel purus magna, sed iaculis nisl. Donec a rhoncus nulla. Fusce consequat faucibus nulla quis dictum. Duis rutrum eros sit amet leo suscipit rhoncus lobortis arcu egestas. Quisque varius volutpat dui eu dictum. Morbi fringilla dolor ut tortor mattis tempor viverra nunc ullamcorper. Donec ac tempor tortor. Maecenas a egestas quam. Maecenas pharetra consectetur accumsan. Aliquam erat volutpat. Mauris quis nibh mi, id faucibus eros. Nullam porta urna a magna lacinia tristique. Nullam malesuada, nisi quis scelerisque scelerisque, risus nisi egestas tellus, sed porttitor lacus eros vel nisi. Duis augue magna, egestas ut suscipit ac, egestas in est.\n\nSed eget turpis velit. Vestibulum sit amet ultricies erat. Fusce eu auctor diam. Duis nec augue eu nulla consectetur dapibus. Nunc placerat vulputate vulputate. Nullam sagittis, enim et tempor euismod, est justo vehicula tortor, eu rutrum sapien risus ut ligula. Curabitur et turpis purus. Integer ornare ullamcorper erat, quis mollis ligula eleifend sit amet. Nulla vulputate sodales ornare. Aenean non metus lacinia ligula cursus ullamcorper sit amet nec metus.\n\nDuis viverra neque eu arcu adipiscing vestibulum. Cras consequat dignissim tellus, in molestie dui euismod eu. Pellentesque purus elit, ultricies dignissim ultrices a, commodo sit amet tortor. Phasellus gravida lorem nec augue tempor viverra. Sed imperdiet mattis lectus non tempor. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nullam neque nisl, blandit quis luctus in, cursus et lacus. Praesent dignissim neque quis metus cursus cursus. Etiam tincidunt condimentum dui at ornare.\n\nIn ac est nibh, tempor imperdiet diam. Sed dapibus suscipit dolor, eu tincidunt eros consectetur ac. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Quisque nunc tortor, dapibus at congue a, interdum non arcu. In fermentum posuere risus ac iaculis. Sed id lorem lorem. Donec arcu dolor, eleifend vitae pretium ac, tempus ac eros. Pellentesque tempus porttitor metus ut venenatis. Sed interdum lectus eget arcu lacinia sollicitudin. Suspendisse potenti. In et metus ligula, vel pharetra odio. Nunc enim lectus, sollicitudin eget imperdiet quis, egestas vel arcu. Maecenas ultrices mauris nec purus sodales fermentum. Sed id ipsum a elit cursus luctus. Ut non blandit erat. Ut vel velit arcu, id tincidunt sem.\n\nAliquam lacinia, mauris non lobortis dignissim, nibh lectus tempor tellus, sit amet laoreet massa diam sed arcu. Mauris rutrum consequat ultrices. Quisque commodo lacinia felis, at dictum arcu convallis non. Sed eget lacus risus, vitae suscipit nisl. Mauris ac metus dui. Aliquam nisl dui, adipiscing eu mollis vel, eleifend nec ligula. Duis ultrices, ipsum vel condimentum ultricies, mauris enim rutrum lacus, at convallis arcu mi in nulla. Integer interdum egestas nunc et pretium. Sed nec turpis neque. Phasellus adipiscing molestie felis eget laoreet.";
        }

        public void uninstall(AtlasPluginManager pluginManager)
        {
            pluginManager.addPluginToUninstall(plugin);
        }

        public String ImageKey
        {
            get
            {
                return plugin.BrandingImageKey;
            }
        }

        public String Name
        {
            get
            {
                return plugin.PluginName;
            }
        }

        public String Description { get; set; }
    }
}
