using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class TeethController
    {
        static Dictionary<String, Tooth> teeth = new Dictionary<string, Tooth>();
        static SimObjectMover teethMover;

        static TeethController()
        {
            HighlightContacts = false;
        }

        public static void addTooth(String name, Tooth tooth)
        {
            teeth.Add(name, tooth);
            if (teethMover != null)
            {
                teethMover.addMovableObject(name, tooth);
            }
        }

        public static void removeTooth(String name)
        {
            if (teethMover != null)
            {
                teethMover.removeMovableObject(teeth[name]);
            }
            teeth.Remove(name);
        }

        public static bool hasTooth(String name)
        {
            return teeth.ContainsKey(name);
        }

        public static Tooth getTooth(String name)
        {
            Tooth ret;
            teeth.TryGetValue(name, out ret);
            return ret;
        }

        public static void adaptAllTeeth(bool adapt)
        {
            foreach (Tooth tooth in teeth.Values)
            {
                tooth.Adapt = adapt;
            }
        }

        public static void adaptSingleTooth(String name, bool adapt)
        {
            teeth[name].Adapt = adapt;
        }

        public static void setAllOffsets(Vector3 value)
        {
            foreach (Tooth tooth in teeth.Values)
            {
                tooth.Offset = value;
            }
        }

        public static void setAllRotations(Quaternion value)
        {
            foreach (Tooth tooth in teeth.Values)
            {
                tooth.Rotation = value;
            }
        }

        /// <summary>
        /// Returns true if all unextracted teeth are touching.
        /// </summary>
        /// <returns>True if all unextracted teeth are touching.</returns>
        public static bool allTeethTouching()
        {
            bool touching = true;
            foreach (Tooth tooth in teeth.Values)
            {
                if (!tooth.Extracted && !tooth.MakingContact)
                {
                    touching = false;
                    break;
                }
            }
            return touching;
        }

        /// <summary>
        /// Returns true if any unextracted teeth are touching.
        /// </summary>
        /// <returns>True if any unextracted teeth are touching.</returns>
        public static bool anyTeethTouching()
        {
            foreach (Tooth tooth in teeth.Values)
            {
                if (!tooth.Extracted && tooth.MakingContact)
                {
                    return true;
                }
            }
            return false;
        }

        public static TeethState createTeethState()
        {
            TeethState state = new TeethState();
            foreach (Tooth tooth in teeth.Values)
            {
                state.addPosition(new ToothState(tooth.Owner.Name, tooth.Extracted, tooth.Offset, tooth.Rotation));
            }
            return state;
        }

        public static void setTeethLoose(bool loose)
        {
            foreach (Tooth tooth in teeth.Values)
            {
                tooth.Loose = loose;
            }
        }

        public static void showTeethTools(bool topVisible, bool bottomVisible)
        {
            foreach (Tooth tooth in teeth.Values)
            {
                if (tooth.IsTopTooth)
                {
                    tooth.ShowTools = topVisible;
                }
                else
                {
                    tooth.ShowTools = bottomVisible;
                }
            }
        }

        /// <summary>
        /// Call this function before a screenshot is rendered to hide the
        /// movement tools if you wish them hidden in the screenshot. This
        /// function is setup to consume as an EventHandler, but it does not
        /// actually use the arguments.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        public static void ScreenshotRenderStarted(Object sender, EventArgs e)
        {
            teethMover.setDrawingSurfaceVisible(false);
        }

        /// <summary>
        /// Call this function after a screenshot is rendered to show the
        /// movement tools if you hid them with ScreenshotRenderStarted. This
        /// function is setup to consume as an EventHandler, but it does not
        /// actually use the arguments.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        public static void ScreenshotRenderCompleted(Object sender, EventArgs e)
        {
            teethMover.setDrawingSurfaceVisible(true);
        }

        public static bool HighlightContacts { get; set; }

        public static SimObjectMover TeethMover
        {
            get
            {
                return teethMover;
            }
            set
            {
                teethMover = value;
            }
        }
    }
}
