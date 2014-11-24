using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Attributes;
using Engine.Saving;

namespace Medical
{
    public partial class AnatomyOrganizer : BehaviorInterface
    {
        [DoNotSave]
        private List<AnatomyTagProperties> tagProperties = new List<AnatomyTagProperties>();

        [DoNotSave]
        private List<AnatomyTagProperties> systemProperties = new List<AnatomyTagProperties>();

        [DoNotSave]
        private List<AnatomyTagProperties> regionProperties = new List<AnatomyTagProperties>();

        [DoNotSave]
        private List<AnatomyTagProperties> classificationProperties = new List<AnatomyTagProperties>();

        [DoNotSave]
        private List<AnatomyTagProperties> structureProperties = new List<AnatomyTagProperties>();

        public AnatomyOrganizer()
        {
            
        }

        protected override void link()
        {
            AnatomyManager.AnatomyOrganizer = this;
        }

        protected override void destroy()
        {
            base.destroy();
            AnatomyManager.AnatomyOrganizer = null;
        }

        public IEnumerable<AnatomyTagProperties> TagProperties
        {
            get
            {
                return tagProperties;
            }
        }

        public IEnumerable<AnatomyTagProperties> SystemProperties
        {
            get
            {
                return systemProperties;
            }
        }

        public IEnumerable<AnatomyTagProperties> RegionProperties
        {
            get
            {
                return regionProperties;
            }
        }

        public IEnumerable<AnatomyTagProperties> ClassificationProperties
        {
            get
            {
                return classificationProperties;
            }
        }

        public IEnumerable<AnatomyTagProperties> StructureProperties
        {
            get
            {
                return structureProperties;
            }
        }

        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            //Note that we are currently on version 1, but that conversion code was removed.
            info.RebuildList<AnatomyTagProperties>("TagProperty", tagProperties);
            info.RebuildList<AnatomyTagProperties>("SystemProperty", systemProperties);
            info.RebuildList<AnatomyTagProperties>("RegionProperty", regionProperties);
            info.RebuildList<AnatomyTagProperties>("ClassificationProperty", classificationProperties);
            info.RebuildList<AnatomyTagProperties>("StructureProperty", structureProperties);
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.Version = 1;
            info.ExtractList<AnatomyTagProperties>("TagProperty", tagProperties);
            info.ExtractList<AnatomyTagProperties>("SystemProperty", systemProperties);
            info.ExtractList<AnatomyTagProperties>("RegionProperty", regionProperties);
            info.ExtractList<AnatomyTagProperties>("ClassificationProperty", classificationProperties);
            info.ExtractList<AnatomyTagProperties>("StructureProperty", structureProperties);
        }
    }

    partial class AnatomyOrganizer
    {
        protected override void customizeEditInterface(EditInterface editInterface)
        {                
            editInterface.addSubInterfaceForObject(tagProperties, 
                new ReflectedListLikeEditInterface<AnatomyTagProperties>(tagProperties, "Tag Properties", () => new AnatomyTagProperties(), validateCallback: () => validateProperties(tagProperties)));
            editInterface.addSubInterfaceForObject(systemProperties,
                new ReflectedListLikeEditInterface<AnatomyTagProperties>(systemProperties, "System Properties", () => new AnatomyTagProperties(), validateCallback: () => validateProperties(systemProperties)));
            editInterface.addSubInterfaceForObject(regionProperties,
                new ReflectedListLikeEditInterface<AnatomyTagProperties>(regionProperties, "Region Properties", () => new AnatomyTagProperties(), validateCallback: () => validateProperties(regionProperties)));
            editInterface.addSubInterfaceForObject(classificationProperties,
                new ReflectedListLikeEditInterface<AnatomyTagProperties>(classificationProperties, "Classification Properties", () => new AnatomyTagProperties(), validateCallback: () => validateProperties(classificationProperties)));
            editInterface.addSubInterfaceForObject(structureProperties,
                new ReflectedListLikeEditInterface<AnatomyTagProperties>(structureProperties, "Structure Properties", () => new AnatomyTagProperties(), validateCallback: () => validateProperties(structureProperties)));
        }

        private void validateProperties(IEnumerable<AnatomyTagProperties> properties)
        {
            HashSet<String> names = new HashSet<string>();
            foreach (var prop in properties)
            {
                if (String.IsNullOrWhiteSpace(prop.Name))
                {
                    throw new ValidationException("Cannot include empty names.");
                }
                if (names.Contains(prop.Name))
                {
                    throw new ValidationException("The name {0} exists more than once, please remove duplicates.", prop.Name);
                }
                names.Add(prop.Name);
            }
        }
    }
}
