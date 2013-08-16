using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Anomalous.Medical.StoreManager
{
    class StoreManagerTypeFinder : TypeFinder
    {
        private static readonly char[] SPLIT = { ',' };

        static StoreManagerTypeFinder()
        {
            Instance = new StoreManagerTypeFinder();
        }

        public static StoreManagerTypeFinder Instance { get; private set; }

        public Type findType(string assemblyQualifiedName)
        {
            String typeName = assemblyQualifiedName.Split(SPLIT)[0];
            return Assembly.GetExecutingAssembly().GetType(typeName);
        }
    }
}
