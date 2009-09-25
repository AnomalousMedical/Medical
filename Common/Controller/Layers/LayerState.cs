using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class LayerState : Saveable
    {
        private String name;
        private LinkedList<LayerEntry> entries = new LinkedList<LayerEntry>();
        private bool hidden = false;

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
            foreach (TransparencyGroup group in TransparencyController.getGroupIter())
            {
                foreach (TransparencyInterface trans in group.getTransparencyObjectIter())
                {
                    LayerEntry entry = new LayerEntry(trans);
                    entries.AddLast(entry);
                }
            }
        }

        public void apply()
        {
            foreach(LayerEntry entry in entries)
            {
                entry.apply();
            }
        }

        public bool Hidden
        {
            get
            {
                return hidden;
            }
            set
            {
                hidden = value;
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

        #region Saveable Members

        private const string NAME = "Name";
        private const string HIDDEN = "Hidden";
        private const string ENTRIES = "Entry";

        protected LayerState(LoadInfo info)
        {
            name = info.GetString(NAME);
            hidden = info.GetBoolean(HIDDEN);
            info.RebuildLinkedList<LayerEntry>(ENTRIES, entries);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(HIDDEN, hidden);
            info.ExtractLinkedList<LayerEntry>(ENTRIES, entries);
        }

        #endregion
    }
}
