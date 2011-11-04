using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using Engine;
using System.Xml;
using Engine.Saving.XMLSaver;
using Logging;
using Engine.Resources;

namespace Medical.Controller
{
    class VirtualFSMovementSequenceInfo : MovementSequenceInfo
    {
        public String FileName { get; set; }

        public override MovementSequence loadSequence(XmlSaver xmlSaver)
        {
            MovementSequence loadingSequence = null;
            try
            {
                VirtualFileSystem archive = VirtualFileSystem.Instance;
                using (XmlTextReader xmlReader = new XmlTextReader(archive.openStream(FileName, FileMode.Open, FileAccess.Read)))
                {
                    loadingSequence = xmlSaver.restoreObject(xmlReader) as MovementSequence;
                    VirtualFileInfo fileInfo = archive.getFileInfo(FileName);
                    loadingSequence.Name = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not read muscle sequence {0}.\nReason: {1}.", FileName, e.Message);
            }
            return loadingSequence;
        }
    }
}
