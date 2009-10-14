using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.Controller
{
    public abstract class ShortcutCommand
    {
        private String name;
        private Keys keyCode;
        private bool requiresControl;

        public ShortcutCommand(String name, Keys keyCode, bool requiresControl)
        {
            this.name = name;
            this.keyCode = keyCode;
            this.requiresControl = requiresControl;
        }

        public void process(Keys pressedKey, bool controlPressed)
        {
            if(requiresControl == controlPressed && pressedKey == keyCode)
            {
                execute();
            }
        }

        protected abstract void execute();

        public String Name
        {
            get
            {
                return name;
            }
        }

        public Keys KeyCode
        {
            get
            {
                return keyCode;
            }
            set
            {
                keyCode = value;
            }
        }

        public bool RequiresControl
        {
            get
            {
                return requiresControl;
            }
            set
            {
                requiresControl = value;
            }
        }
    }
}
