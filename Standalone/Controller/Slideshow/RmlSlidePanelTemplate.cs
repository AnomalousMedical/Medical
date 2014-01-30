using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    public class RmlSlidePanelTemplate : RmlSlidePanel
    {
        public String Rml { get; set; }

        public override bool applyToExisting(Slide slide, SlidePanel panel, bool overwriteContent, EditorResourceProvider resourceProvider)
        {
            String rmlPath = this.getRmlFilePath(slide);
            if (overwriteContent || !resourceProvider.exists(rmlPath))
            {
                using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream(rmlPath), Encoding.UTF8))
                {
                    streamWriter.Write(Rml);
                }
            }
            return base.applyToExisting(slide, panel, overwriteContent, resourceProvider);
        }
    }
}
