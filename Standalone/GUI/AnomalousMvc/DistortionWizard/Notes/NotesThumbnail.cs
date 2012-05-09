using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    partial class NotesThumbnail : Saveable
    {
        private CameraPosition cameraPosition;
        private EditableLayerState layerState;

        public NotesThumbnail()
        {
            cameraPosition = new CameraPosition();
            layerState = new EditableLayerState("");
        }

        public LayerState LayerState
        {
            get
            {
                return layerState;
            }
        }

        public CameraPosition CameraPosition
        {
            get
            {
                return cameraPosition;
            }
        }

        protected NotesThumbnail(LoadInfo info)
        {
            cameraPosition = info.GetValue<CameraPosition>("CameraPosition");
            layerState = info.GetValue<EditableLayerState>("LayerState");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("CameraPosition", cameraPosition);
            info.AddValue("LayerState", layerState);
        }
    }

    partial class NotesThumbnail
    {
        private EditInterface editInterface = null;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = new EditInterface("Thumbnail");
                    editInterface.addSubInterface(cameraPosition.getEditInterface("Camera Position", ReflectedEditInterface.DefaultScanner));
                    editInterface.addSubInterface(layerState.getEditInterface("Layer State", ReflectedEditInterface.DefaultScanner));
                }
                return editInterface;
            }
        }
    }
}
