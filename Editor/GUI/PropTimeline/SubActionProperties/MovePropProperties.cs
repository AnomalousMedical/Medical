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
            translationEdit.EventEditSelectAccept += new MyGUIEvent(translationEdit_EventEditSelectAccept);

            rotationEdit = mainWidget.findWidget("RotationEdit") as Edit;
            rotationEdit.EventEditSelectAccept += new MyGUIEvent(rotationEdit_EventEditSelectAccept);
        }

        public override void setCurrentData(TimelineData data)
        {
            PropTimelineData propData = (PropTimelineData)data;
            moveProp = (MovePropAction)propData.Action;
            translationEdit.OnlyText = moveProp.Translation.ToString();
            rotationEdit.OnlyText = moveProp.Rotation.getEuler().ToString();
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
                rotationEdit.OnlyText = moveProp.Rotation.getEuler().ToString();
            }
        }

        void translationEdit_EventEditSelectAccept(Widget source, EventArgs e)
        {
            Vector3 trans = new Vector3();
            trans.setValue(translationEdit.OnlyText);
            moveProp.Translation = trans;
        }

        void rotationEdit_EventEditSelectAccept(Widget source, EventArgs e)
        {
            Vector3 euler = new Vector3();
            euler.setValue(rotationEdit.OnlyText);
            euler *= 0.0174532925f;
            Quaternion rotation = new Quaternion(euler.x, euler.y, euler.z);
            moveProp.Rotation = rotation;
        }
    }
}
