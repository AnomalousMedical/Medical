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

        private ShowPropAction currentShowPropAction;
        private MovePropAction currentMovePropAction;
        private SimObjectMover simObjectMover;

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
                currentShowPropAction = value;
                if (currentShowPropAction != null)
                {
                    simObjectMover.setActivePlanes(MovementAxis.All, MovementPlane.All);
                    simObjectMover.addMovableObject("Prop", this);
                    simObjectMover.setDrawingSurfaceVisible(true);
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

        public float Duration { get; set; }

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
                    //translationEdit.Caption = currentShowPropAction.Translation.ToString();
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
                    //Vector3 euler = showProp.Rotation.getEuler();
                    //euler *= 57.2957795f;
                    //rotationEdit.Caption = euler.ToString();
                }
            }
        }

        public void alertToolHighlightStatus(bool highlighted)
        {

        }

        #endregion
    }
}
