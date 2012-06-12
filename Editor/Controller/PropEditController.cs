using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class PropEditController : MovableObject
    {
        public event Action<float> DurationChanged;
        public event Action<ShowPropAction> ShowPropActionChanged;

        private ShowPropAction currentShowPropAction;
        private MovePropAction currentMovePropAction;
        private SimObjectMover simObjectMover;
        private float duration;

        private List<ShowPropAction> openProps = new List<ShowPropAction>();
        public event EventDelegate<PropEditController, ShowPropAction> PropClosed;
        public event EventDelegate<PropEditController, ShowPropAction> PropOpened;

        public PropEditController(SimObjectMover simObjectMover)
        {
            this.simObjectMover = simObjectMover;
            simObjectMover.ShowMoveTools = true;
            simObjectMover.ToolSize = 3.0f;
        }

        public ShowPropAction CurrentShowPropAction
        {
            get
            {
                return currentShowPropAction;
            }
            set
            {
                if (currentShowPropAction != null)
                {
                    simObjectMover.removeMovableObject(this);
                    simObjectMover.setDrawingSurfaceVisible(false);
                }
                if (currentMovePropAction != null)
                {
                    CurrentMovePropAction = null;
                }
                currentShowPropAction = value;
                if (currentShowPropAction != null)
                {
                    simObjectMover.setActivePlanes(MovementAxis.All, MovementPlane.All);
                    simObjectMover.addMovableObject("Prop", this);
                    simObjectMover.setDrawingSurfaceVisible(true);
                    if (ShowPropActionChanged != null)
                    {
                        ShowPropActionChanged.Invoke(currentShowPropAction);
                    }
                    Duration = currentShowPropAction.Duration;
                }
            }
        }

        public MovePropAction CurrentMovePropAction
        {
            get
            {
                return currentMovePropAction;
            }
            set
            {
                currentMovePropAction = value;
                if (currentMovePropAction != null)
                {
                    currentShowPropAction._movePreviewProp(currentMovePropAction.Translation, currentMovePropAction.Rotation);
                }
                else
                {
                    currentShowPropAction._movePreviewProp(currentShowPropAction.Translation, currentShowPropAction.Rotation);
                }
            }
        }

        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
                if (DurationChanged != null)
                {
                    DurationChanged.Invoke(duration);
                }
            }
        }

        public void addOpenProp(ShowPropAction prop)
        {
            openProps.Add(prop);
            prop.KeepOpen = true;
            if (PropOpened != null)
            {
                PropOpened.Invoke(this, prop);
            }
        }

        public void removeOpenProp(ShowPropAction prop)
        {
            openProps.Remove(prop);
            prop.KeepOpen = false;
            if (PropClosed != null)
            {
                PropClosed.Invoke(this, prop);
            }
        }

        public void removeOpenProp(int index)
        {
            removeOpenProp(openProps[index]);
        }

        public void removeAllOpenProps()
        {
            while (openProps.Count > 0)
            {
                removeOpenProp(openProps[0]);
            }
        }

        public void hideOpenProps()
        {
            foreach (ShowPropAction prop in openProps)
            {
                prop.KeepOpen = false;
            }
        }

        public void showOpenProps()
        {
            foreach (ShowPropAction prop in openProps)
            {
                prop.KeepOpen = true;
            }
        }

        #region MovableObject Members

        public Vector3 ToolTranslation
        {
            get
            {
                if (currentMovePropAction != null)
                {
                    return currentMovePropAction.Translation;
                }
                else if (currentShowPropAction != null)
                {
                    return currentShowPropAction.Translation;
                }
                else
                {
                    return Vector3.Zero;
                }
            }
        }

        public void move(Vector3 offset)
        {
            if (currentShowPropAction != null)
            {
                if (currentMovePropAction != null)
                {
                    currentMovePropAction.Translation += offset;
                    //currentShowPropAction._movePreviewProp(currentMovePropAction.Translation, currentMovePropAction.Rotation);
                }
                else
                {
                    currentShowPropAction.Translation += offset;
                }
            }
        }

        public Quaternion ToolRotation
        {
            get
            {
                if (currentMovePropAction != null)
                {
                    return currentMovePropAction.Rotation;
                }
                else if (currentShowPropAction != null)
                {
                    return currentShowPropAction.Rotation;
                }
                else
                {
                    return Quaternion.Identity;
                }
            }
        }

        public bool ShowTools
        {
            get { return true; }
        }

        public void rotate(ref Quaternion newRot)
        {
            if (currentShowPropAction != null)
            {
                if (currentMovePropAction != null)
                {
                    currentMovePropAction.Rotation = newRot;
                    //currentShowPropAction._movePreviewProp(currentMovePropAction.Translation, currentMovePropAction.Rotation);
                }
                else
                {
                    currentShowPropAction.Rotation = newRot;
                }
            }
        }

        public void alertToolHighlightStatus(bool highlighted)
        {

        }

        #endregion
    }
}
