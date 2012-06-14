using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;

namespace Medical.GUI
{
    abstract class ConstrainableFormComponent : PropertiesFormLayoutComponent
    {
        public ConstrainableFormComponent(EditableProperty property, Widget parent, String layoutFile)
            :base(property, parent, layoutFile)
        {

        }

        public abstract void setConstraints(ReflectedMinMaxEditableProperty minMaxProp);
    }
}
