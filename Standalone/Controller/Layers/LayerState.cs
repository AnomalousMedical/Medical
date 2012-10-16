using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Medical
{
    public class LayerState : Saveable
    {
        private String name;
        private LinkedList<LayerEntry> entries = new LinkedList<LayerEntry>();

        public LayerState(String name)
        {
            this.name = name;
        }

        /// <summary>
        /// Capture the current state of the transparency controller.
        /// </summary>
        public void captureState()
        {
            entries.Clear();
            foreach (TransparencyInterface trans in TransparencyController.TransparencyInterfaces)
            {
                if (trans.CurrentAlpha > Single.Epsilon)
                {
                    LayerEntry entry = new LayerEntry(trans);
                    entries.AddLast(entry);
                }
            }
        }

        public void apply()
        {
            timedApply(MedicalConfig.CameraTransitionTime);
        }

        public void timedApply(float time)
        {
            List<TransparencyInterface> unvisitedInterfaces = new List<TransparencyInterface>(TransparencyController.TransparencyInterfaces);
            foreach (LayerEntry entry in entries)
            {
                entry.timedApply(time, unvisitedInterfaces);
            }
            foreach (TransparencyInterface unvisited in unvisitedInterfaces)
            {
                unvisited.timedBlend(0.0f, time);
            }
        }

        public void instantlyApply()
        {
            List<TransparencyInterface> unvisitedInterfaces = new List<TransparencyInterface>(TransparencyController.TransparencyInterfaces);
            foreach (LayerEntry entry in entries)
            {
                entry.instantlyApply(unvisitedInterfaces);
            }
            foreach (TransparencyInterface unvisited in unvisitedInterfaces)
            {
                unvisited.CurrentAlpha = 0.0f;
            }
        }

        public void instantlyApplyBlendPercent(float percent)
        {
            List<TransparencyInterface> unvisitedInterfaces = new List<TransparencyInterface>(TransparencyController.TransparencyInterfaces);
            foreach (LayerEntry entry in entries)
            {
                entry.instantlyApplyBlendPercent(unvisitedInterfaces, percent);
            }
            foreach (TransparencyInterface unvisited in unvisitedInterfaces)
            {
                unvisited.CurrentAlpha = 0.0f;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public IEnumerable<LayerEntry> Entries
        {
            get
            {
                return entries;
            }
        }

        /// <summary>
        /// Remove all zero entries from this layer state. This is done on
        /// creation, but this method is around to upgrade old serialized
        /// LayerState instances if needed.
        /// </summary>
        private void trimLayers()
        {
            LinkedList<LayerEntry> originalList = entries;
            entries = new LinkedList<LayerEntry>();
            foreach (LayerEntry entry in originalList)
            {
                if (entry.AlphaValue > Single.Epsilon)
                {
                    entries.AddLast(entry);
                }
            }
        }

        #region Saveable Members

        private const string NAME = "Name";
        private const string ENTRIES = "Entry";

        protected LayerState(LoadInfo info)
        {
            name = info.GetString(NAME);
            info.RebuildLinkedList<LayerEntry>(ENTRIES, entries);
        }

        public void getInfo(SaveInfo info)
        {
            //trimLayers();
            info.AddValue(NAME, name);
            info.ExtractLinkedList<LayerEntry>(ENTRIES, entries);
        }

        #endregion
    }
}
