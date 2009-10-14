using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.Controller
{
    /// <summary>
    /// This ShortcutCommand will forward its execute through an event. Allows
    /// for maximumum implementation lazyness.
    /// </summary>
    public class ShortcutEventCommand : ShortcutCommand
    {
        public delegate void ExecuteEvent();
        public event ExecuteEvent Execute;

        public ShortcutEventCommand(String name, Keys keyCode, bool requiresControl)
            :base(name, keyCode, requiresControl)
        {

        }

        protected override void execute()
        {
            if (Execute != null)
            {
                Execute.Invoke();
            }
        }
    }
}
