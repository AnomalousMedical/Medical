using Engine.Editing;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class DDAtlasDependency : AtlasDependency, Saveable
    {
        public DDAtlasDependency()
        {

        }

        public void Dispose()
        {
            
        }

        public void loadGUIResources()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            Initialized = true;
        }

        [Editable]
        public long DependencyId { get; set; }

        [Editable]
        public string Name { get; set; }

        [Editable]
        public string BrandingImageKey { get; set; }

        public string Location { get; internal set; }

        public Version Version { get; private set; }

        [Editable]
        public String VersionString
        {
            get
            {
                return Version.ToString();
            }
            set
            {
                try
                {
                    Version = new Version(value);
                }
                catch (Exception) { }
            }
        }

        public String RootFolder { get; internal set; }

        [Editable]
        public String DependencyNamespace { get; set; }

        public bool Initialized { get; private set; }

        protected DDAtlasDependency(LoadInfo info)
        {
            DependencyId = info.GetInt64("DependencyId");
            Name = info.GetString("Name");
            BrandingImageKey = info.GetString("BrandingImageKey");
            VersionString = info.GetString("VersionString");
            DependencyNamespace = info.GetString("DependencyNamespace");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("DependencyId", DependencyId);
            info.AddValue("Name", Name);
            info.AddValue("BrandingImageKey", BrandingImageKey);
            info.AddValue("VersionString", VersionString);
            info.AddValue("DependencyNamespace", DependencyNamespace);
        }
    }
}
