﻿using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class StyleDefinition
    {
        private EditInterface editInterface;

        public event Action<StyleDefinition> Changed;

        public StyleDefinition()
        {

        }

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, EditInterfaceName);
            }
            return editInterface;
        }

        protected virtual String EditInterfaceName
        {
            get
            {
                return "Appearance";
            }
        }

        protected void fireChanged()
        {
            if (Changed != null)
            {
                Changed.Invoke(this);
            }
        }

        protected void fireRefreshEditInterface()
        {
            editInterface.safeFireDataNeedsRefresh();
        }
    }
}
