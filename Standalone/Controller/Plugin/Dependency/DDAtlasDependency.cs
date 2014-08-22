using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
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
            MyGUIPlugin.ResourceManager.Instance.load(System.IO.Path.Combine(RootFolder, IconResourceFile));
        }

        public void initialize(StandaloneController standaloneController)
        {
            if (resources.Count > 0)
            {
                var resourceManager = standaloneController.AtlasPluginManager.ResourceManager;
                foreach(var subsystemResources in resourceManager.getSubsystemEnumerator())
                {
                    var resourceGroup = subsystemResources.addResourceGroup(PluginNamespace);
                    foreach (var resource in resources)
                    {
                        var engineResource = resourceGroup.addResource(Path.Combine(RootFolder, resource.Path), "EngineArchive", resource.Recursive);
                    }
                }

                resourceManager.initializeResources();
            }

            if (PropDefinitionDirectory != null)
            {
                String fullPropPath = Path.Combine(RootFolder, PropDefinitionDirectory);
                if (VirtualFileSystem.Instance.exists(fullPropPath))
                {
                    foreach (var propPath in VirtualFileSystem.Instance.listFiles(fullPropPath, "*.prop", false))
                    {
                        using (var stream = VirtualFileSystem.Instance.openStream(propPath, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                        {
                            var propDefinition = SharedXmlSaver.Load<PropDefinition>(stream);
                            propDefinition.DependencyPluginId = PluginId;
                            standaloneController.PropFactory.addDefinition(propDefinition);
                        }
                    }
                }
            }
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

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void sceneRevealed()
        {
            
        }

        [Editable]
        public string PluginName { get; private set; }

        public bool AllowUninstall
        {
            get
            {
                return true;
            }
        }

        [Editable]
        public long PluginId { get; set; }

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
        public String PluginNamespace { get; set; }

        [Editable]
        public String IconResourceFile { get; set; }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
            }
        }

        protected DDAtlasDependency(LoadInfo info)
        {
            PluginId = info.GetInt64("PluginId", -1);
            PluginName = info.GetString("PluginName");
            VersionString = info.GetString("VersionString");
            PluginNamespace = info.GetString("PluginNamespace");
            PropDefinitionDirectory = info.GetString("PropDefinitionDirectory");
            IconResourceFile = info.GetString("IconResourceFile");
            BrandingImageKey = info.GetString("BrandingImageKey");
            info.RebuildList("Resource", resources);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("PluginId", PluginId);
            info.AddValue("PluginName", PluginName);
            info.AddValue("VersionString", VersionString);
            info.AddValue("PluginNamespace", PluginNamespace);
            info.AddValue("PropDefinitionDirectory", PropDefinitionDirectory);
            info.AddValue("IconResourceFile", IconResourceFile);
            info.AddValue("BrandingImageKey", BrandingImageKey);
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
                    resourceLocationsEdit.setPropertyInfo(AtlasDependencyResourceEditableProperty.Info);
                    foreach (var resource in resources)
                    {
                        onResourceAdded(resource);
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
            removeResource(resourceLocationsEdit.getKeyObjectForProperty<AtlasDependencyResource>(property));
        }

        private void onResourceAdded(AtlasDependencyResource resource)
        {
            resourceLocationsEdit.addEditableProperty(resource, new AtlasDependencyResourceEditableProperty(resource));
        }

        private void onResourceRemoved(AtlasDependencyResource resource)
        {
            resourceLocationsEdit.removeEditableProperty(resource);
        }
    }
}
