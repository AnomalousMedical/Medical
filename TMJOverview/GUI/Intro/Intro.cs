﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.Diagnostics;

namespace Medical.GUI
{
    class Intro : Dialog
    {
        TMJOverviewGUIPlugin tmjOverviewGUI;
        Widget currentSlide;

        public Intro(String programName, TMJOverviewGUIPlugin tmjOverviewGUI)
            :base("Medical.GUI.Intro.Intro.layout")
        {
            this.tmjOverviewGUI = tmjOverviewGUI;

            currentSlide = window.findWidget("StartSlide");
            if (currentSlide != null)
            {
                Button nextButton = currentSlide.findWidget(currentSlide.getUserString("NextButton")) as Button;
                if (nextButton != null)
                {
                    nextButton.MouseButtonClick += new MyGUIEvent(nextButton_MouseButtonClick);
                }
            }

            Button anomalousMedicalLink = window.findWidget("AnomalousMedicalLink") as Button;
            anomalousMedicalLink.MouseButtonClick += new MyGUIEvent(anomalousMedicalLink_MouseButtonClick);

            Button viewTMJOverview = window.findWidget("TMJOverview") as Button;
            viewTMJOverview.MouseButtonClick += new MyGUIEvent(viewTMJOverview_MouseButtonClick);
        }

        void viewTMJOverview_MouseButtonClick(Widget source, EventArgs e)
        {
            tmjOverviewGUI.runTMJOverview();
            this.close();
        }

        void anomalousMedicalLink_MouseButtonClick(Widget source, EventArgs e)
        {
            Process.Start("http://www.anomalousmedical.com");
        }

        void nextButton_MouseButtonClick(Widget source, EventArgs e)
        {
            currentSlide.Visible = false;
            currentSlide = window.findWidget(currentSlide.getUserString("NextSlide"));
            if (currentSlide != null)
            {
                currentSlide.Visible = true;
                Button nextButton = currentSlide.findWidget(currentSlide.getUserString("NextButton")) as Button;
                if (nextButton != null)
                {
                    nextButton.MouseButtonClick += new MyGUIEvent(nextButton_MouseButtonClick);
                }
            }
        }
    }
}
