using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class MovePropProperties : TimelineDataPanel
    {
        private MovePropAction moveProp;
        private Edit translationEdit;
        private Edit rotationEdit;

        public MovePropProperties(Widget parent)
            : base(parent, "Medical.GUI.PropTimeline.SubActionProperties.MovePropProperties.layout")
        {
            translationEdit = mainWidget.findWidget("TranslationEdit") as Edit;

            rotationEdit = mainWidget.findWidget("RotationEdit") as Edit;
        }

        public override void setCurrentData(TimelineData data)
        {
            PropTimelineData propData = (PropTimelineData)data;
            moveProp = (MovePropAction)propData.Action;
            translationEdit.OnlyText = moveProp.Translation.ToString();
            rotationEdit.OnlyText = moveProp.Rotation.ToString();
        }

        public Vector3 Translation
        {
            get
            {
                return moveProp.Translation;
            }
            set
            {
                moveProp.Translation = value;
                translationEdit.OnlyText = moveProp.Translation.ToString();
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return moveProp.Rotation;
            }
            set
            {
                moveProp.Rotation = value;
                rotationEdit.OnlyText = moveProp.Rotation.ToString();
            }
        }
    }
}
