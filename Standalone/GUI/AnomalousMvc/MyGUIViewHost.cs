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
        private MyGUIView myGUIView;

        public event Action<ViewHost> ViewClosing;
        public event Action<ViewHost> ViewOpening;
        public event Action<ViewHost> ViewResized;

        public MyGUIViewHost(AnomalousMvcContext context, MyGUIView view)
        {
            this.context = context;
            this.Name = view.Name;
            this.myGUIView = view;
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

        public void changeScale(float newScale)
        {
            component.changeScale(newScale);
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

        public View View
        {
            get
            {
                return myGUIView;
            }
        }

        public bool _RequestClosed { get; set; }

        public bool Open { get; private set; }

        /// <summary>
        /// A callback to send to the GUI manager that will be called when it is done with this view host.
        /// </summary>
        public void _finishedWithView()
        {
            Dispose();
            InputManager.Instance.refreshMouseWidget();
        }

        IntSize2 getDesiredSize()
        {
            IntSize2 workingSize;
            if (myGUIView.fireGetDesiredSizeOverride(layoutContainer, layoutContainer.Widget, out workingSize))
            {
                return workingSize;
            }

            return new IntSize2(layoutContainer.Widget.Width, layoutContainer.Widget.Height);
        }

        void layoutContainer_LayoutChanged()
        {
            component.topLevelResized();
            if (ViewResized != null)
            {
                ViewResized.Invoke(this);
            }
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
