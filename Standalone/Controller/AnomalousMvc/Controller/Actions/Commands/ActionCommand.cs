using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.Controller.AnomalousMvc
{
    public abstract partial class ActionCommand : Saveable
    {
        public ActionCommand()
        {

        }

        public abstract void execute(AnomalousMvcContext context);

        public abstract String Type { get; }

        protected ActionCommand(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, BehaviorSaveMemberScanner.Scanner);
        }
    }

    partial class ActionCommand
    {
        [DoNotSave]
        protected EditInterface editInterface = null;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    createEditInterface();
                }
                return editInterface;
            }
        }

        protected virtual void createEditInterface()
        {
            editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, Type, null);
            customizeEditInterface(editInterface);
        }

        protected virtual void customizeEditInterface(EditInterface editInterface)
        {

        }
    }
}
