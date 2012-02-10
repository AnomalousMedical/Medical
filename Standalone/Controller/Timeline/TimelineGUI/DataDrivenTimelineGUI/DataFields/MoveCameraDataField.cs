using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Engine;

namespace Medical
{
    public class MoveCameraDataField : DataField
    {
        public MoveCameraDataField(String name)
            :base(name)
        {
            CameraPosition = new CameraPosition();
            CameraPosition.LookAt = new Vector3(0f, 0f, 0f);
            CameraPosition.Translation = new Vector3(0f, 0f, 150f);
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        [Editable]
        public CameraPosition CameraPosition { get; set; }

        public override string Type
        {
            get
            {
                return "Move Camera";
            }
        }

        protected MoveCameraDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
