using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using System.IO;
using System.Reflection;
using MyGUIPlugin;

namespace Medical.GUI
{
    class MyGUIRocketResources : RocketFileSystemExtension
    {
        private Assembly myGUIAssembly;

        public MyGUIRocketResources()
        {
            myGUIAssembly = typeof(MyGUIInterface).Assembly;
        }

        public bool canOpenFile(string file)
        {
            return myGUIAssembly.GetManifestResourceInfo(file) != null;
        }

        public Stream openFile(string file)
        {
            return myGUIAssembly.GetManifestResourceStream(file);
        }
    }
}
