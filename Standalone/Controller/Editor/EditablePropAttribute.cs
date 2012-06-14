using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    class EditablePropAttribute : EditableAttribute
    {
        public override EditableProperty createEditableProperty(MemberWrapper memberWrapper, object target)
        {
            return new PropBrowserEditableProperty(memberWrapper.getWrappedName(), memberWrapper, target);
        }
    }
}
