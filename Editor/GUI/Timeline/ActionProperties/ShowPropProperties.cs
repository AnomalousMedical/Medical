using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowPropProperties : TimelineDataPanel, MovableObject
    {
        private ShowPropAction showProp;
        private TimelineActionData actionData;
        private bool comboUninitialized = true;
        private SimObjectMover simObjectMover;

        private ComboBox propTypes;
        private Edit translationEdit;
        private Edit rotationEdit;
        private NumericEdit fadeDurationEdit;
        private ButtonGroup toolButtonGroup = new ButtonGroup();

        private Button translationButton;
        private Button rotationButton;

        private PropTimeline propTimeline;
        private Button propTimelineButton;

        public ShowPropProperties(Widget parentWidget, PropTimeline propTimeline, SimObjectMover simObjectMover)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.ShowPropProperties.layout")
        {
            this.simObjectMover = simObjectMover;

            propTypes = mainWidget.findWidget("PropTypeCombo") as ComboBox;
            propTypes.EventComboChangePosition += new MyGUIEvent(propTypes_EventComboChangePosition);

            translationEdit = mainWidget.findWidget("TranslationEdit") as Edit;
            translationEdit.EventEditSelectAccept += new MyGUIEvent(translationEdit_EventEditSelectAccept);

            rotationEdit = mainWidget.findWidget("RotationEdit") as Edit;
            rotationEdit.EventEditSelectAccept += new MyGUIEvent(rotationEdit_EventEditSelectAccept);

            fadeDurationEdit = new NumericEdit(mainWidget.findWidget("FadeDurationEdit") as Edit);
            fadeDurationEdit.ValueChanged += new MyGUIEvent(fadeDurationEdit_ValueChanged);
            fadeDurationEdit.MinValue = 0.0f;
            fadeDurationEdit.MaxValue = 100.0f;
            fadeDurationEdit.AllowFloat = true;

            translationButton = mainWidget.findWidget("TranslationButton") as Button;
            toolButtonGroup.addButton(translationButton);

            rotationButton = mainWidget.findWidget("RotationButton") as Button;
            toolButtonGroup.addButton(rotationButton);

            toolButtonGroup.SelectedButton = translationButton;
            toolButtonGroup.SelectedButtonChanged += new EventHandler(toolButtonGroup_SelectedButtonChanged);

            this.propTimeline = propTimeline;
            propTimeline.UpdatePropPreview += new EventHandler(propTimeline_UpdatePropPreview);
            propTimeline.Shown += new EventHandler(propTimeline_Shown);
            propTimeline.Closed += new EventHandler(propTimeline_Closed);

            propTimelineButton = mainWidget.findWidget("PropTimelineButton") as Button;
            propTimelineButton.MouseButtonClick += new MyGUIEvent(propTimelineButton_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            if (actionData != null)
            {
                actionData.DurationChanged -= actionData_DurationChanged;
            }
            actionData = ((TimelineActionData)data);
            showProp = (ShowPropAction)actionData.Action;
            if(actionData != null)
            {
                actionData.DurationChanged += actionData_DurationChanged;
            }

            if (comboUninitialized)
            {
                simObjectMover.ShowMoveTools = true;
                simObjectMover.ToolSize = 3.0f;
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
            fadeDurationEdit.FloatValue = showProp.FadeDuration;
            simObjectMover.setActivePlanes(MovementAxis.All, MovementPlane.All);
            simObjectMover.addMovableObject("Prop", this);
            simObjectMover.setDrawingSurfaceVisible(true);
            propTimeline.setPropData(showProp);
        }

        public override void editingCompleted()
        {
            showProp = null;
            simObjectMover.removeMovableObject(this);
            simObjectMover.setDrawingSurfaceVisible(false);
            propTimeline.setPropData(null);
        }

        void propTypes_EventComboChangePosition(Widget source, EventArgs e)
        {
            if (showProp.SubActionCount == 0)
            {
                showProp.PropType = propTypes.getItemNameAt(propTypes.SelectedIndex);
                propTimeline.setPropData(showProp);
            }
            else
            {
                MessageBox.show("Warning changing the prop type for a prop that has subactions will erase all subactions on that prop. Do you wish to continue?", "Erase Actions", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, confirmEraseProps);
            }
        }

        void confirmEraseProps(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                showProp.clearSubActions();
                showProp.PropType = propTypes.getItemNameAt(propTypes.SelectedIndex);
                propTimeline.setPropData(showProp);
            }
            else
            {
                propTypes.SelectedIndex = propTypes.findItemIndexWith(showProp.PropType);
            }
        }

        void translationEdit_EventEditSelectAccept(Widget source, EventArgs e)
        {
            Vector3 trans = new Vector3();
            trans.setValue(translationEdit.OnlyText);
            showProp.Translation = trans;
        }

        void rotationEdit_EventEditSelectAccept(Widget source, EventArgs e)
        {
            Vector3 euler = new Vector3();
            euler.setValue(rotationEdit.OnlyText);
            euler *= 0.0174532925f;
            Quaternion rotation = new Quaternion(euler.x, euler.y, euler.z);
            showProp.Rotation = rotation;
        }

        void fadeDurationEdit_ValueChanged(Widget source, EventArgs e)
        {
            showProp.FadeDuration = fadeDurationEdit.FloatValue;
        }

        void toolButtonGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            simObjectMover.ShowMoveTools = toolButtonGroup.SelectedButton == translationButton;
            simObjectMover.ShowRotateTools = toolButtonGroup.SelectedButton == rotationButton;
        }

        void propTimelineButton_MouseButtonClick(Widget source, EventArgs e)
        {
            propTimeline.Visible = !propTimeline.Visible;
        }

        void actionData_DurationChanged(float duration)
        {
            propTimeline.Duration = duration;
        }

        void propTimeline_UpdatePropPreview(object sender, EventArgs e)
        {
            if (propTimeline.UsingTools)
            {
                showProp._movePreviewProp(propTimeline.Translation, propTimeline.Rotation);
            }
            else
            {
                showProp._movePreviewProp(showProp.Translation, showProp.Rotation);
            }
        }

        void propTimeline_Closed(object sender, EventArgs e)
        {
            propTimelineButton.StateCheck = false;
        }

        void propTimeline_Shown(object sender, EventArgs e)
        {
            propTimelineButton.StateCheck = true;
        }

        #region MovableObject Members

        public Vector3 ToolTranslation
        {
            get
            {
                if (propTimeline.UsingTools)
                {
                    return propTimeline.Translation;
                }
                else
                {
                    return showProp.Translation;
                }
            }
        }

        public void move(Vector3 offset)
        {
            if (propTimeline.UsingTools)
            {
                propTimeline.Translation += offset;
                showProp._movePreviewProp(propTimeline.Translation, propTimeline.Rotation);
            }
            else
            {
                showProp.Translation += offset;
                translationEdit.Caption = showProp.Translation.ToString();
            }
        }

        public Quaternion ToolRotation
        {
            get
            {
                if (propTimeline.UsingTools)
                {
                    return propTimeline.Rotation;
                }
                else
                {
                    return showProp.Rotation;
                }
            }
        }

        public bool ShowTools
        {
            get { return true; }
        }

        public void rotate(ref Quaternion newRot)
        {
            if (propTimeline.UsingTools)
            {
                propTimeline.Rotation = newRot;
                showProp._movePreviewProp(propTimeline.Translation, propTimeline.Rotation);
            }
            else
            {
                showProp.Rotation = newRot;
                Vector3 euler = showProp.Rotation.getEuler();
                euler *= 57.2957795f;
                rotationEdit.Caption = euler.ToString();
            }
        }

        public void alertToolHighlightStatus(bool highlighted)
        {
            
        }

        #endregion
    }
}
