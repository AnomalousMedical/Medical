using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Attributes;
using Engine;

namespace Medical.Editor
{
    public abstract class SaveableEditableItem : EditableItem, Saveable
    {
        private String name;

        protected SaveableEditableItem(String name)
        {
            this.name = name;
        }

        public override String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        protected SaveableEditableItem(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }
    }
}
