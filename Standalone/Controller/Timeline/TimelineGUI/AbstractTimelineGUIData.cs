using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine;
using Engine.Saving;
using Engine.Attributes;

namespace Medical
{
    public abstract class AbstractTimelineGUIData : TimelineGUIData
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
                editInterface = ReflectedEditInterface.createEditInterface(this, BehaviorEditMemberScanner.Scanner, Name, null);
            }
            return editInterface;
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
