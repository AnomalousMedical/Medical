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

        private List<KeyValuePair<View, AnomalousMvcContext>> queuedFloatingViews = new List<KeyValuePair<View, AnomalousMvcContext>>();
        private List<ViewHost> openFloatingViews = new List<ViewHost>();

        public ViewHostManager(GUIManager guiManager, ViewHostFactory viewHostFactory)
        {
            this.guiManager = guiManager;
            this.viewHostFactory = viewHostFactory;
        }

        public void requestOpen(View view, AnomalousMvcContext context)
        {
            switch (view.ViewLocation)
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
                case ViewLocations.Floating:
                    queuedFloatingViews.Add(new KeyValuePair<View, AnomalousMvcContext>(view, context));
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

        public void requestCloseAll()
        {
            if (currentLeft != null)
            {
                currentLeft._RequestClosed = true;
            }
            if (currentRight != null)
            {
                currentRight._RequestClosed = true;
            }
            if (currentTop != null)
            {
                currentTop._RequestClosed = true;
            }
            if (currentBottom != null)
            {
                currentBottom._RequestClosed = true;
            }
            foreach (ViewHost viewHost in openFloatingViews)
            {
                viewHost._RequestClosed = true;
            }
        }

        public void processViewChanges()
        {
            //-----------Left Panel-----------------
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
                    ViewHost last = currentLeft;
                    last.closing();
                    currentLeft = viewHostFactory.createViewHost(queuedLeft, queuedLeftContext);
                    currentLeft.opening();
                    guiManager.changeLeftPanel(currentLeft.Container, last._animationCallback);
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

            //-----------Right Panel-----------------
            //If we have another panel queued
            if (queuedRight != null)
            {
                //If there is no panel open
                if (currentRight == null)
                {
                    currentRight = viewHostFactory.createViewHost(queuedRight, queuedRightContext);
                    currentRight.opening();
                    guiManager.changeRightPanel(currentRight.Container);
                }
                //If there is a panel open they must be switched
                else
                {
                    ViewHost last = currentRight;
                    last.closing();
                    currentRight = viewHostFactory.createViewHost(queuedRight, queuedRightContext);
                    currentRight.opening();
                    guiManager.changeRightPanel(currentRight.Container, last._animationCallback);
                }
            }
            //There is no other panel queued and the current panel wants to be closed
            else if (currentRight != null && currentRight._RequestClosed)
            {
                currentRight.closing();
                guiManager.changeRightPanel(null, currentRight._animationCallback);
                currentRight = null;
            }
            queuedRight = null;
            queuedRightContext = null;

            //-----------Top Panel-----------------
            //If we have another panel queued
            if (queuedTop != null)
            {
                //If there is no panel open
                if (currentTop == null)
                {
                    currentTop = viewHostFactory.createViewHost(queuedTop, queuedTopContext);
                    currentTop.opening();
                    guiManager.changeTopPanel(currentTop.Container);
                }
                //If there is a panel open they must be switched
                else
                {
                    ViewHost last = currentTop;
                    last.closing();
                    currentTop = viewHostFactory.createViewHost(queuedTop, queuedTopContext);
                    currentTop.opening();
                    guiManager.changeTopPanel(currentTop.Container, last._animationCallback);
                }
            }
            //There is no other panel queued and the current panel wants to be closed
            else if (currentTop != null && currentTop._RequestClosed)
            {
                currentTop.closing();
                guiManager.changeTopPanel(null, currentTop._animationCallback);
                currentTop = null;
            }
            queuedTop = null;
            queuedTopContext = null;

            //-----------Bottom Panel-----------------
            //If we have another panel queued
            if (queuedBottom != null)
            {
                //If there is no panel open
                if (currentBottom == null)
                {
                    currentBottom = viewHostFactory.createViewHost(queuedBottom, queuedBottomContext);
                    currentBottom.opening();
                    guiManager.changeBottomPanel(currentBottom.Container);
                }
                //If there is a panel open they must be switched
                else
                {
                    ViewHost last = currentBottom;
                    last.closing();
                    currentBottom = viewHostFactory.createViewHost(queuedBottom, queuedBottomContext);
                    currentBottom.opening();
                    guiManager.changeBottomPanel(currentBottom.Container, last._animationCallback);
                }
            }
            //There is no other panel queued and the current panel wants to be closed
            else if (currentBottom != null && currentBottom._RequestClosed)
            {
                currentBottom.closing();
                guiManager.changeBottomPanel(null, currentBottom._animationCallback);
                currentBottom = null;
            }
            queuedBottom = null;
            queuedBottomContext = null;

            //Floating views
            for (int i = 0; i < openFloatingViews.Count; ++i)
            {
                ViewHost host = openFloatingViews[i];
                if (host._RequestClosed)
                {
                    host.closing();
                    //TEMP, this will crash unless it happens after the event fires, so delay the close for a bit
                    ThreadManager.invoke(new Action(delegate()
                    {
                        host._animationCallback(null);
                    }));
                    openFloatingViews.RemoveAt(i--);
                }
            }
            foreach (KeyValuePair<View, AnomalousMvcContext> viewInfo in queuedFloatingViews)
            {
                ViewHost viewHost = viewHostFactory.createViewHost(viewInfo.Key, viewInfo.Value);
                viewHost.opening();
                openFloatingViews.Add(viewHost);
            }
            queuedFloatingViews.Clear();
        }

        public bool HasOpenViews
        {
            get
            {
                return currentLeft != null ||
                    currentRight != null ||
                    currentTop != null ||
                    currentBottom != null ||
                    openFloatingViews.Count > 0;
            }
        }
    }
}
