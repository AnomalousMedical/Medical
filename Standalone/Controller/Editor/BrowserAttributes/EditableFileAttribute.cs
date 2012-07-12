using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    public class EditableFileAttribute : EditableAttribute
    {
        private String fileSearchPattern;
        private String prompt;

        public EditableFileAttribute(String fileSearchPattern, String prompt)
        {
            this.fileSearchPattern = fileSearchPattern;
            this.prompt = prompt;
        }

        public override EditableProperty createEditableProperty(MemberWrapper memberWrapper, object target)
        {
            return new FileBrowserEditableProperty(memberWrapper.getWrappedName(), memberWrapper, target, fileSearchPattern, prompt);
        }
    }
}
