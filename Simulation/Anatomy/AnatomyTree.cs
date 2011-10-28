using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;

namespace Medical
{
    public class AnatomyTree : Interface
    {
        private AnatomyTreeNode rootNode;

        public AnatomyTree()
        {
            rootNode = new AnatomyTreeNode("Root");
        }

        protected override void link()
        {
            recursiveCopyDebug(rootNode);
        }

        void recursiveCopyDebug(AnatomyTreeNode node)
        {
            Logging.Log.Debug("Found {0}", node.Name);
            foreach(AnatomyTreeNode child in node.Children)
            {
                recursiveCopyDebug(child);
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(rootNode.EditInterface);
        }

        public IEnumerable<AnatomyTreeNode> Children
        {
            get
            {
                return rootNode.Children;
            }
        }
    }
}
