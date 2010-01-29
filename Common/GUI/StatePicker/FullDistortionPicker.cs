using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Properties;
using Engine.Saving.XMLSaver;
using System.Xml;
using Engine.Resources;
using System.IO;

namespace Medical.GUI
{
    class FullDistortionPicker : FileBrowserPickerPanel
    {
        private PresetState presetState;
        private XmlSaver xmlSaver = new XmlSaver();

        public FullDistortionPicker(StatePickerPanelController panelController)
            :base("*.dst", panelController)
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
            using (Archive archive = FileSystem.OpenArchive(filename))
            {
                using (Stream stream = archive.openStream(filename, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                {
                    using (XmlTextReader textReader = new XmlTextReader(stream))
                    {
                        presetState = xmlSaver.restoreObject(textReader) as PresetState;
                    }
                }
            }
            showChanges(false);
        }
    }
}
