using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Engine.ObjectManagement;
using Engine.Attributes;

namespace Medical
{
    /// <summary>
    /// This class provides stubbed implementations of all of the AnatomyCommand's methods. Used to reduce code in subclasses.
    /// </summary>
    public abstract class AbstractAnatomyCommand : AnatomyCommand
    {
        protected static MemberScanner memberScanner = new MemberScanner();

        static AbstractAnatomyCommand()
        {
            memberScanner.ProcessFields = true;
            memberScanner.Filter = new EditableAttributeFilter();
        }

        public event AnatomyNumericValueChanged NumericValueChanged;

        public event AnatomyBooleanValueChanged BooleanValueChanged;

        public abstract void link(SimObject owner);

        public abstract AnatomyCommandUIType UIType { get; }

        [DoNotCopy]
        public abstract float NumericValue { get; set; }

        public abstract float NumericValueMin { get; }

        public abstract float NumericValueMax { get; }

        [DoNotCopy]
        public abstract bool BooleanValue { get; set; }

        public virtual void execute()
        {

        }
        
        public EditInterface createEditInterface()
        {
		    EditInterface editInterface = ReflectedEditInterface.createEditInterface(this, memberScanner, GetType().Name, null);
	        return editInterface;
        }

        public bool Valid { get; set; }

        public abstract void getInfo(SaveInfo info);

        protected void fireNumericValueChanged(float value)
        {
            if (NumericValueChanged != null)
            {
                NumericValueChanged.Invoke(this, value);
            }
        }

        protected void fireBooleanValueChanged(bool value)
        {
            if (BooleanValueChanged != null)
            {
                BooleanValueChanged.Invoke(this, value);
            }
        }
    }
}
