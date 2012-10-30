using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public class MyGUIViewHost : ViewHost
    {
        private ViewHostComponent component;
        private MyGUILayoutContainer layoutContainer;
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
            layoutContainer = new MyGUILayoutContainer(component.Widget);
            layoutContainer.LayoutChanged += new Action(layoutContainer_LayoutChanged);
        }

        public void Dispose()
        {
            component.Dispose();
        }

        public void opening()
        {
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

        public void _animationCallback(LayoutContainer oldChild)
        {
            Dispose();
            InputManager.Instance.refreshMouseWidget();
        }

        void layoutContainer_LayoutChanged()
        {
            component.topLevelResized();
        }
    }
}
