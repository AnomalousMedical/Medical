using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine;
using Engine.Saving;

namespace Medical
{
    public class MoveCameraChangeLayersDataField : DataField
    {
        public MoveCameraChangeLayersDataField(String name)
            :base(name)
        {
            CameraPosition = new CameraPosition();
            CameraPosition.LookAt = new Vector3(0f, 0f, 0f);
            CameraPosition.Translation = new Vector3(0f, 0f, 150f);

            Layers = new EditableLayerState("Layers");
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        [Editable]
        public EditableLayerState Layers { get; set; }

        [Editable]
        public CameraPosition CameraPosition { get; set; }

        public override string Type
        {
            get
            {
                return "Move Camera and Change Layers";
            }
        }

        protected MoveCameraChangeLayersDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
