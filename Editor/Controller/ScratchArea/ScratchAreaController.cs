using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    enum ScratchAreaCustomQueries
    {
        GetClipboard
    }

    partial class ScratchAreaController
    {
        private ScratchAreaFolder rootFolder;
        private SaveableClipboard clipboard;

        public ScratchAreaController(SaveableClipboard clipboard)
        {
            this.clipboard = clipboard;

            rootFolder = new ScratchAreaRootFolder("Scratch Area", MedicalConfig.UserDocRoot);
            rootFolder.loadInfo();
        }

        public SaveableClipboard Clipboard
        {
            get
            {
                return clipboard;
            }
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
