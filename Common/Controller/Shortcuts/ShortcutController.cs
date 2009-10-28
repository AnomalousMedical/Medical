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
        private const int WM_ACTIVATEAPP = 0x1c;

        private List<ShortcutGroup> groups = new List<ShortcutGroup>();
        private bool controlPressed = false;
        private Keys lastKey = Keys.None;
        private bool enabled = true;

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
            if (enabled)
            {
                if (msg.Msg == WM_KEYDOWN)
                {
                    Keys pressedKey = (Keys)msg.WParam;
                    if (pressedKey == Keys.ControlKey)
                    {
                        controlPressed = true;
                    }
                    else if (pressedKey != lastKey)
                    {
                        lastKey = pressedKey;
                        bool controlPressed = this.controlPressed;
                        foreach (ShortcutGroup group in groups)
                        {
                            group.process(pressedKey, controlPressed);
                        }
                    }
                }
                else if (msg.Msg == WM_KEYUP)
                {
                    Keys pressedKey = (Keys)msg.WParam;
                    if (pressedKey == Keys.ControlKey)
                    {
                        controlPressed = false;
                    }
                    else if (pressedKey == lastKey)
                    {
                        lastKey = Keys.None;
                    }
                }
            }
        }

        /// <summary>
        /// Call this when the app looses focus.
        /// </summary>
        public void resetButtons()
        {
            controlPressed = false;
            lastKey = Keys.None;
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                if (enabled)
                {
                    resetButtons();
                }
            }
        }
    }
}
