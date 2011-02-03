using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class Intro : Dialog
    {
        DopplerGUIPlugin dopplerGUI;

        public Intro(String programName, DopplerGUIPlugin dopplerGUI)
            :base("Medical.GUI.Intro.Intro.layout")
        {
            this.dopplerGUI = dopplerGUI;

            Edit introText = window.findWidget("IntroText") as Edit;
            introText.Caption = introText.Caption.Replace("$(PROGRAM_NAME)", programName);

            Button detailedButton = window.findWidget("DetailedButton") as Button;
            detailedButton.MouseButtonClick += new MyGUIEvent(detailedButton_MouseButtonClick);

            Button quickButton = window.findWidget("QuickButton") as Button;
            quickButton.MouseButtonClick += new MyGUIEvent(quickButton_MouseButtonClick);

            Button sandboxButton = window.findWidget("SandboxButton") as Button;
            sandboxButton.MouseButtonClick += new MyGUIEvent(sandboxButton_MouseButtonClick);
        }

        void sandboxButton_MouseButtonClick(Widget source, EventArgs e)
        {
            dopplerGUI.startSandboxMode();
            this.close();
        }

        void quickButton_MouseButtonClick(Widget source, EventArgs e)
        {
            dopplerGUI.runQuickDiagnosis();
            this.close();
        }

        void detailedButton_MouseButtonClick(Widget source, EventArgs e)
        {
            dopplerGUI.runDetailedDiagnosis();
            this.close();
        }
    }
}
