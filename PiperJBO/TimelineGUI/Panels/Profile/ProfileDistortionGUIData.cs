using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.GUI
{
    class ProfileDistortionGUIData : TimelineWizardPanelData
    {        
        public ProfileDistortionGUIData()
        {
            RightCamera = new CameraPosition();
            RightMidCamera = new CameraPosition();
            MidlineCamera = new CameraPosition();
            LeftMidCamera = new CameraPosition();
            LeftSideCamera = new CameraPosition();
        }

        [Editable]
        public CameraPosition RightCamera { get; set; }

        [Editable]
        public CameraPosition RightMidCamera { get; set; }

        [Editable]
        public CameraPosition MidlineCamera { get; set; }

        [Editable]
        public CameraPosition LeftMidCamera { get; set; }

        [Editable]
        public CameraPosition LeftSideCamera { get; set; }

        public override string Name
        {
            get { return "ProfileDistortionGUIData"; }
        }

        protected ProfileDistortionGUIData(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
