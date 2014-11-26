using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.IO;
using Engine.Editing;
using Engine;

namespace Medical
{
    public class LayerState : Saveable
    {
        private List<LayerEntry> entries = new List<LayerEntry>();

        /// <summary>
        /// Convienence method to create a layer state and capture the current state in one call.
        /// It is still safe to use the constructors directly as well.
        /// </summary>
        /// <returns></returns>
        public static LayerState CreateAndCapture()
        {
            LayerState layerState = new LayerState();
            layerState.captureState();
            return layerState;
        }

        public LayerState()
        {
            Easing = EasingFunction.EaseOutQuadratic;
        }

        public LayerState(IEnumerable<String> transparencyInterfaceNames, float overrideAlpha)
        {
            foreach (String name in transparencyInterfaceNames)
            {
                entries.Add(new LayerEntry(name, overrideAlpha));
            }
        }

        /// <summary>
        /// Capture the current state of the transparency controller.
        /// </summary>
        public void captureState()
        {
            entries.Clear();
            entries.Capacity = TransparencyController.TransparencyInterfaceCount; //We aren't going to have a capacity greater than this
            foreach (TransparencyInterface trans in TransparencyController.TransparencyInterfaces)
            {
                if (trans.CurrentAlpha > Single.Epsilon)
                {
                    LayerEntry entry = new LayerEntry(trans);
                    entries.Add(entry);
                }
            }
            entries.Capacity = entries.Count; //Trim the capcity down to the actual number of elements
        }

        public void copyFrom(LayerState source)
        {
            entries.Clear();
            entries.Capacity = source.entries.Count;
            foreach (LayerEntry entry in source.entries)
            {
                entries.Add(CopySaver.Default.copy(entry));
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

        /// <summary>
        /// Tries to determine if this layer state is the same as the given one. This will check the sizes of the entries lists,
        /// if they are not the same it will be false. If they are the same it will compare each entry, if the entries do not
        /// match name for name and alpha value for alpha value they will not be the same. Note that the order of the entries in
        /// both layer states must be the same or else they will still be considered different. The entries list is only enumerated
        /// one time. This is mostly useful to see if two layer states generated on the same program run with the same scene are the
        /// same (since the entries should always come out in the same order).
        /// </summary>
        /// <param name="other">The other entry.</param>
        /// <returns>True if they are the same and false otherwise.</returns>
        public bool isTheSameAs(LayerState other)
        {
            if(entries.Count == other.entries.Count)
            {
                for(int i = 0; i < entries.Count; ++i)
                {
                    var mine = entries[i];
                    var theirs = other.entries[i];
                    if(mine.TransparencyObject != theirs.TransparencyObject || mine.AlphaValue != theirs.AlphaValue)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
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
            List<LayerEntry> originalList = entries;
            entries = new List<LayerEntry>(originalList.Count);
            foreach (LayerEntry entry in originalList)
            {
                if (entry.AlphaValue > Single.Epsilon)
                {
                    entries.Add(entry);
                }
            }
        }

        #region Saveable Members

        private const string ENTRIES = "Entry";
        private const string EASING = "Easing";

        protected LayerState(LoadInfo info)
        {
            Easing = info.GetValue(EASING, EasingFunction.EaseOutQuadratic);
            entries.Capacity = TransparencyController.TransparencyInterfaceCount; //We aren't likely going to have a capacity greater than this, this helps reduce the number of resizes as we rebuild the list
            info.RebuildList<LayerEntry>(ENTRIES, entries);
            entries.Capacity = entries.Count; //Reduce the count down to the actual number needed.
        }

        public void getInfo(SaveInfo info)
        {
            //temporarily add the name to retain backward compatability with the older anomalous medical, this will not be reloaded in the
            //new version and should be erased when we don't need compatability anymore.
            info.AddValue("Name", "None");

            info.AddValue(EASING, EasingFunction.EaseOutQuadratic);
            info.ExtractList<LayerEntry>(ENTRIES, entries);
        }

        #endregion
    }
}
