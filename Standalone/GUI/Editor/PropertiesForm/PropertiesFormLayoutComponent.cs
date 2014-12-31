using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;
using Anomalous.GuiFramework;

namespace Medical.GUI
{
    public abstract class PropertiesFormLayoutComponent : Component, PropertiesFormComponent
    {
        private MyGUILayoutContainer layoutContainer;
        private EditableProperty editableProperty;

        public PropertiesFormLayoutComponent(EditableProperty property, Widget parent, String layoutFile)
            :base(layoutFile)
        {
            this.editableProperty = property;
            layoutContainer = new MyGUILayoutContainer(widget);
            widget.attachToWidget(parent);
        }

        public abstract void refreshData();

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }

        public EditableProperty Property
        {
            get
            {
                return editableProperty;
            }
        }
    }
}
