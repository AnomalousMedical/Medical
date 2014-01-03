using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public enum ViewType
    {
        Panel,
        Window
    }

    /// <summary>
    /// The name of an element in the layout chain that a layout container should go into.
    /// This class and any subclasses should be immutable.
    /// </summary>
    public class LayoutElementName : Saveable
    {
        public LayoutElementName(String name)
        {
            this.name = name;
        }

        private String name;
        public String Name
        {
            get
            {
                return name;
            }
        }

        public virtual String UniqueDerivedName
        {
            get
            {
                return Name;
            }
        }

        public virtual ViewLocations LocationHint
        {
            get
            {
                return ViewLocations.Floating;
            }
        }

        public virtual ViewType ViewType
        {
            get
            {
                return ViewType.Panel;
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

        public static bool operator == (LayoutElementName a, LayoutElementName b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.UniqueDerivedName == b.UniqueDerivedName;
        }

        public static bool operator !=(LayoutElementName a, LayoutElementName b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return UniqueDerivedName.GetHashCode();
        }

        protected LayoutElementName(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
        }
    }
}
