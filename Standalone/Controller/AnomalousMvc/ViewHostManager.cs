using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.GUI;
using Engine.Attributes;

namespace Medical.Controller.AnomalousMvc
{
    [SingleEnum]
    public enum ViewLocations
    {
        Left,
        Right,
        Top,
        Bottom,
        Floating
    }

    class ViewHostManager
    {
        private GUIManager guiManager;
        private ViewHostFactory viewHostFactory;

        private ViewHost currentLeft;
        private ViewHost currentRight;
        private ViewHost currentTop;
        private ViewHost currentBottom;

        private View queuedLeft;
        private View queuedRight;
        private View queuedTop;
        private View queuedBottom;

        private AnomalousMvcContext queuedLeftContext;
        private AnomalousMvcContext queuedRightContext;
        private AnomalousMvcContext queuedTopContext;
        private AnomalousMvcContext queuedBottomContext;

        public ViewHostManager(GUIManager guiManager, ViewHostFactory viewHostFactory)
        {
            this.guiManager = guiManager;
            this.viewHostFactory = viewHostFactory;
        }

        public void requestOpen(View view, AnomalousMvcContext context, ViewLocations viewLocation)
        {
            switch (viewLocation)
            {
                case ViewLocations.Left:
                    queuedLeft = view;
                    queuedLeftContext = context;
                    break;
                case ViewLocations.Right:
                    queuedRight = view;
                    queuedRightContext = context;
                    break;
                case ViewLocations.Top:
                    queuedTop = view;
                    queuedTopContext = context;
                    break;
                case ViewLocations.Bottom:
                    queuedBottom = view;
                    queuedBottomContext = context;
                    break;
            }
        }

        public void requestClose(ViewHost viewHost)
        {
            if (viewHost != null)
            {
                viewHost._RequestClosed = true;
            }
        }

        public void processViewChanges()
        {
            //If we have another panel queued
            if (queuedLeft != null)
            {
                //If there is no panel open
                if (currentLeft == null)
                {
                    currentLeft = viewHostFactory.createViewHost(queuedLeft, queuedLeftContext);
                    currentLeft.opening();
                    guiManager.changeLeftPanel(currentLeft.Container);
                }
                //If there is a panel open they must be switched
                else
                {
                    ViewHost lastLeft = currentLeft;
                    lastLeft.closing();
                    currentLeft = viewHostFactory.createViewHost(queuedLeft, queuedLeftContext);
                    currentLeft.opening();
                    guiManager.changeLeftPanel(currentLeft.Container, lastLeft._animationCallback);
                }
            }
            //There is no other panel queued and the current panel wants to be closed
            else if (currentLeft != null && currentLeft._RequestClosed)
            {
                currentLeft.closing();
                guiManager.changeLeftPanel(null, currentLeft._animationCallback);
                currentLeft = null;
            }

            queuedLeft = null;
            queuedLeftContext = null;
        }

        public bool HasOpenViews
        {
            get
            {
                return currentLeft != null ||
                    currentRight != null ||
                    currentTop != null ||
                    currentBottom != null;
            }
        }
    }
}
