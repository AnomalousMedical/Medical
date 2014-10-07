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

        public OffsetKeyframeProperties(Widget parentWidget, OffsetSequenceEditor sequenceEditor, MedicalUICallback uiCallback)
            : base(parentWidget, "Medical.GUI.OffsetSequence.OffsetKeyframeProperties.layout")
        {
            this.sequenceEditor = sequenceEditor;

            propertiesForm = new PropertiesForm(mainWidget, uiCallback);
        }

        public override void Dispose()
        {
            propertiesForm.Dispose();
            base.Dispose();
        }

        public override void setCurrentData(TimelineData data)
        {
            if(propertiesForm.EditInterface != null)
            {
                propertiesForm.EditInterface.OnDataNeedsRefresh -= editInterface_OnDataNeedsRefresh;
            }
            keyframe = ((OffsetKeyframeData)data).KeyFrame;
            propertiesForm.EditInterface = keyframe.EditInterface;
            propertiesForm.EditInterface.OnDataNeedsRefresh += editInterface_OnDataNeedsRefresh;
            mainWidget.Height = propertiesForm.Height;
        }

        void editInterface_OnDataNeedsRefresh(EditInterface editInterface)
        {
            if (sequenceEditor.Player != null)
            {
                sequenceEditor.Player.blend(keyframe.BlendAmount);
            }
        }
    }
}
