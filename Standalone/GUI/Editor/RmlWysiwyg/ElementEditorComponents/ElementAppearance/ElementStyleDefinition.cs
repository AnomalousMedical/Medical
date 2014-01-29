using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public abstract class ElementStyleDefinition : StyleDefinition
    {
        public abstract bool buildClassList(StringBuilder classes);

        public abstract bool buildStyleAttribute(StringBuilder styleAttribute);
    }
}
