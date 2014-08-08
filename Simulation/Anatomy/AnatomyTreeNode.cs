using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Attributes;
using Engine.Saving;

namespace Medical
{
    public partial class AnatomyTreeNode : Saveable
    {
        String name;

        private List<AnatomyTreeNode> children = new List<AnatomyTreeNode>();

        public AnatomyTreeNode(String name)
        {
            this.name = name;
        }

        public AnatomyTreeNode()
        {

        }

        public void addChild(AnatomyTreeNode child)
        {
            children.Add(child);
            onChildAdded(child);
        }

        public void insertChild(AnatomyTreeNode newChild, AnatomyTreeNode insertBefore)
        {
            int index = children.IndexOf(insertBefore);
            if (index != -1)
            {
                children.Insert(index, newChild);
            }
            else
            {
                children.Add(newChild);
            }
            onChildAdded(newChild);
        }

        public void removeChild(AnatomyTreeNode child)
        {
            children.Remove(child);
            onChildRemoved(child);
        }

        [DoNotCopy]
        public IEnumerable<AnatomyTreeNode> Children
        {
            get
            {
                return children;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        #region Saveable Members

        protected AnatomyTreeNode(LoadInfo info)
        {
            name = info.GetString("Name");
            info.RebuildList<AnatomyTreeNode>("Child", children);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", name);
            info.ExtractList<AnatomyTreeNode>("Child", children);
        }

        #endregion
    }

    partial class AnatomyTreeNode
    {
        [DoNotCopy]
        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, name, null);
                    editInterface.addCommand(new EditInterfaceCommand("Add Child", addChild));
                    var childNodeManager = editInterface.createEditInterfaceManager<AnatomyTreeNode>();
                    childNodeManager.addCommand(new EditInterfaceCommand("Remove", removeChild));
                    childNodeManager.addCommand(new EditInterfaceCommand("Rename", renameChild));
                    foreach (AnatomyTreeNode child in children)
                    {
                        onChildAdded(child);
                    }
                }
                return editInterface;
            }
        }

        void addChild(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.getInputString("Enter a name for the child anatomy tag group.", delegate(String result, ref String message)
            {
                foreach (AnatomyTreeNode child in children)
                {
                    if (child.name == result)
                    {
                        message = String.Format("A child anatomy group named {0} already exists at this level. Please enter another name.", result);
                        return false;
                    }
                }
                addChild(new AnatomyTreeNode(result));
                return true;
            });
        }

        void onChildAdded(AnatomyTreeNode child)
        {
            if (editInterface != null)
            {
                editInterface.addSubInterface(child, child.EditInterface);
            }
        }

        void removeChild(EditUICallback callback, EditInterfaceCommand caller)
        {
            AnatomyTreeNode child = editInterface.resolveSourceObject<AnatomyTreeNode>(callback.getSelectedEditInterface());
            removeChild(child);
        }

        void onChildRemoved(AnatomyTreeNode child)
        {
            if (editInterface != null)
            {
                editInterface.removeSubInterface(child);
            }
        }

        void renameChild(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.getInputString("Enter a new name for the child anatomy tag group.", delegate(String result, ref String message)
            {
                foreach (AnatomyTreeNode child in children)
                {
                    if (child.name == result)
                    {
                        message = String.Format("A child anatomy group named {0} already exists at this level. Please enter another name.", result);
                        return false;
                    }
                }
                var selectedEditInterface = callback.getSelectedEditInterface();
                AnatomyTreeNode oldChild = editInterface.resolveSourceObject<AnatomyTreeNode>(selectedEditInterface);
                oldChild.Name = result;
                selectedEditInterface.setName(oldChild.Name);
                return true;
            });
        }
    }
}
