using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Logging;

namespace Medical.GUI
{
    public class EditInterfaceTreeNode : TreeNode
    {
        private EditInterface editInterface;
        private EditInterfaceTreeView editInterfaceTreeView;

        public EditInterfaceTreeNode(EditInterface editInterface, EditInterfaceTreeView editInterfaceTreeView)
            :base(editInterface.getName())
        {
            if (editInterface.IconReferenceTag != null)
            {
                this.ImageResource = editInterface.IconReferenceTag.ToString();
            }

            this.editInterfaceTreeView = editInterfaceTreeView;

            this.editInterface = editInterface;
            editInterface.OnSubInterfaceAdded += editInterface_OnSubInterfaceAdded;
            editInterface.OnSubInterfaceRemoved += editInterface_OnSubInterfaceRemoved;
            editInterface.OnBackColorChanged += editInterface_OnBackColorChanged;
            editInterface.OnForeColorChanged += editInterface_OnForeColorChanged;
            editInterface.OnIconReferenceChanged += editInterface_OnIconReferenceChanged;
            editInterface.OnNameChanged += editInterface_OnNameChanged;

            if (editInterface.hasSubEditInterfaces())
            {
                foreach (EditInterface subInterface in editInterface.getSubEditInterfaces())
                {
                    this.Children.add(new EditInterfaceTreeNode(subInterface, editInterfaceTreeView));
                }
            }
        }

        public override void Dispose()
        {
            editInterface.OnSubInterfaceAdded -= editInterface_OnSubInterfaceAdded;
            editInterface.OnSubInterfaceRemoved -= editInterface_OnSubInterfaceRemoved;
            editInterface.OnBackColorChanged -= editInterface_OnBackColorChanged;
            editInterface.OnForeColorChanged -= editInterface_OnForeColorChanged;
            editInterface.OnIconReferenceChanged -= editInterface_OnIconReferenceChanged;
            editInterface.OnNameChanged -= editInterface_OnNameChanged;
            foreach (EditInterfaceTreeNode child in Children)
            {
                child.Dispose();
            }
            base.Dispose();
        }

        public EditInterface EditInterface
        {
            get
            {
                return editInterface;
            }
        }

        void editInterface_OnIconReferenceChanged(EditInterface editInterface)
        {

        }

        void editInterface_OnForeColorChanged(EditInterface editInterface)
        {

        }

        void editInterface_OnBackColorChanged(EditInterface editInterface)
        {

        }

        void editInterface_OnNameChanged(EditInterface editInterface)
        {
            this.Text = editInterface.getName();
        }

        void editInterface_OnSubInterfaceRemoved(EditInterface editInterface)
        {
            EditInterfaceTreeNode matchingNode = null;
            foreach (EditInterfaceTreeNode node in Children)
            {
                if (node.EditInterface == editInterface)
                {
                    matchingNode = node;
                    break;
                }
            }
            if (matchingNode != null)
            {
                editInterfaceTreeView.nodeRemoved(this);
                this.Children.remove(matchingNode);
            }
            else
            {
                Log.Default.sendMessage("Malformed EditInterfaceTreeNodes the EditInterface {0} does not contain a child named {1} when remove was attempted.", LogLevel.Error, "Editor", this.editInterface.getName(), editInterface.getName());
            }
        }

        void editInterface_OnSubInterfaceAdded(EditInterface editInterface)
        {
            this.Children.add(new EditInterfaceTreeNode(editInterface, editInterfaceTreeView));
            editInterfaceTreeView.nodeAdded(this);
        }
    }
}
