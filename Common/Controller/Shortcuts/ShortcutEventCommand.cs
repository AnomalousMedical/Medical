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
        public delegate void ExecuteEvent(ShortcutEventCommand shortcut);
        public event ExecuteEvent Execute;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keyCode"></param>
        /// <param name="requiresControl"></param>
        public ShortcutEventCommand(String name, Keys keyCode, bool requiresControl)
            :base(name, keyCode, requiresControl)
        {

        }

        /// <summary>
        /// Execute function. Calls the event.
        /// </summary>
        protected override void execute()
        {
            if (Execute != null)
            {
                Execute.Invoke(this);
            }
        }

        /// <summary>
        /// A user defined property for this shortcut.
        /// </summary>
        public object UserData { get; set; }
    }
}
