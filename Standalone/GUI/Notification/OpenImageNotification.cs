using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical.GUI
{
    public class OpenImageNotification : AbstractNotification
    {
        private String filename;

        public OpenImageNotification(String filename)
            : base(String.Format("Image saved as {0}.\nClick here to open.", Path.GetFileName(filename)), "RenderIcon")
        {
            this.filename = filename;
        }

        public override void clicked()
        {
            OtherProcessManager.openLocalURL(filename);
        }
    }
}
