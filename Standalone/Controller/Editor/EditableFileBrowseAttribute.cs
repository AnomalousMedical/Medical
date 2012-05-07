using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    class EditableFileAttribute : EditableAttribute
    {
        private String fileSearchPattern;

        public EditableFileAttribute(String fileSearchPattern)
        {
            this.fileSearchPattern = fileSearchPattern;
        }

        public override EditableProperty createEditableProperty(MemberWrapper memberWrapper, object target)
        {
            return new FileBrowserEditableProperty(memberWrapper.getWrappedName(), memberWrapper, target, fileSearchPattern);
        }
    }
}
