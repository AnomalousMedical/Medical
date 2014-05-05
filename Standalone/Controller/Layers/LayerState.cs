using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Engine.Editing;
using Engine;

namespace Medical
{
    public class LayerState : Saveable
    {
        private String name;
        private LinkedList<LayerEntry> entries = new LinkedList<LayerEntry>();

        public LayerState(String name)
        {
            this.name = name;
            Easing = EasingFunction.EaseOutQuadratic;
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

        public void buildFrom(IEnumerable<TransparencyInterface> transparencyInterfaces, float overrideAlpha)
        {
            entries.Clear();
            foreach (TransparencyInterface trans in transparencyInterfaces)
            {
                LayerEntry entry = new LayerEntry(trans, overrideAlpha);
                entries.AddLast(entry);
            }
        }

        public void copyFrom(LayerState source)
        {
            entries.Clear();
            foreach (LayerEntry entry in source.entries)
            {
                entries.AddLast(CopySaver.Default.copy(entry));
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
                entry.timedApply(time, unvisitedInterfaces, Easing);
            }
            foreach (TransparencyInterface unvisited in unvisitedInterfaces)
            {
                unvisited.timedBlend(0.0f, time, Easing);
            }
        }

        public void instantlyApplyTo(String transparencyStateName)
        {
            String activeTransaparencyState = TransparencyController.ActiveTransparencyState;
            TransparencyController.ActiveTransparencyState = transparencyStateName;
            instantlyApply();
            TransparencyController.ActiveTransparencyState = activeTransaparencyState;
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

        [Editable]
        public EasingFunction Easing { get; set; }

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
        private const string EASING = "Easing";

        protected LayerState(LoadInfo info)
        {
            name = info.GetString(NAME);
            Easing = info.GetValue(EASING, EasingFunction.EaseOutQuadratic);
            info.RebuildLinkedList<LayerEntry>(ENTRIES, entries);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(EASING, EasingFunction.EaseOutQuadratic);
            info.ExtractLinkedList<LayerEntry>(ENTRIES, entries);
        }

        #endregion
    }
}
