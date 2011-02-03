using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using System.Reflection;
using Medical.GUI;

namespace Medical.GUI
{
    class DopplerAppMenu : PopupContainer, AppMenu
    {
        private DopplerGUIPlugin dopplerGUI;
        private StandaloneController standaloneController;

        public DopplerAppMenu(DopplerGUIPlugin dopplerGUI, StandaloneController standaloneController)
            :base("Medical.GUI.Menus.DopplerAppMenu.layout")
        {
            this.dopplerGUI = dopplerGUI;
            this.standaloneController = standaloneController;

            //Diagnose
            Button detailedDiagnose = widget.findWidget("DetailedDiagnose") as Button;
            detailedDiagnose.MouseButtonClick += new MyGUIEvent(detailedDiagnose_MouseButtonClick);

            Button quickDiagnose = widget.findWidget("QuickDiagnose") as Button;
            quickDiagnose.MouseButtonClick += new MyGUIEvent(quickDiagnose_MouseButtonClick);

            //Export
            Button export = widget.findWidget("Export") as Button;
            export.MouseButtonClick += new MyGUIEvent(export_MouseButtonClick);

            //Options
            Button optionsButton = widget.findWidget("Options") as Button;
            optionsButton.MouseButtonClick += new MyGUIEvent(optionsButton_MouseButtonClick);

            //Help
            Button helpButton = widget.findWidget("Help") as Button;
            helpButton.MouseButtonClick += new MyGUIEvent(helpButton_MouseButtonClick);

            //About
            Button aboutButton = widget.findWidget("About") as Button;
            aboutButton.MouseButtonClick += new MyGUIEvent(aboutButton_MouseButtonClick);

            //Update
            Button updateButton = widget.findWidget("Updates") as Button;
            updateButton.MouseButtonClick += new MyGUIEvent(updateButton_MouseButtonClick);

            //Quit
            Button quitButton = widget.findWidget("File/Quit") as Button;
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);
        }

        void detailedDiagnose_MouseButtonClick(Widget source, EventArgs e)
        {
            dopplerGUI.runDetailedDiagnosis();
            this.hide();
        }

        void quickDiagnose_MouseButtonClick(Widget source, EventArgs e)
        {
            dopplerGUI.runQuickDiagnosis();
            this.hide();
        }

        void export_MouseButtonClick(Widget source, EventArgs e)
        {
            dopplerGUI.export();
            this.hide();
        }

        void quitButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.closeMainWindow();
            this.hide();
        }

        void optionsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            dopplerGUI.showOptions();
            this.hide();
        }

        void aboutButton_MouseButtonClick(Widget source, EventArgs e)
        {
            dopplerGUI.showAboutDialog();
            this.hide();
        }

        void updateButton_MouseButtonClick(Widget source, EventArgs e)
        {
            UpdateManager.checkForUpdates(Assembly.GetAssembly(this.GetType()).GetName().Version);
            this.hide();
        }

        void helpButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.openHelpTopic(0);
            this.hide();
        }
    }
}
