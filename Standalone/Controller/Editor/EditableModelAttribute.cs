using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    class EditableModelAttribute : EditableAttribute
    {
        private Type assignableFromType;

        public EditableModelAttribute(Type assignableFromType)
        {
            this.assignableFromType = assignableFromType;
        }

        public override EditableProperty createEditableProperty(MemberWrapper memberWrapper, object target)
        {
            return new ModelBrowserEditableProperty(memberWrapper.getWrappedName(), memberWrapper, target, assignableFromType);
        }
    }
}
