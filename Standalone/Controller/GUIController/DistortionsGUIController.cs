using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class WizardTestPanel : IDisposable
    {
        private Layout layout;
        private MyGUILayoutContainer layoutContainer;
        private Widget mainWidget;
        private BasicGUI basicGUI;
        private Button button;

        public WizardTestPanel(String file, Button button, BasicGUI basicGUI)
        {
            this.button = button;
            this.basicGUI = basicGUI;
            button.MouseButtonClick += button_MouseButtonClick;
            layout = LayoutManager.Instance.loadLayout(file);
            mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            layoutContainer = new MyGUILayoutContainer(mainWidget);
        }

        public void Dispose()
        {
            button.MouseButtonClick -= button_MouseButtonClick;
            LayoutManager.Instance.unloadLayout(layout);
            layout = null;
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            if (mainWidget.Visible)
            {
                basicGUI.changeLeftPanel(null);
            }
            else
            {
                basicGUI.changeLeftPanel(layoutContainer);
            }
        }
    }

    class DistortionsGUIController : IDisposable
    {
        List<WizardTestPanel> panels = new List<WizardTestPanel>();

        public DistortionsGUIController(Gui gui, BasicGUI basicGUI)
        {
            panels.Add(new WizardTestPanel("DistortionPanels/DopplerPanel.layout", gui.findWidgetT("TestDoppler") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/BottomTeethRemovalPanel.layout", gui.findWidgetT("TestBottomTeeth") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/TopTeethRemovalPanel.layout", gui.findWidgetT("TestTopTeeth") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/LeftCondylarGrowthPanel.layout", gui.findWidgetT("TestLeftCondylarGrowth") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/LeftCondyleDegenerationPanel.layout", gui.findWidgetT("TestLeftCondylarDegeneration") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/RightCondylarGrowthPanel.layout", gui.findWidgetT("TestRightCondylarGrowth") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/RightCondyleDegenerationPanel.layout", gui.findWidgetT("TestRightCondylarDegeneration") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/LeftFossaPanel.layout", gui.findWidgetT("TestLeftFossa") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/RightFossaPanel.layout", gui.findWidgetT("TestRightFossa") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/DiscSpacePanel.layout", gui.findWidgetT("TestDiscSpace") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/DisclaimerPanel.layout", gui.findWidgetT("TestDisclaimer") as Button, basicGUI));
            panels.Add(new WizardTestPanel("DistortionPanels/NotesPanel.layout", gui.findWidgetT("TestNotes") as Button, basicGUI));
        }

        public void Dispose()
        {
            foreach (WizardTestPanel panel in panels)
            {
                panel.Dispose();
            }
        }
    }
}
