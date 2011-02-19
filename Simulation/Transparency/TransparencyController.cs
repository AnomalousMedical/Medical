using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TransparencyController
    {
        public static event EventHandler ActiveTransparencyStateChanged;

        public static readonly String DefaultTransparencyState = "Default";

        static NaturalSort<RenderGroup> sorter = new NaturalSort<RenderGroup>();
        static SortedList<RenderGroup, TransparencyGroup> groups = new SortedList<RenderGroup, TransparencyGroup>(sorter);
        static List<TransparencyInterface> transparencyInterfaces = new List<TransparencyInterface>();
        static List<String> transparencyStateNames = new List<string>();

        static TransparencyController()
        {
            transparencyStateNames.Add(DefaultTransparencyState);
        }

        internal static void addTransparencyObject(TransparencyInterface alphaObject)
        {
            if (!groups.ContainsKey(alphaObject.RenderGroup))
            {
                groups.Add(alphaObject.RenderGroup, new TransparencyGroup(alphaObject.RenderGroup));
            }
            groups[alphaObject.RenderGroup].addTransparencyObject(alphaObject);
            //Add all the needed transparency states.
            for (int i = 1; i < transparencyStateNames.Count; ++i)
            {
                alphaObject.createTransparencyState();
            }
            alphaObject.ActiveTransparencyState = TransparencyStateIndex;
            transparencyInterfaces.Add(alphaObject);
        }

        internal static void removeTransparencyObject(TransparencyInterface alphaObject)
        {
            if (groups.ContainsKey(alphaObject.RenderGroup))
            {
                TransparencyGroup group = groups[alphaObject.RenderGroup];
                group.removeTransparencyObject(alphaObject);
                if (group.isEmpty())
                {
                    groups.Remove(alphaObject.RenderGroup);
                }
                transparencyInterfaces.Remove(alphaObject);
            }
        }

        public static TransparencyGroup getTransparencyGroup(RenderGroup group)
        {
            TransparencyGroup ret = null;
            groups.TryGetValue(group, out ret);
            return ret;
        }

        public static IEnumerable<TransparencyGroup> getGroupIter()
        {
            return groups.Values;
        }

        public static void createTransparencyState(String name)
        {
            if (!transparencyStateNames.Contains(name))
            {
                foreach (TransparencyInterface transInterface in transparencyInterfaces)
                {
                    transInterface.createTransparencyState();
                }
                transparencyStateNames.Add(name);
            }
            else
            {
                throw new Exception(String.Format("Transparency state {0} already exists.", name));
            }
        }

        public static void removeTransparencyState(String name)
        {
            int stateIndex = transparencyStateNames.IndexOf(name);
            if (stateIndex != -1)
            {
                transparencyStateNames.Remove(name);
                foreach (TransparencyInterface transInterface in transparencyInterfaces)
                {
                    transInterface.removeTransparencyState(stateIndex);
                }
                if (TransparencyStateIndex == stateIndex)
                {
                    TransparencyStateIndex = 0;
                }
                else if (TransparencyStateIndex > stateIndex)
                {
                    TransparencyStateIndex--;
                }
            }
        }

        public static bool hasTransparencyState(String name)
        {
            return transparencyStateNames.IndexOf(name) != -1;
        }

        public static void applyTransparencyState(String name)
        {
            int stateIndex = transparencyStateNames.IndexOf(name);
            if (stateIndex != -1)
            {
                foreach (TransparencyInterface transInterface in transparencyInterfaces)
                {
                    transInterface.applyTransparencyState(stateIndex);
                }
            }
        }

        /// <summary>
        /// Make a copy of the list of all Transparency Interfaces.
        /// </summary>
        /// <returns>A copy of the list of all TransparencyInterfaces. These can be modified and will have an effect on the scene.</returns>
        public static List<TransparencyInterface> getTransparencyList()
        {
            return new List<TransparencyInterface>(transparencyInterfaces);
        }

        /// <summary>
        /// This is the transparency state that will be modified when set alpha
        /// or blend functions are called. This does not mean that the scene is
        /// currently setup with this transparency, however. That must be
        /// modified by applyTransparencyState.
        /// </summary>
        public static String ActiveTransparencyState
        {
            get
            {
                return transparencyStateNames[TransparencyStateIndex];
            }
            set
            {
                int stateIndex = transparencyStateNames.IndexOf(value);
                if (stateIndex != -1 && stateIndex != TransparencyStateIndex)
                {
                    TransparencyStateIndex = stateIndex;
                    
                    if (ActiveTransparencyStateChanged != null)
                    {
                        ActiveTransparencyStateChanged.Invoke(null, EventArgs.Empty);
                    }
                }
            }
        }

        private static int transparencyStateIndex = 0;//Do not modify directly, use property instead.
        private static int TransparencyStateIndex
        {
            get
            {
                return transparencyStateIndex;
            }
            set
            {
                transparencyStateIndex = value;
                foreach (TransparencyInterface transInterface in transparencyInterfaces)
                {
                    transInterface.ActiveTransparencyState = transparencyStateIndex;
                }
            }
        }
    }
}
