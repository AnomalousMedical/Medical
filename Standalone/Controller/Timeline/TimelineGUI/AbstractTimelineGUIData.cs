using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine;
using Engine.Saving;
using Engine.Attributes;
using Engine.Reflection;

namespace Medical
{
    public abstract class AbstractTimelineGUIData : TimelineGUIData, ReflectedEditablePropertyProvider
    {
        [DoNotSave]
        private EditInterface editInterface;

        [DoNotSave]
        private static CopySaver copySaver = new CopySaver();

        public AbstractTimelineGUIData()
        {

        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, TimelineGUIEditMemberScanner.Scanner, Name, null, this);
                customizeEditInterface(editInterface);
            }
            return editInterface;
        }

        public virtual void customizeEditInterface(EditInterface editInterface)
        {

        }

        public bool addProperties(MemberWrapper memberWrapper, Object instance, EditInterface editInterface)
        {
            if (memberWrapper.getWrappedType() == typeof(LayerState))
            {
                editInterface.addEditableProperty(new LayerStateEditableProperty(instance, memberWrapper));
                return true;
            }
            return false;
        }

        public TimelineGUIData createCopy()
        {
            return copySaver.copy<AbstractTimelineGUIData>(this);
        }

        public abstract String Name { get; }

        protected AbstractTimelineGUIData(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }
    }
}
