using Engine;
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
        private Dictionary<String, AnimatedLayoutContainer> panels = new Dictionary<string, AnimatedLayoutContainer>();

        public BorderLayoutChainLink(String name, Engine.Platform.UpdateTimer tempTimer)
            :base(name)
        {
            //OMG!!!!! MAKE THE CONTAINERS CREATED BY THE BORDERLAYOUTCHAINLINK GET DISPOSED SOMEHOW
            AnimatedLayoutContainer animatedContainer = new HorizontalPopoutLayoutContainer(tempTimer);
            borderLayout.Left = animatedContainer;
            panels.Add("Left", animatedContainer);

            animatedContainer = new HorizontalPopoutLayoutContainer(tempTimer);
            borderLayout.Right = animatedContainer;
            panels.Add("Right", animatedContainer);

            animatedContainer = new VerticalPopoutLayoutContainer(tempTimer);
            borderLayout.Top = animatedContainer;
            panels.Add("Top", animatedContainer);

            animatedContainer = new VerticalPopoutLayoutContainer(tempTimer);
            borderLayout.Bottom = animatedContainer;
            panels.Add("Bottom", animatedContainer);
        }

        public void Dispose()
        {
            foreach (AnimatedLayoutContainer panel in panels.Values)
            {
                panel.Dispose();
            }
            panels.Clear();
        }

        public override void setLayoutItem(string positionHint, LayoutContainer container, AnimationCompletedDelegate animationCompleted = null)
        {
            AnimatedLayoutContainer panel;
            if (panels.TryGetValue(positionHint, out panel))
            {
                if (panel.CurrentContainer != container)
                {
                    panel.changePanel(container, 0.25f, animationCompleted);
                }
            }
            else
            {
                Log.Warning("Cannot add container to border layout position '{0}' because it cannot be found. Container will not be visible on UI.", positionHint);
                if (animationCompleted != null)
                {
                    animationCompleted.Invoke(container);
                }
            }
        }

        public override void removeLayoutItem(string positionHint)
        {
            AnimatedLayoutContainer panel;
            if (panels.TryGetValue(positionHint, out panel))
            {
                if (panel.CurrentContainer != null)
                {
                    panel.changePanel(null, 0.25f, null);
                }
            }
            else
            {
                Log.Warning("Cannot remove container from border layout position '{0}' because it cannot be found. Container will not be visible on UI.", positionHint);
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
    }
}
