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
        private AnatomyTreeNode rootNode;

        [DoNotSave]
        private List<AnatomyTagProperties> tagPropertiesList = new List<AnatomyTagProperties>();

        public AnatomyOrganizer()
        {
            rootNode = new AnatomyTreeNode("Anatomy Tree");
        }

        protected override void destroy()
        {
            base.destroy();
            AnatomyManager.AnatomyOrganizer = null;
        }

        public void addProperty(AnatomyTagProperties prop)
        {
            tagPropertiesList.Add(prop);
            onPropertyAdded(prop);
        }

        public void removeProperty(AnatomyTagProperties prop)
        {
            tagPropertiesList.Remove(prop);
            onPropertyRemoved(prop);
        }

        public IEnumerable<AnatomyTreeNode> RootNodeChildren
        {
            get
            {
                return rootNode.Children;
            }
        }

        public IEnumerable<AnatomyTagProperties> TagProperties
        {
            get
            {
                return tagPropertiesList;
            }
        }

        protected override void link()
        {
            AnatomyManager.AnatomyOrganizer = this;
        }

        protected override void customLoad(LoadInfo info)
        {
            info.RebuildList<AnatomyTagProperties>("TagProperty", tagPropertiesList);
        }

        protected override void customSave(SaveInfo info)
        {
            info.ExtractList<AnatomyTagProperties>("TagProperty", tagPropertiesList);
        }
    }

    partial class AnatomyOrganizer
    {
        private EditInterface tagPropertiesInterface;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            tagPropertiesInterface = ReflectedEditInterface.createUnscannedEditInterface("Tag Properties", null);
            tagPropertiesInterface.addCommand(new EditInterfaceCommand("Add Tag Property", addProperty));
            var propertyManager = tagPropertiesInterface.createEditInterfaceManager<AnatomyTagProperties>();
            propertyManager.addCommand(new EditInterfaceCommand("Remove", removeProperty));
            propertyManager.addCommand(new EditInterfaceCommand("Rename", renameProperty));
            foreach (AnatomyTagProperties prop in tagPropertiesList)
            {
                onPropertyAdded(prop);
            }

            editInterface.addSubInterface(tagPropertiesInterface);
            editInterface.addSubInterface(rootNode.EditInterface);
        }

        void addProperty(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.getInputString("Enter a name for the child anatomy tag group.", delegate(String result, ref String message)
            {
                foreach (AnatomyTagProperties prop in tagPropertiesList)
                {
                    if (prop.Name == result)
                    {
                        message = String.Format("Tag group properties for {0} already exists. Please enter another name.", result);
                        return false;
                    }
                }
                addProperty(new AnatomyTagProperties(result));
                return true;
            });
        }

        void onPropertyAdded(AnatomyTagProperties prop)
        {
            if (tagPropertiesInterface != null)
            {
                tagPropertiesInterface.addSubInterface(prop, prop.EditInterface);
            }
        }

        void removeProperty(EditUICallback callback, EditInterfaceCommand caller)
        {
            AnatomyTagProperties prop = tagPropertiesInterface.resolveSourceObject<AnatomyTagProperties>(callback.getSelectedEditInterface());
            removeProperty(prop);
        }

        void onPropertyRemoved(AnatomyTagProperties prop)
        {
            if (tagPropertiesInterface != null)
            {
                tagPropertiesInterface.removeSubInterface(prop);
            }
        }

        void renameProperty(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.getInputString("Enter a new name for the child anatomy tag group.", delegate(String result, ref String message)
            {
                foreach (AnatomyTagProperties prop in tagPropertiesList)
                {
                    if (prop.Name == result)
                    {
                        message = String.Format("Tag group properties for {0} already exists. Please enter another name.", result);
                        return false;
                    }
                }
                var selectedEditInterface = callback.getSelectedEditInterface();
                AnatomyTagProperties oldProp = tagPropertiesInterface.resolveSourceObject<AnatomyTagProperties>(selectedEditInterface);
                oldProp.Name = result;
                selectedEditInterface.setName(oldProp.Name);
                return true;
            });
        }
    }
}
