using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;
using Engine.Resources;

namespace Medical
{
    public delegate void LayerControllerEvent(LayerController controller);

    public class LayerController
    {
        public event LayerControllerEvent LayerStateSetChanged;

        private LayerStateSet currentLayers;
        private XmlSaver xmlSaver = new XmlSaver();

        private String lastLayersFile;

        public LayerController()
        {

        }

        public void applyLayerState(String name)
        {
            LayerState state = currentLayers.getState(name);
            state.apply();
        }

        public void saveLayerStateSet(String filename)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(filename, Encoding.Unicode))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlSaver.saveObject(currentLayers, xmlWriter);
            }
        }

        public void loadLayerStateSet(String filename)
        {
            if (filename != lastLayersFile)
            {
                lastLayersFile = filename;

                using (Archive archive = FileSystem.OpenArchive(filename))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(archive.openStream(filename, FileMode.Open, FileAccess.Read)))
                    {
                        LayerStateSet states = xmlSaver.restoreObject(xmlReader) as LayerStateSet;
                        if (states == null)
                        {
                            throw new Exception(String.Format("Could not load a LayerStateSet out of the file {0}.", filename));
                        }
                        CurrentLayers = states;
                    }
                }
            }
        }

        public LayerStateSet CurrentLayers
        {
            get
            {
                return currentLayers;
            }
            set
            {
                currentLayers = value;
                if (LayerStateSetChanged != null)
                {
                    LayerStateSetChanged.Invoke(this);
                }
            }
        }
    }
}
