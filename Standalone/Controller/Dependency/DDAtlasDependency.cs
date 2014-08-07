using Engine;
using Engine.Editing;
using Engine.Resources;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.IO;
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
            String dependencyPath = Path.Combine("Dependencies", DependencyNamespace);

            if (resources.Count > 0)
            {
                var resourceManager = PluginManager.Instance.createEmptyResourceManager();
                foreach(var subsystemResources in resourceManager.getSubsystemEnumerator())
                {
                    var resourceGroup = subsystemResources.addResourceGroup(DependencyNamespace);
                    foreach (var resource in resources)
                    {
                        var engineResource = resourceGroup.addResource(Path.Combine(dependencyPath, resource.Path), resource.Recursive);
                    }
                }

                PluginManager.Instance.PersistentResourceManager.addResources(resourceManager);
                PluginManager.Instance.PersistentResourceManager.initializeResources();
            }

            if (PropDefinitionDirectory != null)
            {
                String fullPropPath = Path.Combine(dependencyPath, PropDefinitionDirectory);
                if (VirtualFileSystem.Instance.exists(fullPropPath))
                {
                    foreach (var propPath in VirtualFileSystem.Instance.listFiles(fullPropPath, "*.prop", false))
                    {
                        using (var stream = VirtualFileSystem.Instance.openStream(propPath, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                        {
                            var propDefinition = SharedXmlSaver.Load<PropDefinition>(stream);
                            standaloneController.PropFactory.addDefinition(propDefinition);
                        }
                    }
                }
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

        [Editable]
        public String PropDefinitionDirectory { get; set; }

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
            PropDefinitionDirectory = info.GetString("PropDefinitionDirectory");
            info.RebuildList("Resource", resources);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("DependencyId", DependencyId);
            info.AddValue("Name", Name);
            info.AddValue("BrandingImageKey", BrandingImageKey);
            info.AddValue("VersionString", VersionString);
            info.AddValue("DependencyNamespace", DependencyNamespace);
            info.AddValue("PropDefinitionDirectory", PropDefinitionDirectory);
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

                    resourceLocationsEdit = new EditInterface("Resource Locations", addResource, removeResource);
                    resourceLocationsEdit.setPropertyInfo(AtlasDependencyResource.Info);
                    foreach (var resource in resources)
                    {
                        resourceLocationsEdit.addEditableProperty(resource);
                    }
                    editInterface.addSubInterface(resourceLocationsEdit);
                }

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
