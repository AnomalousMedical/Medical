using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class DistortionsGUIController : IDisposable
    {
        List<WizardPanel> panels = new List<WizardPanel>();

        public DistortionsGUIController(Gui gui, BasicGUI basicGUI)
        {
            panels.Add(new WizardPanel("DistortionPanels/DopplerPanel.layout", gui.findWidgetT("TestDoppler") as Button, basicGUI));
            panels.Add(new ToothPanel("DistortionPanels/BottomTeethRemovalPanel.layout", gui.findWidgetT("TestBottomTeeth") as Button, basicGUI));
            panels.Add(new ToothPanel("DistortionPanels/TopTeethRemovalPanel.layout", gui.findWidgetT("TestTopTeeth") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/LeftCondylarGrowthPanel.layout", gui.findWidgetT("TestLeftCondylarGrowth") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/LeftCondyleDegenerationPanel.layout", gui.findWidgetT("TestLeftCondylarDegeneration") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/RightCondylarGrowthPanel.layout", gui.findWidgetT("TestRightCondylarGrowth") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/RightCondyleDegenerationPanel.layout", gui.findWidgetT("TestRightCondylarDegeneration") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/LeftFossaPanel.layout", gui.findWidgetT("TestLeftFossa") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/RightFossaPanel.layout", gui.findWidgetT("TestRightFossa") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/DiscSpacePanel.layout", gui.findWidgetT("TestDiscSpace") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/DisclaimerPanel.layout", gui.findWidgetT("TestDisclaimer") as Button, basicGUI));
            panels.Add(new WizardPanel("DistortionPanels/NotesPanel.layout", gui.findWidgetT("TestNotes") as Button, basicGUI));
        }

        public void Dispose()
        {
            foreach (WizardPanel panel in panels)
            {
                panel.Dispose();
            }
        }
    }
}
