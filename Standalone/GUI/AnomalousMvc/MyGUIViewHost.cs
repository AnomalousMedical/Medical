using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    public class MyGUIViewHost : ViewHost
    {
        private ViewHostComponent component;
        private VariableSizeMyGUILayoutContainer layoutContainer;
        private AnomalousMvcContext context;

        public event Action<ViewHost> ViewClosing;
        public event Action<ViewHost> ViewOpening;

        public MyGUIViewHost(AnomalousMvcContext context, View view)
        {
            this.context = context;
            this.Name = view.Name;
            this.View = view;
        }

        public void setTopComponent(ViewHostComponent component)
        {
            this.component = component;
            layoutContainer = new VariableSizeMyGUILayoutContainer(component.Widget, getDesiredSize);
            layoutContainer.LayoutChanged += new Action(layoutContainer_LayoutChanged);
            layoutContainer.AnimatedResizeStarted += layoutContainer_AnimatedResizeStarted;
            layoutContainer.AnimatedResizeCompleted += layoutContainer_AnimatedResizeCompleted;
        }

        public void Dispose()
        {
            component.Dispose();
        }

        public void opening()
        {
            Open = true;
            component.opening();
            if (!String.IsNullOrEmpty(View.OpeningAction))
            {
                context.queueRunAction(View.OpeningAction, this);
            }
            if (ViewOpening != null)
            {
                ViewOpening.Invoke(this);
            }
        }

        public void closing()
        {
            Open = false;
            if (ViewClosing != null)
            {
                ViewClosing.Invoke(this);
            }
            if (!String.IsNullOrEmpty(View.ClosingAction))
            {
                context.queueRunAction(View.ClosingAction, this);
            }
            component.closing();
        }

        public void populateViewData(IDataProvider dataProvider)
        {
            component.populateViewData(dataProvider);
        }

        public void analyzeViewData(IDataProvider dataProvider)
        {
            component.analyzeViewData(dataProvider);
        }

        public ViewHostControl findControl(String name)
        {
            return component.findControl(name);
        }

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }

        public AnomalousMvcContext Context
        {
            get
            {
                return context;
            }
        }

        public String Name { get; private set; }

        public View View { get; private set; }

        public bool _RequestClosed { get; set; }

        public bool Open { get; private set; }

        public void _animationCallback(LayoutContainer oldChild)
        {
            Dispose();
            InputManager.Instance.refreshMouseWidget();
        }

        IntSize2 getDesiredSize()
        {
            if (View.SizeStrategy == Controller.AnomalousMvc.View.SizeType.Percentage)
            {
                if (View.ViewLocation == ViewLocations.Left || View.ViewLocation == ViewLocations.Right)
                {
                    int width = (int)(View.Size * 0.01f * layoutContainer.TopmostWorkingSize.Width);
                    return new IntSize2(width, component.Widget.Height);
                }

                if (View.ViewLocation == ViewLocations.Top || View.ViewLocation == ViewLocations.Bottom)
                {
                    int height = (int)(View.Size * 0.01f * layoutContainer.TopmostWorkingSize.Height);
                    return new IntSize2(component.Widget.Width, height);
                }
            }
            return new IntSize2(component.Widget.Width, component.Widget.Height);
        }

        void layoutContainer_LayoutChanged()
        {
            component.topLevelResized();
        }

        void layoutContainer_AnimatedResizeCompleted()
        {
            component.animatedResizeCompleted();
        }

        void layoutContainer_AnimatedResizeStarted(IntSize2 finalSize)
        {
            component.animatedResizeStarted(finalSize);
        }
    }
}
