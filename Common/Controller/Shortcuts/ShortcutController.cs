using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Logging;

namespace Medical.Controller
{
    public class ShortcutController
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;

        private List<ShortcutGroup> groups = new List<ShortcutGroup>();
        private bool controlPressed = false;

        public ShortcutController()
        {

        }

        public ShortcutGroup createOrRetrieveGroup(String name)
        {
            foreach (ShortcutGroup group in groups)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }
            ShortcutGroup newGroup = new ShortcutGroup(name);
            groups.Add(newGroup);
            return newGroup;
        }

        public void processShortcuts(ref Message msg)
        {
            if (msg.Msg == WM_KEYDOWN)
            {
                Keys pressedKey = (Keys)msg.WParam;
                if (pressedKey == Keys.ControlKey)
                {
                    controlPressed = true;
                }
                else
                {
                    foreach (ShortcutGroup group in groups)
                    {
                        group.process(pressedKey, controlPressed); 
                    }
                }
            }
            else if (msg.Msg == WM_KEYUP)
            {
                if ((Keys)msg.WParam == Keys.ControlKey)
                {
                    controlPressed = false;
                }
            }
        }
    }
}
