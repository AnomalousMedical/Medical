using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

namespace Medical
{
    /// <summary>
    /// A basic class that can be used to create timelineguis quickly that use MyGUI.
    /// </summary>
    public abstract class MyGUITimelineGUI : TimelineGUI, IDisposable
    {
        private Layout layout;
        protected Widget widget;
        protected MyGUILayoutContainer layoutContainer;

        public MyGUITimelineGUI(String layoutFile)
        {
            layout = LayoutManager.Instance.loadLayout(layoutFile);
            widget = layout.getWidget(0);
            layoutContainer = new MyGUILayoutContainer(widget);
        }

        public virtual void Dispose()
        {
            if (layout != null)
            {
                LayoutManager.Instance.unloadLayout(layout);
            }
        }

        public abstract void initialize(ShowTimelineGUIAction showTimelineAction);

        public abstract void show(GUIManager guiManager);

        public abstract void hide(GUIManager guiManager);

        internal LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }
    }
}
