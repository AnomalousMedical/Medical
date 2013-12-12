﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class PopupAreaChainLink : LayoutChainLink
    {
        private EventLayoutContainer eventContainer = new EventLayoutContainer();
        private Dictionary<String, LayoutContainer> fullscreenPopups = new Dictionary<String, LayoutContainer>();

        public PopupAreaChainLink(String name)
            :base(name)
        {
            eventContainer.LayoutChanged += container_LayoutChanged;
        }

        public override void setLayoutItem(string positionHint, LayoutContainer container, AnimationCompletedDelegate animationCompleted = null)
        {
            container.Location = eventContainer.Location;
            container.WorkingSize = eventContainer.WorkingSize;
            container.layout();
            fullscreenPopups.Add(positionHint, container);
        }

        public override void removeLayoutItem(string positionHint)
        {
            fullscreenPopups.Remove(positionHint);
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
            foreach (LayoutContainer fullscreenPopup in fullscreenPopups.Values)
            {
                fullscreenPopup.Location = new IntVector2(xPos, yPos);
                fullscreenPopup.WorkingSize = new IntSize2(innerWidth, innerHeight);
                fullscreenPopup.layout();
            }
        }
    }
}
