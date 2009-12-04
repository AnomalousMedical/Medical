using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Properties;
using Engine.Saving.XMLSaver;
using System.Xml;

namespace Medical.GUI
{
    class FullDistortionPicker : FileBrowserPickerPanel
    {
        private PresetState presetState;
        private XmlSaver xmlSaver = new XmlSaver();

        public FullDistortionPicker()
            :base("*.dst")
        {
            
        }

        public override void applyToState(MedicalState state)
        {
            if (presetState != null)
            {
                presetState.applyToState(state);
            }
        }

        protected override void onFileChosen(string filename)
        {
            using (XmlTextReader textReader = new XmlTextReader(filename))
            {
                presetState = xmlSaver.restoreObject(textReader) as PresetState;
            }
            showChanges(false);
        }
    }
}
