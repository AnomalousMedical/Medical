using Engine.Editing;
using Engine.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Editor
{
    public class EditableElementNameAttribute : EditableAttribute
    {
        public override EditableProperty createEditableProperty(MemberWrapper memberWrapper, object target)
        {
            return new ElementNameEditableProperty(memberWrapper.getWrappedName(), memberWrapper, target);
        }
    }
}
