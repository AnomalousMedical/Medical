using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.Editor
{
    public abstract class EditableItem
    {
        protected EditableItem()
        {
            
        }

        public abstract String Name { get; set; }

        [DoNotSave]
        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = createEditInterface();
            }
            return editInterface;
        }

        /// <summary>
        /// If you need complete control over edit interface construction override this function.
        /// </summary>
        protected virtual EditInterface createEditInterface()
        {
            EditInterface editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, Name, null);
            customizeEditInterface(editInterface);
            return editInterface;
        }

        /// <summary>
        /// If you just want to a simple customization to the reflected edit interface override this function.
        /// </summary>
        /// <param name="editInterface"></param>
        protected virtual void customizeEditInterface(EditInterface editInterface)
        {

        }
    }
}
