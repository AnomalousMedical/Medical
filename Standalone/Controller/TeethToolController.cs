using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    static class TeethToolController
    {
        class ToothMover : MovableObject
        {
            private Tooth tooth;

            public ToothMover(Tooth tooth)
            {
                this.tooth = tooth;
            }

            public Vector3 ToolTranslation
            {
                get
                {
                    return tooth.Owner.Translation;
                }
            }

            public void move(Vector3 offset)
            {
                tooth.Offset += offset;
            }

            public Quaternion ToolRotation
            {
                get
                {
                    return tooth.Rotation;
                }
            }

            public bool ShowTools
            {
                get
                {
                    return tooth.ShowTools;
                }
            }

            public void rotate(ref Quaternion newRot)
            {
                tooth.Rotation = newRot;
            }

            public void alertToolHighlightStatus(bool highlighted)
            {
                tooth.ToolHighlight = highlighted;
            }
        }

        private static Dictionary<Tooth, ToothMover> toothMovers = new Dictionary<Tooth, ToothMover>();

        static TeethToolController()
        {
            TeethController.ToothAdded +=TeethController_ToothAdded;
            TeethController.ToothRemoved += TeethController_ToothRemoved;
        }

        public static SimObjectMover TeethMover { get; set; }

        static void TeethController_ToothAdded(Tooth tooth)
        {
            var toothMover = new ToothMover(tooth);
            toothMovers.Add(tooth, toothMover);
            TeethMover.addMovableObject(tooth.Owner.Name, toothMover);
        }

        static void TeethController_ToothRemoved(Tooth tooth)
        {
            ToothMover toothMover;
            if (toothMovers.TryGetValue(tooth, out toothMover))
            {
                toothMovers.Remove(tooth);
                TeethMover.removeMovableObject(toothMover);
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
            if (TeethMover != null)
            {
                TeethMover.setDrawingSurfaceVisible(false);
            }
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
            if (TeethMover != null)
            {
                TeethMover.setDrawingSurfaceVisible(true);
            }
        }

    }
}
