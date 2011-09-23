using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI
{
    class CondylarDegenerationGUIData : TimelineWizardPanelData
    {
        public CondylarDegenerationGUIData()
        {
            NormalCamera = new CameraPosition();
            ShowOsteophyteCamera = new CameraPosition();
        }

        [Editable]
        public CameraPosition NormalCamera { get; set; }

        [Editable]
        public CameraPosition ShowOsteophyteCamera { get; set; }

        public override string Name
        {
            get { return "CondylarDegenerationGUIData"; }
        }

        protected CondylarDegenerationGUIData(LoadInfo info)
            :base(info)
        {

        }
    }
}
