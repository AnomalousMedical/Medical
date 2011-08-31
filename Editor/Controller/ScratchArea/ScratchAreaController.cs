using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    partial class ScratchAreaController
    {
        private ScratchAreaFolder rootFolder;

        public ScratchAreaController()
        {
            rootFolder = new ScratchAreaRootFolder("Scratch Area", MedicalConfig.DocRoot);
            rootFolder.loadInfo();
        }
    }

    partial class ScratchAreaController
    {
        public EditInterface EditInterface
        {
            get
            {
                return rootFolder.EditInterface;
            }
        }
    }
}
