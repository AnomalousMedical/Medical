using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class PopupAreaChainLink : LayoutChainLink
    {
        private EventLayoutContainer eventContainer = new EventLayoutContainer();
        private List<LayoutContainer> fullscreenPopups = new List<LayoutContainer>();
        private ActiveContainerTracker activeContainers = new ActiveContainerTracker();

        public PopupAreaChainLink(String name)
            : base(name)
        {
            eventContainer.LayoutChanged += container_LayoutChanged;
        }

        public override void setLayoutItem(LayoutElementName elementName, LayoutContainer container, Action removedCallback)
        {
            if (container != null)
            {
                container.Location = eventContainer.Location;
                container.WorkingSize = eventContainer.WorkingSize;
                container.layout();
                fullscreenPopups.Add(container);
                activeContainers.add(container, removedCallback);
            }
        }

        public override void removeLayoutItem(LayoutElementName elementName, LayoutContainer container)
        {
            fullscreenPopups.Remove(container);
            activeContainers.remove(container);
        }

        public override LayoutContainer Container
        {
            get
            {
                return eventContainer;
            }
        }

        protected internal override void _setChildContainer(LayoutContainer layoutContainer)
        {
            eventContainer.Child = layoutContainer;
        }

        void container_LayoutChanged(EventLayoutContainer obj)
        {
            int xPos = (int)eventContainer.Location.x;
            int yPos = (int)eventContainer.Location.y;
            int innerWidth = (int)eventContainer.WorkingSize.Width;
            int innerHeight = (int)eventContainer.WorkingSize.Height;
            foreach (LayoutContainer fullscreenPopup in fullscreenPopups)
            {
                fullscreenPopup.Location = new IntVector2(xPos, yPos);
                fullscreenPopup.WorkingSize = new IntSize2(innerWidth, innerHeight);
                fullscreenPopup.layout();
            }
        }

        public override IEnumerable<LayoutElementName> ElementNames
        {
            get
            {
                yield return new LayoutElementName(Name);
            }
        }
    }
}
