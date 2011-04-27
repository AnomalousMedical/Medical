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

        public AbstractAnatomyCommand(AnatomyCommandUIType uiType)
            :this(uiType, float.MinValue, float.MaxValue)
        {
            
        }

        public AbstractAnatomyCommand(AnatomyCommandUIType uiType, float minValue, float maxValue)
            :base()
        {
            this.UIType = uiType;
            NumericValueMin = minValue;
            NumericValueMax = maxValue;
        }

        public event AnatomyNumericValueChanged NumericValueChanged;

        public event AnatomyBooleanValueChanged BooleanValueChanged;

        public abstract void link(SimObject owner);

        public AnatomyCommandUIType UIType { get; private set; }

        private float numericValue = 0.0f;

        [DoNotCopy]
        public virtual float NumericValue
        {
            get
            {
                return numericValue;
            }
            set
            {
                if (numericValue != value)
                {
                    numericValue = value;
                    if (NumericValueChanged != null)
                    {
                        NumericValueChanged.Invoke(this, value);
                    }
                }
            }
        }

        public float NumericValueMin { get; set; }

        public float NumericValueMax { get; set; }

        private bool booleanValue = false;

        [DoNotCopy]
        public virtual bool BooleanValue
        {
            get
            {
                return booleanValue;
            }
            set
            {
                if (booleanValue != value)
                {
                    booleanValue = value;
                    if (BooleanValueChanged != null)
                    {
                        BooleanValueChanged.Invoke(this, value);
                    }
                }
            }
        }

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
    }
}
