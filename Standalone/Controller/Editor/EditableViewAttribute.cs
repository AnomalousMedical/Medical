using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    public class EditableViewAttribute : EditableAttribute
    {
        public override EditableProperty createEditableProperty(MemberWrapper memberWrapper, object target)
        {
            return new ViewBrowserEditableProperty(memberWrapper.getWrappedName(), memberWrapper, target);
        }
    }
}
