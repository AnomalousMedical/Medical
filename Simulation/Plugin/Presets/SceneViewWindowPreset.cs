using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    public partial class SceneViewWindowPreset : Saveable
    {
        String name;
        Vector3 position;
        Vector3 lookAt;
        String parentWindow;
        WindowAlignment windowPosition = WindowAlignment.Top;

        public SceneViewWindowPreset(String name, Vector3 position, Vector3 lookAt)
        {
            this.name = name;
            this.position = position;
            this.lookAt = lookAt;
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        [Editable]
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        [Editable]
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

        [Editable]
        public String ParentWindow
        {
            get
            {
                return parentWindow;
            }
            set
            {
                parentWindow = value;
            }
        }

        [Editable]
        public WindowAlignment WindowPosition
        {
            get
            {
                return windowPosition;
            }
            set
            {
                windowPosition = value;
            }
        }

        protected SceneViewWindowPreset(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
        }
    }

    public partial class SceneViewWindowPreset
    {
        [DoNotSave]
        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, String.Format("{0} - Preset", Name));
            }
            return editInterface;
        }
    }
}
