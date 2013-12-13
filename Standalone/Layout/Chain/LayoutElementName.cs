using Engine.Saving;
using Medical.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class LayoutElementName : SaveableEditableItem
    {
        public LayoutElementName(String name)
            :base(name)
        {
            
        }

        protected virtual String UniqueDerivedName
        {
            get
            {
                return Name;
            }
        }

        public override bool Equals(object obj)
        {
            LayoutElementName other = obj as LayoutElementName;
            if (other == null)
            {
                return false;
            }
            return this.UniqueDerivedName == other.UniqueDerivedName;
        }

        public override int GetHashCode()
        {
            return UniqueDerivedName.GetHashCode();
        }

        protected LayoutElementName(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
