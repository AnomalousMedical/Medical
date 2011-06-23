using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Engine;

namespace Medical
{
    public abstract class EditableShowPropSubAction : ShowPropSubAction
    {
        [DoNotCopy]
        [DoNotSave]
        protected EditInterface editInterface;

        public EditableShowPropSubAction()
        {

        }

        [DoNotCopy]
        public virtual EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, BehaviorEditMemberScanner.Scanner, GetType().Name, null);
                }
                return editInterface;
            }
        }

        #region Saveable Members

        protected EditableShowPropSubAction(LoadInfo info)
            :base (info)
        {
            
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
        }

        #endregion
    }
}
