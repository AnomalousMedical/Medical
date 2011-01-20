using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical
{
    public class NavigationState
    {
        private String name;
        private Vector3 lookAt;
        private Vector3 translation;
        private bool hidden;
        private KeyboardButtonCode shortcutKey = KeyboardButtonCode.KC_UNASSIGNED;

        public NavigationState(String name, Vector3 lookAt, Vector3 translation, bool hidden)
        {
            this.name = name;
            this.translation = translation;
            this.lookAt = lookAt;
            this.hidden = hidden;
        }

        public NavigationState(String name, Vector3 lookAt, Vector3 translation, bool hidden, KeyboardButtonCode shortcutKey)
            :this(name, lookAt, translation, hidden)
        {
            this.shortcutKey = shortcutKey;
        }

        public Vector3 LookAt
        {
            get
            {
                return lookAt;
            }
            set
            {
                lookAt = value;
            }
        }

        public Vector3 Translation
        {
            get
            {
                return translation;
            }
            set
            {
                translation = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            internal set
            {
                name = value;
            }
        }

        public bool Hidden
        {
            get
            {
                return hidden;
            }
            set
            {
                hidden = value;
            }
        }

        public KeyboardButtonCode ShortcutKey
        {
            get
            {
                return shortcutKey;
            }
            set
            {
                shortcutKey = value;
            }
        }
    }
}
