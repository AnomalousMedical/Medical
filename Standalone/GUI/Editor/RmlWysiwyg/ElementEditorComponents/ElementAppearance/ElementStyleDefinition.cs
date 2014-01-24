using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public abstract class ElementStyleDefinition
    {
        private EditInterface editInterface;

        public event Action<ElementStyleDefinition> Changed;

        public ElementStyleDefinition()
        {

        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, "Appearance");
            }
            return editInterface;
        }

        protected void fireChanged()
        {
            if (Changed != null)
            {
                Changed.Invoke(this);
            }
        }

        public abstract bool buildClassList(StringBuilder classes);

        public abstract bool buildStyleAttribute(StringBuilder styleAttribute);
    }
}
