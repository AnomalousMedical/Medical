﻿using System;
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
        protected static FilteredMemberScanner memberScanner = new FilteredMemberScanner();

        static AbstractAnatomyCommand()
        {
            memberScanner.ProcessFields = true;
            memberScanner.Filter = new EditableAttributeFilter();
        }

        public event AnatomyNumericValueChanged NumericValueChanged;
        public event AnatomyBooleanValueChanged BooleanValueChanged;

        public virtual void Dispose()
        {

        }

        public abstract bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref String errorMessage);

        public abstract AnatomyCommandUIType UIType { get; }

        [DoNotCopy]
        public abstract float NumericValue { get; set; }

        public abstract float NumericValueMin { get; }

        public abstract float NumericValueMax { get; }

        [DoNotCopy]
        public abstract bool BooleanValue { get; set; }

        public abstract String UIText { get; }

        public abstract void execute();

        public virtual bool allowDisplay(AnatomyCommandPermissions permissions)
        {
            //By default always let everything through
            return true;
        }
        
        public EditInterface createEditInterface()
        {
		    EditInterface editInterface = ReflectedEditInterface.createEditInterface(this, memberScanner, GetType().Name, null);
	        return editInterface;
        }

        public abstract AnatomyCommand createTagGroupCommand();

        public abstract void addToTagGroupCommand(AnatomyCommand tagGroupCommand);

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
