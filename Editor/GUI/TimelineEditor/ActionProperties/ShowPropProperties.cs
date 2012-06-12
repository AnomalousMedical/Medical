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
        private TimelineActionData actionData;
        private bool comboUninitialized = true;

        private ComboBox propTypes;
        private EditBox translationEdit;
        private EditBox rotationEdit;
        private NumericEdit fadeDurationEdit;
        private ButtonGroup toolButtonGroup = new ButtonGroup();

        private Button translationButton;
        private Button rotationButton;
        private Button keepOpenButton;

        private PropEditController propEditController;
        private OpenPropManager propManager;

        public ShowPropProperties(Widget parentWidget, PropEditController propEditController, OpenPropManager propManager)
            :base(parentWidget, "Medical.GUI.TimelineEditor.ActionProperties.ShowPropProperties.layout")
        {
            this.propManager = propManager;
            propManager.PropClosed += new EventDelegate<OpenPropManager, ShowPropAction>(propManager_PropClosed);

            propTypes = mainWidget.findWidget("PropTypeCombo") as ComboBox;
            propTypes.EventComboChangePosition += new MyGUIEvent(propTypes_EventComboChangePosition);

            translationEdit = mainWidget.findWidget("TranslationEdit") as EditBox;
            translationEdit.EventEditSelectAccept += new MyGUIEvent(translationEdit_EventEditSelectAccept);

            rotationEdit = mainWidget.findWidget("RotationEdit") as EditBox;
            rotationEdit.EventEditSelectAccept += new MyGUIEvent(rotationEdit_EventEditSelectAccept);

            fadeDurationEdit = new NumericEdit(mainWidget.findWidget("FadeDurationEdit") as EditBox);
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

            this.propEditController = propEditController;

            Button moveToStart = (Button)mainWidget.findWidget("startPosButton");
            moveToStart.MouseButtonClick += new MyGUIEvent(moveToStart_MouseButtonClick);

            keepOpenButton = (Button)mainWidget.findWidget("keepOpen");
            keepOpenButton.MouseButtonClick += new MyGUIEvent(keepOpenButton_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            if (actionData != null)
            {
                actionData.DurationChanged -= actionData_DurationChanged;
            }
            if (showProp != null)
            {
                showProp.Translated -= showProp_Translated;
                showProp.Rotated -= showProp_Rotated;
            }
            actionData = ((TimelineActionData)data);
            showProp = (ShowPropAction)actionData.Action;
            if(actionData != null)
            {
                actionData.DurationChanged += actionData_DurationChanged;
            }
            if (showProp != null)
            {
                showProp.Translated += showProp_Translated;
                showProp.Rotated += showProp_Rotated;
            }

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
            fadeDurationEdit.FloatValue = showProp.FadeDuration;
            propEditController.CurrentShowPropAction = showProp;
            keepOpenButton.Selected = showProp.KeepOpen;
        }

        public override void editingCompleted()
        {
            showProp = null;
            propEditController.CurrentShowPropAction = null;
        }

        void propTypes_EventComboChangePosition(Widget source, EventArgs e)
        {
            if (showProp.SubActionCount == 0)
            {
                showProp.PropType = propTypes.getItemNameAt(propTypes.SelectedIndex);
                propEditController.CurrentShowPropAction = showProp;
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
                propEditController.CurrentShowPropAction = showProp;
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
            //simObjectMover.ShowMoveTools = toolButtonGroup.SelectedButton == translationButton;
            //simObjectMover.ShowRotateTools = toolButtonGroup.SelectedButton == rotationButton;
        }

        void actionData_DurationChanged(float duration)
        {
            propEditController.Duration = duration;
        }

        void moveToStart_MouseButtonClick(Widget source, EventArgs e)
        {
            showProp.moveToPropStartPosition();
            showProp._movePreviewProp(showProp.Translation, showProp.Rotation);
        }

        void keepOpenButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (keepOpenButton.Selected)
            {
                propManager.removeOpenProp(showProp);
            }
            else
            {
                propManager.addOpenProp(showProp);
                if (!propManager.Visible)
                {
                    propManager.Visible = true;
                }
            }
            keepOpenButton.Selected = showProp.KeepOpen;
        }

        void propManager_PropClosed(OpenPropManager source, ShowPropAction arg)
        {
            if (arg == showProp)
            {
                keepOpenButton.Selected = arg.KeepOpen;
            }
        }

        void showProp_Rotated(ShowPropAction obj)
        {
            Vector3 euler = obj.Rotation.getEuler();
            euler *= 57.2957795f;
            rotationEdit.Caption = euler.ToString();
        }

        void showProp_Translated(ShowPropAction obj)
        {
            translationEdit.Caption = obj.Translation.ToString();
        }
    }
}
