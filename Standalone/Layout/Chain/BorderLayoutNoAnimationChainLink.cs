using Engine;
using Engine.Platform;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BorderLayoutNoAnimationChainLink : LayoutChainLink, IDisposable
    {
        private BorderLayoutContainer borderLayout = new BorderLayoutContainer();
        private ActiveContainerTracker activePanels = new ActiveContainerTracker();

        public BorderLayoutNoAnimationChainLink(String name)
            : base(name)
        {
            
        }

        public void Dispose()
        {
            
        }

        public override void setLayoutItem(LayoutElementName elementName, LayoutContainer container, Action removedCallback)
        {
            bool added = true;
            switch (((BorderLayoutElementName)elementName).ViewLocation)
            {
                case BorderLayoutLocations.Left:
                    borderLayout.Left = container;
                    break;
                case BorderLayoutLocations.Right:
                    borderLayout.Right = container;
                    break;
                case BorderLayoutLocations.Top:
                    borderLayout.Top = container;
                    break;
                case BorderLayoutLocations.Bottom:
                    borderLayout.Bottom = container;
                    break;
                case BorderLayoutLocations.Center:
                    borderLayout.Center = container;
                    break;
                default:
                    Log.Warning("Cannot add container to border layout position '{0}' because it cannot be found. Container will not be visible on UI.", elementName.Name);
                    if (removedCallback != null)
                    {
                        removedCallback.Invoke();
                    }
                    added = false;
                    break;
            }
            if (added)
            {
                activePanels.add(container, removedCallback);
            }
        }

        public override void removeLayoutItem(LayoutElementName elementName, LayoutContainer container)
        {
            bool removed = true;
            switch (((BorderLayoutElementName)elementName).ViewLocation)
            {
                case BorderLayoutLocations.Left:
                    borderLayout.Left = null;
                    break;
                case BorderLayoutLocations.Right:
                    borderLayout.Right = null;
                    break;
                case BorderLayoutLocations.Top:
                    borderLayout.Top = null;
                    break;
                case BorderLayoutLocations.Bottom:
                    borderLayout.Bottom = null;
                    break;
                case BorderLayoutLocations.Center:
                    borderLayout.Center = null;
                    break;
                default:
                    Log.Warning("Cannot remove container from border layout position '{0}' because it cannot be found. Container will not be visible on UI.", elementName.Name);
                    removed = false;
                    break;
            }
            if (removed)
            {
                activePanels.remove(container);
            }
        }

        protected internal override void _setChildContainer(LayoutContainer layoutContainer)
        {
            borderLayout.Center = layoutContainer;
        }

        public override LayoutContainer Container
        {
            get
            {
                return borderLayout;
            }
        }

        public override IEnumerable<LayoutElementName> ElementNames
        {
            get
            {
                yield return new BorderLayoutElementName(Name, BorderLayoutLocations.Left);
                yield return new BorderLayoutElementName(Name, BorderLayoutLocations.Right);
                yield return new BorderLayoutElementName(Name, BorderLayoutLocations.Top);
                yield return new BorderLayoutElementName(Name, BorderLayoutLocations.Bottom);
                yield return new BorderLayoutElementName(Name, BorderLayoutLocations.Center);
            }
        }
    }
}
