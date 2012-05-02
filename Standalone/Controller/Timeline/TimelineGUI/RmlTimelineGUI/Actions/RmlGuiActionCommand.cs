using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.RmlTimeline.Actions
{
    public abstract partial class RmlGuiActionCommand : Saveable
    {
        public RmlGuiActionCommand()
        {

        }

        public abstract void execute(RmlTimelineGUI gui);

        public abstract String Type { get; }

        protected RmlGuiActionCommand(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }
    }

    partial class RmlGuiActionCommand
    {
        [DoNotSave]
        private EditInterface editInterface = null;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, Type, null);
                    customizeEditInterface(editInterface);
                }
                return editInterface;
            }
        }

        protected virtual void customizeEditInterface(EditInterface editInterface)
        {

        }
    }
}
