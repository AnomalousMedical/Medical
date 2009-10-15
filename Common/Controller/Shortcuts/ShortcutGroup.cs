using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.Controller
{
    /// <summary>
    /// This is a group of shortcuts similar to a namespace.
    /// </summary>
    public class ShortcutGroup
    {
        private String name;
        private List<ShortcutCommand> shortcuts = new List<ShortcutCommand>();

        internal ShortcutGroup(String name)
        {
            this.name = name;
        }

        public void addShortcut(ShortcutCommand shortcut)
        {
            shortcuts.Add(shortcut);
        }

        public void removeShortcut(ShortcutCommand shortcut)
        {
            shortcuts.Remove(shortcut);
        }

        public void clearShortcuts()
        {
            shortcuts.Clear();
        }

        public void process(Keys pressedKey, bool ctrlPressed)
        {
            foreach (ShortcutCommand cmd in shortcuts)
            {
                cmd.process(pressedKey, ctrlPressed);
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }
    }
}
