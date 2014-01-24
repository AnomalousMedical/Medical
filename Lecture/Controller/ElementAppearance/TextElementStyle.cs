using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture
{
    class TextElementStyle : ElementStyleDefinition
    {
        public override bool buildClassList(StringBuilder classes)
        {
            return false;
        }

        public override bool buildStyleAttribute(StringBuilder styleAttribute)
        {
            return false;
        }

        [Editable]
        public bool Bold { get; set; }

        [Editable]
        public bool Italic { get; set; }

        [Editable]
        public bool Center { get; set; }
    }
}
