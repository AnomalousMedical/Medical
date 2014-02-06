using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.SlideshowActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    internal class MedicalRmlSlide : Slide
    {
        private MedicalRmlSlide()
        {

        }

        protected MedicalRmlSlide(LoadInfo info)
            :base(info)
        {
            //Consider a version if statment for this, might not need if you remove the medicalrmlslides somehow
            if (info.hasValue("rml"))
            {
                RmlSlidePanel panel = new RmlSlidePanel();
                InlineRmlUpgradeCache.setRml(panel, info.GetString("rml"));
                panel.ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left);
                panel.Size = 480;
                addPanel(panel);
            }
            if (info.hasValue("layers"))
            {
                this.StartupAction = new SetupSceneAction("Show", info.GetValue<CameraPosition>("cameraPosition", null), info.GetValue<LayerState>("layers", null), info.GetValue<MusclePosition>("musclePosition", null), info.GetValue<PresetState>("medicalState", null));
            }
        }
    }
}
