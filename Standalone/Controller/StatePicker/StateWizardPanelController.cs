using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class StateWizardPanelController : IDisposable
    {
        List<StateWizardPanel> panels = new List<StateWizardPanel>();

        public StateWizardPanelController(Gui gui, BasicGUI basicGUI)
        {
            panels.Add(new StateWizardPanel("DistortionPanels/DopplerPanel.layout", gui.findWidgetT("TestDoppler") as Button, basicGUI));
            panels.Add(new ToothPanel("DistortionPanels/BottomTeethRemovalPanel.layout", gui.findWidgetT("TestBottomTeeth") as Button, basicGUI));
            panels.Add(new ToothPanel("DistortionPanels/TopTeethRemovalPanel.layout", gui.findWidgetT("TestTopTeeth") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/LeftCondylarGrowthPanel.layout", gui.findWidgetT("TestLeftCondylarGrowth") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/LeftCondyleDegenerationPanel.layout", gui.findWidgetT("TestLeftCondylarDegeneration") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/RightCondylarGrowthPanel.layout", gui.findWidgetT("TestRightCondylarGrowth") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/RightCondyleDegenerationPanel.layout", gui.findWidgetT("TestRightCondylarDegeneration") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/LeftFossaPanel.layout", gui.findWidgetT("TestLeftFossa") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/RightFossaPanel.layout", gui.findWidgetT("TestRightFossa") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/DiscSpacePanel.layout", gui.findWidgetT("TestDiscSpace") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/DisclaimerPanel.layout", gui.findWidgetT("TestDisclaimer") as Button, basicGUI));
            panels.Add(new StateWizardPanel("DistortionPanels/NotesPanel.layout", gui.findWidgetT("TestNotes") as Button, basicGUI));
        }

        public void Dispose()
        {
            foreach (StateWizardPanel panel in panels)
            {
                panel.Dispose();
            }
        }
    }
}
