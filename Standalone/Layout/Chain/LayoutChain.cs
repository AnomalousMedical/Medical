using Engine;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class will chain various layout elements together in an abstract way to form a gui.
    /// It provides a flexible way to add / remove elements from various places on the ui.
    /// </summary>
    public class LayoutChain
    {
        private LinkedList<LayoutChainLink> activeLinks = new LinkedList<LayoutChainLink>(); //The currently active chain
        private Dictionary<String, LayoutChainLink> links = new Dictionary<String, LayoutChainLink>(); //All elements that can be part of the chain
        private Dictionary<LayoutElementName, LayoutChainLink> namedLinkedElements = new Dictionary<LayoutElementName, LayoutChainLink>();

        public LayoutChain()
        {

        }

        public void addLink(LayoutChainLink link, bool addToActiveChainEnd = false)
        {
            links.Add(link.Name, link);
            foreach (var namedElement in link.ElementNames)
            {
                namedLinkedElements.Add(namedElement, link);
            }
            if (addToActiveChainEnd)
            {
                doActivateLink(link, activeLinks.Last);
            }
        }

        public void removeLink(LayoutChainLink link)
        {
            if (links.ContainsValue(link))
            {
                deactivateLink(link.Name);
                foreach (var namedElement in link.ElementNames)
                {
                    namedLinkedElements.Remove(namedElement);
                }
                links.Remove(link.Name);
            }
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

        /// <summary>
        /// Add a layout container to this chain. When the chain is done with the item it will call the removedCallback
        /// if one is provided.
        /// </summary>
        public void addContainer(LayoutElementName elementName, LayoutContainer container, Action removedCallback)
        {
            LayoutChainLink link;
            if (namedLinkedElements.TryGetValue(elementName, out link))
            {
                link.setLayoutItem(elementName, container, removedCallback);
            }
            else
            {
                Log.Warning("Cannot add container to container '{0}' because it cannot be found. No changes made.", elementName.Name);
                if (removedCallback != null)
                {
                    removedCallback.Invoke();
                }
            }
        }

        /// <summary>
        /// Remove a container from the chain. This will trigger the removed callback for the container if one was provided.
        /// </summary>
        public void removeContainer(LayoutElementName elementName, LayoutContainer container)
        {
            LayoutChainLink link;
            if (namedLinkedElements.TryGetValue(elementName, out link))
            {
                link.removeLayoutItem(elementName, container);
            }
            else
            {
                Log.Warning("Cannot remove container from link '{0}' because it cannot be found. No changes made.", elementName.Name);
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

        public IEnumerable<LayoutElementName> NamedLinks
        {
            get
            {
                return namedLinkedElements.Keys;
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
