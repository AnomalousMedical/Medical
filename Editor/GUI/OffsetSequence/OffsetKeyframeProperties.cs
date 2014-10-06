using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class OffsetKeyframeProperties : TimelineDataPanel
    {
        private OffsetModifierKeyframe keyframe;
        private OffsetSequenceEditor sequenceEditor;
        private PropertiesForm propertiesForm;
        private EditInterface editInterface;

        public OffsetKeyframeProperties(Widget parentWidget, OffsetSequenceEditor sequenceEditor, MedicalUICallback uiCallback)
            : base(parentWidget, "Medical.GUI.OffsetSequence.OffsetKeyframeProperties.layout")
        {
            this.sequenceEditor = sequenceEditor;

            propertiesForm = new PropertiesForm(mainWidget, uiCallback);

            editInterface = ReflectedEditInterface.createEditInterface(this, "Properties");
            editInterface.addCommand(new EditInterfaceCommand("Use Current", (callback, caller) =>
            {
                if (sequenceEditor.Player != null && keyframe != null)
                {
                    sequenceEditor.Player.setKeyframeOffset(keyframe);
                    editInterface.fireDataNeedsRefresh();
                }
            }));
        }

        public override void Dispose()
        {
            propertiesForm.Dispose();
            base.Dispose();
        }

        public override void setCurrentData(TimelineData data)
        {
            keyframe = ((OffsetKeyframeData)data).KeyFrame;
            propertiesForm.EditInterface = editInterface;
            mainWidget.Height = propertiesForm.Height;
        }

        [Editable]
        public Vector3 TranslationOffset
        {
            get
            {
                if(keyframe != null)
                {
                    return keyframe.TranslationOffset;
                }
                return Vector3.Zero;
            }
            set
            {
                if(keyframe != null)
                {
                    keyframe.TranslationOffset = value;
                    updatePlayer();
                }
            }
        }

        [Editable]
        public Quaternion RotationOffset
        {
            get
            {
                if (keyframe != null)
                {
                    return keyframe.RotationOffset;
                }
                return Quaternion.Identity;
            }
            set
            {
                if (keyframe != null)
                {
                    keyframe.RotationOffset = value;
                    updatePlayer();
                }
            }
        }

        private void updatePlayer()
        {
            if (sequenceEditor.Player != null)
            {
                sequenceEditor.Player.blend(keyframe.BlendAmount);
            }
        }
    }
}
