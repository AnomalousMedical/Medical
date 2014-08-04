using Engine.Editing;
using Engine.Resources;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public partial class DDAtlasDependency : AtlasDependency, Saveable
    {
        private List<AtlasDependencyResource> resources = new List<AtlasDependencyResource>();

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
            if (resources.Count > 0)
            {
                var ogreResourceManager = OgreWrapper.OgreResourceGroupManager.getInstance();
                foreach (var resource in resources)
                {
                    ogreResourceManager.addResourceLocation(resource.Path, "EngineArchive", DependencyNamespace, true);
                }
                ogreResourceManager.initializeResourceGroup(DependencyNamespace);
            }

            Initialized = true;
        }

        /// <summary>
        /// Add a resource directly to this group.
        /// </summary>
        /// <param name="resource">The resource to add.</param>
        public void addResource(AtlasDependencyResource resource)
        {
            resources.Add(resource);
            onResourceAdded(resource);
        }

        private void removeResource(AtlasDependencyResource resource)
        {
            resources.Remove(resource);
            onResourceRemoved(resource);
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
            info.RebuildList("Resource", resources);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("DependencyId", DependencyId);
            info.AddValue("Name", Name);
            info.AddValue("BrandingImageKey", BrandingImageKey);
            info.AddValue("VersionString", VersionString);
            info.AddValue("DependencyNamespace", DependencyNamespace);
            info.ExtractList("Resource", resources);
        }

    }

    partial class DDAtlasDependency
    {
        private EditInterface editInterface = null;
        private EditInterface resourceLocationsEdit;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, "DDAtlasDependency", null);
                }

                resourceLocationsEdit = new EditInterface("Resource Locations", addResource, removeResource);
                resourceLocationsEdit.setPropertyInfo(AtlasDependencyResource.Info);
                foreach (var resource in resources)
                {
                    resourceLocationsEdit.addEditableProperty(resource);
                }
                editInterface.addSubInterface(resourceLocationsEdit);

                return editInterface;
            }
        }

        /// <summary>
        /// Callback to add a resource.
        /// </summary>
        /// <param name="callback"></param>
        private void addResource(EditUICallback callback)
        {
            addResource(new AtlasDependencyResource());
        }

        /// <summary>
        /// Callback to remove a resource.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="property"></param>
        private void removeResource(EditUICallback callback, EditableProperty property)
        {
            removeResource(((AtlasDependencyResource)property));
        }

        private void onResourceAdded(AtlasDependencyResource resource)
        {
            resourceLocationsEdit.addEditableProperty(resource);
        }

        private void onResourceRemoved(AtlasDependencyResource resource)
        {
            resourceLocationsEdit.removeEditableProperty(resource);
        }
    }
}
