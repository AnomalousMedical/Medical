using Engine;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class LayoutChain
    {
        private LinkedList<LayoutChainLink> activeLinks = new LinkedList<LayoutChainLink>(); //The currently active chain
        private Dictionary<String, LayoutChainLink> links = new Dictionary<String, LayoutChainLink>(); //All elements that can be part of the chain

        public LayoutChain()
        {

        }

        public void addLink(LayoutChainLink link, bool addToActiveChainEnd = false)
        {
            links.Add(link.Name, link);
            if (addToActiveChainEnd)
            {
                doActivateLink(link, activeLinks.Last);
            }
        }

        public void removeLink(LayoutChainLink link)
        {
            deactivateLink(link.Name);
            links.Remove(link.Name);
        }

        public void activateLinkAsRoot(String name)
        {
            LayoutChainLink link;
            if (links.TryGetValue(name, out link))
            {
                LayoutContainer topContainer = link.Container;
                topContainer.Location = Location;
                topContainer.WorkingSize = WorkingSize;
                if (activeLinks.First != null)
                {
                    link._setChildContainer(activeLinks.First.Value.Container);
                }
                activeLinks.AddFirst(link);
            }
            else
            {
                Log.Warning("Cannot find a layout chain link named '{0}' to activate as root. Did you build your chain correctly.");
            }
        }

        public void deactivateLink(string name)
        {
            LayoutChainLink link;
            if (links.TryGetValue(name, out link))
            {
                link._setChildContainer(null);
                LinkedListNode<LayoutChainLink> node = activeLinks.Find(links[name]);
                if (node != null)
                {
                    if (node.Previous != null)
                    {
                        LayoutContainer nextContainer = null;
                        if (node.Next != null)
                        {
                            nextContainer = node.Next.Value.Container;
                        }
                        node.Previous.Value._setChildContainer(nextContainer);
                    }
                    activeLinks.Remove(link);
                }
            }
            else
            {
                Log.Warning("Cannot find a layout chain link named '{0}' to deactivate. Did you build your chain correctly.");
            }
        }

        public void addContainer(String linkName, String positionHint, LayoutContainer container, AnimationCompletedDelegate animationCompleted = null)
        {
            LayoutChainLink link;
            if (links.TryGetValue(linkName, out link))
            {
                link.setLayoutItem(positionHint, container, animationCompleted);
            }
            else
            {
                Log.Warning("Cannot add container to link '{0}' because it cannot be found. No changes made.", linkName);
                if (animationCompleted != null)
                {
                    animationCompleted.Invoke(container);
                }
            }
        }

        public void removeContainer(String linkName, String positionHint)
        {
            LayoutChainLink link;
            if (links.TryGetValue(linkName, out link))
            {
                link.removeLayoutItem(positionHint);
            }
            else
            {
                Log.Warning("Cannot remove container from link '{0}' because it cannot be found. No changes made.", linkName);
            }
        }

        public void layout()
        {
            if (activeLinks.Count > 0)
            {
                LayoutContainer topContainer = activeLinks.First.Value.Container;
                topContainer.Location = Location;
                topContainer.WorkingSize = WorkingSize;
                topContainer.layout();
            }
        }

        public IntVector2 Location { get; set; }

        public IntSize2 WorkingSize { get; set; }

        public bool SuppressLayout
        {
            get
            {
                return TopContainer.SuppressLayout;
            }
            set
            {
                TopContainer.SuppressLayout = value;
            }
        }

        private LayoutContainer TopContainer
        {
            get
            {
                return activeLinks.First.Value.Container;
            }
        }

        private void doActivateLink(LayoutChainLink link, LinkedListNode<LayoutChainLink> previousNode)
        {
            if (previousNode != null)
            {
                activeLinks.AddAfter(previousNode, link);
                previousNode.Value._setChildContainer(link.Container);
            }
            else
            {
                activeLinks.AddLast(link);
            }
        }
    }
}
