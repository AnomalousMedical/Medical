using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowPropProperties : TimelineDataPanel
    {
        private ShowPropAction showProp;
        private bool comboUninitialized = true;

        private ComboBox propTypes;
        private Edit translationEdit;
        private Edit rotationEdit;

        public ShowPropProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.ShowPropProperties.layout")
        {
            propTypes = mainWidget.findWidget("PropTypeCombo") as ComboBox;
            propTypes.EventComboChangePosition += new MyGUIEvent(propTypes_EventComboChangePosition);

            translationEdit = mainWidget.findWidget("TranslationEdit") as Edit;
            translationEdit.EventEditSelectAccept += new MyGUIEvent(translationEdit_EventEditSelectAccept);

            rotationEdit = mainWidget.findWidget("RotationEdit") as Edit;
            rotationEdit.EventEditSelectAccept += new MyGUIEvent(rotationEdit_EventEditSelectAccept);
        }

        public override void setCurrentData(TimelineData data)
        {
            showProp = (ShowPropAction)((TimelineActionData)data).Action;
            if (comboUninitialized)
            {
                PropFactory propFactory = showProp.TimelineController.PropFactory;
                foreach (String propName in propFactory.PropNames)
                {
                    propTypes.addItem(propName);
                }
                comboUninitialized = false;
            }
            uint index = propTypes.findItemIndexWith(showProp.PropType);
            if (index != ComboBox.Invalid)
            {
                propTypes.SelectedIndex = index;
            }
            translationEdit.Caption = showProp.Translation.ToString();
            Vector3 euler = showProp.Rotation.getEuler();
            euler *= 57.2957795f;
            rotationEdit.Caption = euler.ToString();
        }

        void propTypes_EventComboChangePosition(Widget source, EventArgs e)
        {
            showProp.PropType = propTypes.getItemNameAt(propTypes.SelectedIndex);
        }

        void translationEdit_EventEditSelectAccept(Widget source, EventArgs e)
        {
            Vector3 trans = new Vector3();
            trans.setValue(translationEdit.Caption);
            showProp.Translation = trans;
        }

        void rotationEdit_EventEditSelectAccept(Widget source, EventArgs e)
        {
            Vector3 euler = new Vector3();
            euler.setValue(rotationEdit.Caption);
            euler *= 0.0174532925f;
            Quaternion rotation = new Quaternion(euler.x, euler.y, euler.z);
            showProp.Rotation = rotation;
        }
    }
}
