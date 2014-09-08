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
        public event Action<float> MarkerMoved;

        private ShowPropAction currentShowPropAction;
        private MovePropAction currentMovePropAction;

        private SimObjectMover simObjectMover;
        private float duration = 0.0f;
        private float markerPosition = 0.0f;

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
                    simObjectMover.Visible = false;
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
                    simObjectMover.Visible = true;
                    Duration = currentShowPropAction.Duration;
                    ShowTools = true;
                }
                if (ShowPropActionChanged != null)
                {
                    ShowPropActionChanged.Invoke(currentShowPropAction);
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

        public float MarkerPosition
        {
            get
            {
                return markerPosition;
            }
            set
            {
                markerPosition = value;
                if (MarkerMoved != null)
                {
                    MarkerMoved.Invoke(markerPosition);
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

        public IEnumerable<ShowPropAction> OpenProps
        {
            get
            {
                return openProps;
            }
        }

        public void setRotateMode()
        {
            simObjectMover.ShowMoveTools = false;
            simObjectMover.ShowRotateTools = true;
        }

        public void setMoveMode()
        {
            simObjectMover.ShowMoveTools = true;
            simObjectMover.ShowRotateTools = false;
        }

        public Vector3 ToolTranslation
        {
            get
            {
                if (currentShowPropAction != null && currentShowPropAction.PropSimObject != null)
                {
                    return currentShowPropAction.PropSimObject.Translation;
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
                if (currentShowPropAction != null && currentShowPropAction.PropSimObject != null)
                {
                    return currentShowPropAction.PropSimObject.Rotation;
                }
                else
                {
                    return Quaternion.Identity;
                }
            }
        }

        public bool ShowTools { get; set; }

        public void rotate(ref Quaternion newRot)
        {
            if (currentShowPropAction != null)
            {
                if (currentMovePropAction != null)
                {
                    currentMovePropAction.Rotation = newRot;
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
    }
}