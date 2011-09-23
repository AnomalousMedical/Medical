using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TimelineWizardGUIManager : IDisposable
    {
        private TimelineWizard timelineWizard;

        public TimelineWizardGUIManager(StandaloneController standaloneController)
        {
            Gui.Instance.load("Medical.Resources.WizardImagesets.xml");

            timelineWizard = new TimelineWizard(standaloneController);

            standaloneController.TimelineGUIFactory.addPrototype(new RemoveTopTeethGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RemoveBottomTeethGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new DisclaimerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDiscSpaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDiscSpaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDopplerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDopplerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new FossaGUILeftPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new FossaGUIRightPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftCondylarDegenerationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightCondylarDegenerationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftCondylarGrowthGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightCondylarGrowthGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDiscClockFaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDiscClockFaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new ProfileDistortionGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new TeethAdaptationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new TeethHeightAdaptationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new NotesGUIPrototype(timelineWizard));
        }

        public void Dispose()
        {
            timelineWizard.Dispose();
        }
    }
}
