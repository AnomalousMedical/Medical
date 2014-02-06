using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.SlideshowActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// There used to be a class called MedicalRmlSlide that was our base slide saved to a bunch of slideshows.
    /// This class converts any of those found to be loading to plain slides.
    /// </summary>
    class MedicalRmlSlideUpdater : ObjectIdentifier
    {
        public static void Touch()
        {
            ObjectIdentifierFactory.AddCreationMethod("Medical.MedicalRmlSlide, Standalone", CreateObjectIdentifier);
        }

        static ObjectIdentifier CreateObjectIdentifier(long id, String assemblyQualifiedName, TypeFinder typeFinder)
        {
            return new MedicalRmlSlideUpdater(id);
        }

        public MedicalRmlSlideUpdater(long objectId)
            :base(objectId, null, typeof(Slide))
        {

        }

        public override object restoreObject(LoadInfo info)
        {
            Slide slide = (Slide)base.restoreObject(info);
            //Scan the loadInfo and see if there is anything we need to upgrade.
            if (info.hasValue("rml"))
            {
                RmlSlidePanel panel = new RmlSlidePanel();
                InlineRmlUpgradeCache.setRml(panel, info.GetString("rml"));
                panel.ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left);
                panel.Size = 480;
                slide.addPanel(panel);
            }
            if (info.hasValue("layers"))
            {
                slide.StartupAction = new SetupSceneAction("Show", info.GetValue<CameraPosition>("cameraPosition", null), info.GetValue<LayerState>("layers", null), info.GetValue<MusclePosition>("musclePosition", null), info.GetValue<PresetState>("medicalState", null));
            }
            return slide;
        }
    }
}
