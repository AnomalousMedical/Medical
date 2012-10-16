using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class TransparencyController
    {
        public static event EventHandler ActiveTransparencyStateChanged;

        public static readonly String DefaultTransparencyState = "Default";

        static FastIteratorMap<String, TransparencyInterface> transparencyInterfaces = new FastIteratorMap<String, TransparencyInterface>();
        static List<String> transparencyStateNames = new List<string>();

        static TransparencyController()
        {
            transparencyStateNames.Add(DefaultTransparencyState);
        }

        internal static void addTransparencyObject(TransparencyInterface alphaObject)
        {
            try
            {
                transparencyInterfaces.Add(alphaObject.ObjectName, alphaObject);
                //Add all the needed transparency states.
                for (int i = 1; i < transparencyStateNames.Count; ++i)
                {
                    alphaObject.createTransparencyState();
                }
                alphaObject.ActiveTransparencyState = TransparencyStateIndex;
            }
            catch (ArgumentException)
            {
                throw new TransparencyException(String.Format("A Transparency Object named '{0}' from Sim Object '{1}' already exists. The Transparency Object that was stored for this name is '{2}'.", alphaObject.ObjectName, alphaObject.Owner.Name, transparencyInterfaces[alphaObject.ObjectName].Owner.Name));
            }
        }

        internal static void removeTransparencyObject(TransparencyInterface alphaObject)
        {
            transparencyInterfaces.Remove(alphaObject.ObjectName);
        }

        public static TransparencyInterface getTransparencyObject(String name)
        {
            TransparencyInterface transInter = null;
            transparencyInterfaces.TryGetValue(name, out transInter);
            return transInter;
        }

        public static IEnumerable<TransparencyInterface> TransparencyInterfaces
        {
            get
            {
                return transparencyInterfaces;
            }
        }

        public static void setAllAlphas(float alpha)
        {
            foreach (TransparencyInterface transInterface in transparencyInterfaces)
            {
                transInterface.CurrentAlpha = alpha;
            }
        }

        public static void smoothSetAllAlphas(float alpha, float duration)
        {
            foreach (TransparencyInterface transInterface in transparencyInterfaces)
            {
                transInterface.timedBlend(alpha, duration);
            }
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
