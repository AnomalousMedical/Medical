using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Saving;
using Engine;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    public abstract class DataField : Saveable
    {
        [DoNotSave]
        private EditInterface editInterface;

        public DataField(String name)
        {
            this.Name = name;
        }

        public abstract DataControl createControl(Widget parentWidget);

        public abstract String Type { get; }

        public String Name { get; set; }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, TimelineGUIEditMemberScanner.Scanner, String.Format("{0} - {1}", Name, Type), null);
                customizeEditInterface(editInterface);
            }
            return editInterface;
        }

        protected virtual void customizeEditInterface(EditInterface editInterface)
        {

        }

        protected DataField(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }
    }
}
