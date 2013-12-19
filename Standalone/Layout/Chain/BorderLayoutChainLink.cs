using Engine;
using Engine.Platform;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BorderLayoutChainLink : LayoutChainLink, IDisposable
    {
        private BorderLayoutContainer borderLayout = new BorderLayoutContainer();
        private Dictionary<LayoutElementName, AnimatedLayoutContainer> panels = new Dictionary<LayoutElementName, AnimatedLayoutContainer>();
        private ActiveContainerTracker activePanels = new ActiveContainerTracker();

        public BorderLayoutChainLink(String name, UpdateTimer tempTimer)
            : base(name)
        {
            AnimatedLayoutContainer animatedContainer = new HorizontalPopoutLayoutContainer(tempTimer);
            borderLayout.Left = animatedContainer;
            panels.Add(new BorderLayoutElementName(name, BorderLayoutLocations.Left), animatedContainer);
            animatedContainer.AnimationComplete += animatedContainer_AnimationComplete;

            animatedContainer = new HorizontalPopoutLayoutContainer(tempTimer);
            borderLayout.Right = animatedContainer;
            panels.Add(new BorderLayoutElementName(name, BorderLayoutLocations.Right), animatedContainer);
            animatedContainer.AnimationComplete += animatedContainer_AnimationComplete;

            animatedContainer = new VerticalPopoutLayoutContainer(tempTimer);
            borderLayout.Top = animatedContainer;
            panels.Add(new BorderLayoutElementName(name, BorderLayoutLocations.Top), animatedContainer);
            animatedContainer.AnimationComplete += animatedContainer_AnimationComplete;

            animatedContainer = new VerticalPopoutLayoutContainer(tempTimer);
            borderLayout.Bottom = animatedContainer;
            panels.Add(new BorderLayoutElementName(name, BorderLayoutLocations.Bottom), animatedContainer);
            animatedContainer.AnimationComplete += animatedContainer_AnimationComplete;
        }

        public void Dispose()
        {
            foreach (AnimatedLayoutContainer panel in panels.Values)
            {
                if (panel.CurrentContainer != null)
                {
                    activePanels.remove(panel.CurrentContainer);
                }
                panel.Dispose();
            }
            panels.Clear();
        }

        public override void setLayoutItem(LayoutElementName elementName, LayoutContainer container, Action removedCallback)
        {
            AnimatedLayoutContainer panel;
            if (panels.TryGetValue(elementName, out panel))
            {
                if (panel.CurrentContainer != container)
                {
                    panel.changePanel(container, 0.25f);
                    activePanels.add(container, removedCallback);
                }
            }
            else
            {
                Log.Warning("Cannot add container to border layout position '{0}' because it cannot be found. Container will not be visible on UI.", elementName.Name);
                if (removedCallback != null)
                {
                    removedCallback.Invoke();
                }
            }
        }

        public override void removeLayoutItem(LayoutElementName elementName, LayoutContainer container)
        {
            AnimatedLayoutContainer panel;
            if (panels.TryGetValue(elementName, out panel))
            {
                if (panel.CurrentContainer != null)
                {
                    panel.changePanel(null, 0.25f);
                }
            }
            else
            {
                Log.Warning("Cannot remove container from border layout position '{0}' because it cannot be found. No changes made.", elementName.Name);
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
                return panels.Keys;
            }
        }

        void animatedContainer_AnimationComplete(LayoutContainer oldChild)
        {
            activePanels.remove(oldChild);
        }
    }
}
