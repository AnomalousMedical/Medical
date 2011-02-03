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
            Button diagnose = widget.findWidget("Diagnose") as Button;
            diagnose.MouseButtonClick += new MyGUIEvent(diagnose_MouseButtonClick);

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

        void diagnose_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.MedicalStateController.clearStates();
            standaloneController.MedicalStateController.createNormalStateFromScene();
            standaloneController.TimelineController.ResourceLocation = "S:/export/Timelines/One Minute Doppler.tlp";
            Timeline tl = standaloneController.TimelineController.openTimeline("A Startup.tl");
            standaloneController.TimelineController.startPlayback(tl);
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
