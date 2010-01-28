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
        public event LayerControllerEvent CurrentLayerStateChanged;

        private LayerStateSet currentLayers;
        private XmlSaver xmlSaver = new XmlSaver();

        private String lastLayersFile;
        private LayerState currentLayerState;

        public LayerController()
        {

        }

        public void applyLayerState(String name)
        {
            CurrentLayerState = currentLayers.getState(name);
        }

        /// <summary>
        /// Apply a layer state quicky without causing any disruption to the
        /// current layering including any transitions currently in effect. This
        /// will be valid until the next update. It is intended for use by the
        /// ImageRenderer so images can be captured with transparency that is
        /// different from the current transparency.
        /// </summary>
        /// <param name="name">The name of the state to apply.</param>
        /// <returns>A LayerState that can restore the scene to how it was before this fuction was called.</returns>
        public LayerState applyLayerStateTemporaryUndisruptive(String name)
        {
            LayerState currentConditions = new LayerState("CurrentConditions");
            currentConditions.captureState();

            LayerState tempState = currentLayers.getState(name);
            tempState.applyTemporaryUndisruptive();

            return currentConditions;
        }

        /// <summary>
        /// Call this function to restore the old conditions that were returned
        /// from applyLayerStateTemporaryUndisruptive.
        /// </summary>
        /// <param name="oldConditions"></param>
        public void restoreConditions(LayerState oldConditions)
        {
            oldConditions.applyTemporaryUndisruptive();
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

        public void mergeLayerSet(String filename)
        {
            if (filename != lastLayersFile)
            {
                lastLayersFile = filename;

                using (Archive archive = FileSystem.OpenArchive(filename))
                {
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
                currentLayerState = null;
                currentLayers = value;
                if (LayerStateSetChanged != null)
                {
                    LayerStateSetChanged.Invoke(this);
                }
            }
        }

        public LayerState CurrentLayerState
        {
            get
            {
                return currentLayerState;
            }
            set
            {
                if (currentLayerState != value)
                {
                    currentLayerState = value;
                    if (currentLayerState != null)
                    {
                        currentLayerState.apply();
                        if (CurrentLayerStateChanged != null)
                        {
                            CurrentLayerStateChanged.Invoke(this);
                        }
                    }
                }
            }
        }
    }
}
