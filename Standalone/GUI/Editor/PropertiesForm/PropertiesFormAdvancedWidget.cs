using Engine;
using Engine.Editing;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class PropertiesFormAdvancedWidget : Component
    {
        public event Action<PropertiesFormAdvancedWidget> OnExpandToggle;

        private TextBox caption;
        private CheckButton expandButton;
        private int defaultHeight;
        private Widget childArea;

        protected StretchLayoutContainer flowLayout = new StretchLayoutContainer(StretchLayoutContainer.LayoutType.Vertical, 3, new IntVector2(0, 0));
        private List<PropertiesFormComponent> components = new List<PropertiesFormComponent>();
        private MyGUILayoutContainer layoutContainer;

        public PropertiesFormAdvancedWidget(Widget parentWidget)
            :base("Medical.GUI.Editor.PropertiesForm.PropertiesFormAdvancedWidget.layout")
        {
            defaultHeight = widget.Height;
            widget.setSize(parentWidget.Width, defaultHeight);
            widget.attachToWidget(parentWidget);

            caption = (TextBox)widget.findWidget("Caption");
            caption.Caption = "Advanced";

            expandButton = new CheckButton((Button)widget.findWidget("ExpandButton"));
            expandButton.CheckedChanged += new MyGUIEvent(expandButton_CheckedChanged);

            childArea = widget.findWidget("ChildArea");

            layoutContainer = new MyGUILayoutContainer(widget);
            layoutContainer.LayoutChanged += layoutContainer_LayoutChanged;
        }

        public override void Dispose()
        {
            clear();
            base.Dispose();
        }

        public void clear()
        {
            flowLayout.clearChildren();
            foreach (PropertiesFormComponent component in components)
            {
                component.Dispose();
            }
            components.Clear();
        }

        public void addComponent(PropertiesFormComponent component)
        {
            components.Add(component);
            flowLayout.addChild(component.Container);
        }

        void expandButton_CheckedChanged(Widget source, EventArgs e)
        {
            if (Expanded)
            {
                int height = flowLayout.DesiredSize.Height;
                height += defaultHeight;
                widget.setSize(widget.Width, height);
            }
            else
            {
                widget.setSize(widget.Width, defaultHeight);
            }
            layoutContainer.changeDesiredSize(new IntSize2(widget.Width, widget.Height), false);

            if (OnExpandToggle != null)
            {
                OnExpandToggle.Invoke(this);
            }
        }

        public bool Expanded
        {
            get
            {
                return !expandButton.Checked;
            }
            set
            {
                expandButton.Checked = !value;
            }
        }

        public Widget ComponentWidget
        {
            get
            {
                return childArea;
            }
        }

        public LayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }

        void layoutContainer_LayoutChanged()
        {
            if (Expanded)
            {
                int height = flowLayout.DesiredSize.Height;
                int width = widget.Width;
                flowLayout.WorkingSize = new IntSize2(width, height);
                flowLayout.layout();
            }
        }
    }
}
