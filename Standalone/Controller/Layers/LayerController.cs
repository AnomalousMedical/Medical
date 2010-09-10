using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;
using Engine.Resources;
using Engine;

namespace Medical
{
    public delegate void LayerControllerEvent(LayerController controller);

    public class LayerController : IDisposable
    {
        public event LayerControllerEvent LayerStateSetChanged;
        public event LayerControllerEvent LayerStateApplied;

        private LayerStateSet currentLayers;
        private XmlSaver xmlSaver = new XmlSaver();

        private String lastLayersFile;

        public LayerController()
        {

        }

        public void Dispose()
        {
            if(currentLayers != null)
            {
                currentLayers.Dispose();
            }
        }

        public void applyLayerState(String name)
        {
            applyLayerState(currentLayers.getState(name));
        }

        public void applyLayerState(LayerState state)
        {
            state.apply();
            if (LayerStateApplied != null)
            {
                LayerStateApplied.Invoke(this);
            }
        }

        public void instantlyApplyLayerState(String name, bool reportChanges)
        {
            instantlyApplyLayerState(currentLayers.getState(name), reportChanges);
        }

        public void instantlyApplyLayerState(LayerState state, bool reportChanges)
        {
            state.instantlyApply();
            if (reportChanges && LayerStateApplied != null)
            {
                LayerStateApplied.Invoke(this);
            }
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

                VirtualFileSystem archive = VirtualFileSystem.Instance;
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

        public void mergeLayerSet(String filename)
        {
            if (filename != lastLayersFile)
            {
                lastLayersFile = filename;

                VirtualFileSystem archive = VirtualFileSystem.Instance;
                using (XmlTextReader xmlReader = new XmlTextReader(archive.openStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    using (LayerStateSet states = xmlSaver.restoreObject(xmlReader) as LayerStateSet)
                    {
                        if (states == null)
                        {
                            throw new Exception(String.Format("Could not load a LayerStateSet out of the file {0}.", filename));
                        }
                        foreach (LayerState state in states.LayerStates)
                        {
                            if (!currentLayers.hasState(state.Name))
                            {
                                currentLayers.addState(state);
                            }
                        }
                    }
                    if (LayerStateSetChanged != null)
                    {
                        LayerStateSetChanged.Invoke(this);
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
                if (currentLayers != null)
                {
                    currentLayers.Dispose();
                }
                currentLayers = value;
                if (LayerStateSetChanged != null)
                {
                    LayerStateSetChanged.Invoke(this);
                }
            }
        }
    }
}
