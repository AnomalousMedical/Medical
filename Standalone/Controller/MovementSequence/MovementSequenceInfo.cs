using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using Engine.Saving.XMLSaver;

namespace Medical.Controller
{
    public abstract class MovementSequenceInfo
    {
        public String Name { get; set; }

        public abstract MovementSequence loadSequence(XmlSaver xmlSaver);
    }
}
