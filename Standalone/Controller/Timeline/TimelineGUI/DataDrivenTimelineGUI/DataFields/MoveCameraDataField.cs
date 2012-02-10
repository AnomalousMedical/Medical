using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    public class MoveCameraDataField : DataField
    {
        public MoveCameraDataField(String name)
            :base(name)
        {
            CameraPosition = new CameraPosition();
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
